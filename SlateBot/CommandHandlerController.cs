using CsHelper;
using SlateBot.Commands;
using SlateBot.DAL.CommandFile;
using SlateBot.Errors;
using SlateBot.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace SlateBot
{
  internal class CommandController : IController
  {
    private readonly Dictionary<CommandHandlerType, ICommandHandler> commandHandlers;
    private readonly Dictionary<Languages, List<Command>> commands;
    private readonly SlateBotController controller;
    private readonly SynchronizationContext synchronizationContext;

    public CommandController(SlateBotController controller)
    {
      this.controller = controller ?? throw new ArgumentNullException(nameof(controller));
      this.commandHandlers = new Dictionary<CommandHandlerType, ICommandHandler>();
      this.commands = new Dictionary<Languages, List<Command>>();
      this.synchronizationContext = SynchronizationContext.Current;

      // Get all ICommandHandler in the assembly and instantiate them
      foreach (var commandHandlerType in Assembly
         .GetExecutingAssembly()
         .GetTypes()
         .Where(t => t.GetInterfaces().Contains(typeof(ICommandHandler))))
      {
        var handler = (ICommandHandler)Activator.CreateInstance(commandHandlerType);

        if (commandHandlers.TryGetValue(handler.CommandHandlerType, out ICommandHandler dummy))
        {
          controller.ErrorLogger.LogError(new Error(ErrorCode.CommandHandlerNotImplemented, ErrorSeverity.Error, "Duplicate command handler found: " + handler.CommandHandlerType + " in type " + handler.GetType()));
        }

        // Add it to the dictionary.
        commandHandlers[handler.CommandHandlerType] = handler;
      }

      // Check if any are missing.
      foreach (CommandHandlerType cht in Enum.GetValues(typeof(CommandHandlerType)))
      {
        if (!commandHandlers.TryGetValue(cht, out ICommandHandler dummy))
        {
          if (cht != CommandHandlerType.Unknown)
          {
            controller.ErrorLogger.LogError(new Error(ErrorCode.CommandHandlerNotImplemented, ErrorSeverity.Warning, cht.ToString()));
          }
        }
      }
    }

    public void Initialise()
    {
      if (synchronizationContext != null)
      {
        // CheckAccess is performed in here.
        synchronizationContext.Invoke(LoadCommands);
      }
      else
      {
        // If no sync context, just go ahead and load them.
        LoadCommands();
      }
    }

    public Command[] GetCommandsForLanguage(Languages language, bool includeDefault = true)
    {
      List<Command> retVal = new List<Command>();
      if (commands.TryGetValue(language, out List<Command> temp))
      {
        retVal.AddRange(temp);
      }

      if (includeDefault && language != Languages.Default)
      {
        retVal.AddRange(commands[Languages.Default]);
      }
      return retVal.ToArray();
    }

    private void LoadCommands()
    {
      commands.Clear();

      foreach (var pair in controller.dal.ReadCommandFiles())
      {
        var language = pair.Key;
        
        // Translate each CommandFile into Commands.
        foreach (CommandFile file in pair.Value)
        {
          ICommandHandler commandHandler = GetCommandHandlerFromCommandHandlerType(file.CommandType);
          if (commandHandler == null)
          {
            controller.ErrorLogger.LogError(new Error(ErrorCode.CommandHandlerNotImplemented, ErrorSeverity.Error, $"{file.CommandType} specified in an XML file with aliases {file.AliasesStr}"));
          }
          else
          {
            Command c = commandHandler.CreateCommand(controller, file);
            if (c == null)
            {
              controller.ErrorLogger.LogError(new Error(ErrorCode.CommandNotValid, ErrorSeverity.Error, $"Found handler but did not create a {file.CommandType} specified in an XML file with aliases {file.AliasesStr}"));
            }
            else
            {
              commands.AddToInner(language, c);
            }
          }
        }
      }
    }

    private ICommandHandler GetCommandHandlerFromCommandHandlerType(string search)
    {
      ICommandHandler handler = null;
      bool success = Enum.TryParse(search, out CommandHandlerType commandHandlerType);
      if (success)
      {
        bool found = commandHandlers.TryGetValue(commandHandlerType, out handler);

        if (!found)
        {
          controller.ErrorLogger.LogError(new Error(ErrorCode.CommandHandlerNotImplemented, ErrorSeverity.Error, $"{search} exists but does not have an associated command handler."));
        }
      }

      return handler;
    }

    internal IList<Response> ExecuteCommand(SenderSettings senderDetail, IMessageDetail messageDetail)
    {
      List<Response> responses = new List<Response>();
      CommandMessageHelper helper = new CommandMessageHelper(senderDetail.ServerSettings.CommandSymbol, messageDetail.Message);

      Languages currentLanguage = senderDetail.ServerSettings.Language;

      // If we're running default, assume English.
      if (currentLanguage == Languages.Default)
      {
        currentLanguage = Languages.English;
      }

      if (helper.IsCommand || messageDetail.IsPrivate)
      {
        // Check language specific commands.
        foreach (Command command in commands[currentLanguage])
        {
          if (command.Aliases.Contains(helper.CommandLower, StringComparer.OrdinalIgnoreCase) || command.NoSetAlias)
          {
            responses.AddRange(command.Execute(senderDetail, messageDetail));
          }
        }

        // Then check defaults.
        foreach (Command command in commands[Languages.Default])
        {
          if (command.Aliases.Contains(helper.CommandLower, StringComparer.OrdinalIgnoreCase) || command.NoSetAlias)
          {
            responses.AddRange(command.Execute(senderDetail, messageDetail));
          }
        }
      }
      else
      {
        // Check only commands that do not require a command symbol.
        // Check language specific commands.
        foreach (Command command in commands[currentLanguage].Where(c => !c.RequiresSymbol))
        {
          if (command.Aliases.Contains(helper.CommandLower, StringComparer.OrdinalIgnoreCase) || command.NoSetAlias)
          {
            responses.AddRange(command.Execute(senderDetail, messageDetail));
          }
        }

        // Then check defaults.
        foreach (Command command in commands[Languages.Default].Where(c => !c.RequiresSymbol))
        {
          if (command.Aliases.Contains(helper.CommandLower, StringComparer.OrdinalIgnoreCase) || command.NoSetAlias)
          {
            responses.AddRange(command.Execute(senderDetail, messageDetail));
          }
        }
      }

      return responses;
    }
  }
}