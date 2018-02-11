﻿using SlateBot.Commands;
using SlateBot.DAL.CommandFile;
using SlateBot.Errors;
using SlateBot.Language;
using System;
using System.Collections.Generic;
using System.Linq;
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
      foreach (CommandHandlerType cht in Enum.GetValues(typeof(CommandHandlerType)))
      {
        switch (cht)
        {
          // TODO add all other handlers.
          case CommandHandlerType.Unknown:
            break;

          case CommandHandlerType.Coin:
            commandHandlers[cht] = new Commands.Coin.CoinCommandHandler();
            break;

          default:
            errorLogger.LogError(new Error(ErrorCode.CommandHandlerNotImplemented, ErrorSeverity.Warning, cht.ToString()));
            break;
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
            commands.Add(commandHandler.CreateCommand(languageHandler, file));
          }
        }
      }
    }

    private ICommandHandler GetCommandHandlerFromCommandHandlerType(string search)
    {
      bool success = Enum.TryParse(search, out CommandHandlerType commandHandlerType);
      if (success)
      {
        return commandHandlers[commandHandlerType];
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
