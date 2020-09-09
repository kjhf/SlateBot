using System;
using System.Diagnostics;
using System.Reflection;

namespace SlateBot
{
  internal class SlateBotProgram
  {
    #region Static and Main

    private static SlateBotProgram instance;
    public static bool ConsoleIsPrivate = true;

    private static void Main(string[] args)
    {
      Console.Title = nameof(SlateBot);
      instance = new SlateBotProgram(args);

      try
      {
        Debug.WriteLine("RUNNING IN DEBUG.");
        if (Debugger.IsAttached)
        {
          Console.WriteLine("DEBUGGER IS ATTACHED.");
        }
        instance.MainLoop();
      }
      catch (Exception ex)
      {
        instance.controller.ErrorLogger.LogException(ex, Errors.ErrorSeverity.Fatal);
      }

      Console.ReadLine();
    }

    #endregion Static and Main

    private readonly SlateBotController controller;
    private readonly ExitCodes lastExitCode;

    public SlateBotProgram(string[] args)
    {
      AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
      controller = new SlateBotController();

      lastExitCode = ExitCodes.Success;
      if (args.Length > 1)
      {
        Enum.TryParse<ExitCodes>(args[1], out lastExitCode);
      }
    }

    private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
      // De-register the handler
      AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;

      // Die after pause
      Console.WriteLine("Uncaught exception: the program will terminate after the pause:");
      Console.WriteLine(e.ExceptionObject);
      Console.WriteLine();
      Console.WriteLine("Press any key to continue ...");
      Console.ReadKey();
    }

    /// <summary>
    /// The Main Loop of the console. Waits for input to execute commands.
    /// </summary>
    private void MainLoop()
    {
      Console.WriteLine("C: Connect");
      Console.WriteLine("d: Dump debug");
      Console.WriteLine("D: Disconnect");
      Console.WriteLine("#: Connection options");
      Console.WriteLine("L: Load app settings");
      Console.WriteLine("r: Restart");
      Console.WriteLine("t: Change title");
      Console.WriteLine("x: Close");
      Console.WriteLine("\\: Test code");
      Console.WriteLine("!: Toggle console is private");

      // Load commands
      controller.Initialise();

      // Connect now?
      switch (lastExitCode)
      {
        case ExitCodes.SuccessReconnect:
        case ExitCodes.RestartRequestedWithReconnect:
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

              case 'd':
              {
                controller.ToggleDumpDebug();
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
                if (controller.Connected)
                {
                  Restart(ExitCodes.RestartRequestedWithReconnect);
                }
                else
                {
                  Restart(ExitCodes.RestartRequestedNoReconnect);
                }
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

              case '!':
              {
                ConsoleIsPrivate = !ConsoleIsPrivate;
                Console.WriteLine("Console is private now: " + ConsoleIsPrivate);
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