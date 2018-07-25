﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SlateBot
{
  class SlateBotProgram
  {
    #region Static and Main
    private static SlateBotProgram instance;

    static void Main(string[] args)
    {
      Console.Title = nameof(SlateBot);
      instance = new SlateBotProgram(args);

      try
      {
        instance.MainLoop();
      }
      catch (Exception ex)
      {
        instance.controller.ErrorLogger.LogException(ex, Errors.ErrorSeverity.Fatal);
      }

      Console.ReadLine();
    }
    #endregion


    
    private SlateBotController controller;
    private readonly ExitCodes lastExitCode;

    public SlateBotProgram(string[] args)
    {
      controller = new SlateBotController();

      lastExitCode = ExitCodes.Success;
      if (args.Length > 1)
      {
        Enum.TryParse<ExitCodes>(args[1], out lastExitCode);
      }
    }

    /// <summary>
    /// The Main Loop of the console. Waits for input to execute commands.
    /// </summary>
    private void MainLoop()
    {
      Console.WriteLine("C: Connect");
      Console.WriteLine("D: Disconnect");
      Console.WriteLine("#: Connection options");
      Console.WriteLine("L: Load app settings");
      Console.WriteLine("r: Restart");
      Console.WriteLine("t: Change title");
      Console.WriteLine("x: Close");
      Console.WriteLine("\\: Test code");

      // Load commands
      controller.Initialise();

      // Connet now?
      switch (lastExitCode)
      {
        case ExitCodes.SuccessReconnect:
        case ExitCodes.RestartErrorReconnect:
        {
          controller.Connect();
          break;
        }
      }

      for (; ; )
      {
        try
        {
          char keyPressed = '\0';
          string line = Console.ReadLine();
          if (line.Length == 1)
          {
            keyPressed = line[0];
            Console.WriteLine();

            switch (keyPressed)
            {
              case 'C':
              {
                controller.Connect();
                break;
              }

              case 'D':
              {
                controller.Disconnect();
                break;
              }

              case '#':
              {
                // TODO - connection options
                Console.WriteLine("Not implemented.");
                break;
              }

              case 'L':
              {
                controller.Initialise();
                break;
              }

              case 'r':
              {
                Restart(ExitCodes.RestartRequested);
                break;
              }

              case 't':
              {
                Console.WriteLine("Enter a new title.");
                Console.Title = (Console.ReadLine());
                Console.WriteLine(Environment.NewLine);
                break;
              }

              case 'x':
              {
                controller.Disconnect();

                // Closes the current process
                Environment.Exit((int)ExitCodes.Success);
                break;
              }
            }
          }
          else
          {
            // It's a command instead.
            controller.HandleConsoleCommand(line);
          }
        }
        catch (Exception ex)
        {
          controller.ErrorLogger.LogDebug($"Unhandled exception risen to {nameof(MainLoop)}.", true);
          controller.ErrorLogger.LogException(ex, Errors.ErrorSeverity.Fatal);
        }
      }
    }

    private static void Restart(ExitCodes code)
    {
      // Starts a new instance of the program itself
      Process.Start(Assembly.GetExecutingAssembly().Location, code.ToString());

      // Closes the current process
      Environment.Exit((int)code);
    }
  }
}
