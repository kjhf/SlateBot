using SlateBot.Commands;
using SlateBot.DAL.CommandFile;
using SlateBot.Errors;
using SlateBot.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SlateBot
{
  internal class CommandController : IController
  {
    private readonly Dictionary<CommandHandlerType, ICommandHandler> commandHandlers;
    private readonly List<Command> commands;
    private readonly IErrorLogger errorLogger;
    private readonly DAL.SlateBotDAL slateBotDAL;
    private readonly LanguageHandler languageHandler;

    public CommandController(IErrorLogger errorLogger, DAL.SlateBotDAL slateBotDAL, LanguageHandler languageHandler)
    {
      this.errorLogger = errorLogger ?? throw new ArgumentNullException(nameof(errorLogger));
      this.slateBotDAL = slateBotDAL ?? throw new ArgumentNullException(nameof(slateBotDAL));
      this.languageHandler = languageHandler ?? throw new ArgumentNullException(nameof(languageHandler));
      this.commandHandlers = new Dictionary<CommandHandlerType, ICommandHandler>();
      this.commands = new List<Command>();

      // Get all ICommandHandler in the assembly
      var allCommandHandlerTypes = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.GetInterfaces().Contains(typeof(ICommandHandler)));

      // Instantiate them
      foreach (var commandHandlerType in allCommandHandlerTypes)
      {
        var handler = (ICommandHandler)Activator.CreateInstance(commandHandlerType);

        if (commandHandlers.TryGetValue(handler.CommandHandlerType, out ICommandHandler dummy))
        {
          errorLogger.LogError(new Error(ErrorCode.CommandHandlerNotImplemented, ErrorSeverity.Error, "Duplicate command handler found: " + handler.CommandHandlerType + " in type " + handler.GetType()));
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
            errorLogger.LogError(new Error(ErrorCode.CommandHandlerNotImplemented, ErrorSeverity.Warning, cht.ToString()));
          }
        }
      }
    }

    public void Initialise()
    {
      LoadCommands();
    }

    private void LoadCommands()
    {
      commands.Clear();

      var commandFiles = slateBotDAL.ReadCommandFiles();

      foreach (var pair in commandFiles)
      {
        var language = pair.Key;

        // Translate each CommandFile into Commands.
        foreach (CommandFile file in pair.Value)
        {
          ICommandHandler commandHandler = GetCommandHandlerFromCommandHandlerType(file.CommandType);
          if (commandHandler == null)
          {
            errorLogger.LogError(new Error(ErrorCode.CommandHandlerNotImplemented, ErrorSeverity.Error, $"{file.CommandType} specified in an xml file with aliases {file.AliasesStr}"));
          }
          else
          {
            Command c = commandHandler.CreateCommand(languageHandler, file);
            if (c == null)
            {
              errorLogger.LogError(new Error(ErrorCode.CommandNotValid, ErrorSeverity.Error, $"Found handler but did not create a {file.CommandType} specified in an xml file with aliases {file.AliasesStr}"));
            }
            else
            {
              commands.Add(c);
            }
          }
        }
      }
    }

    private ICommandHandler GetCommandHandlerFromCommandHandlerType(string search)
    {
      bool success = Enum.TryParse(search, out CommandHandlerType commandHandlerType);
      if (success)
      {
        bool found = commandHandlers.TryGetValue(commandHandlerType, out ICommandHandler handler);

        if (!found)
        {
          errorLogger.LogError(new Error(ErrorCode.CommandHandlerNotImplemented, ErrorSeverity.Error, $"{search} exists but does not have an associated command handler."));
        }

        return handler;
      }

      // Else 
      return null;
    }

    internal Tuple<Command, string> ExecuteCommand(SenderDetail senderDetail, IMessageDetail messageDetail)
    {
      string response = null;
      Commands.CommandMessageHelper helper = new CommandMessageHelper(senderDetail.ServerSettings.CommandSymbol, messageDetail.Message);
      Command foundCommand = null;
      foreach (Command command in commands)
      {
        if (command.Aliases.Contains(helper.CommandLower, StringComparer.OrdinalIgnoreCase))
        {
          response = command.Execute(senderDetail, messageDetail);
          foundCommand = command;
          break;
        }
      }

      return new Tuple<Command, string>(foundCommand, response);
    }
  }
}
