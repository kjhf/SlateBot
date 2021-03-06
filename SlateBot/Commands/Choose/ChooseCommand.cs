﻿using CsHelper;
using SlateBot.Language;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlateBot.Commands.Choose
{
  public class ChooseCommand : Command
  {
    private readonly LanguageHandler languageHandler;
    private readonly string delimiter;

    internal ChooseCommand(LanguageHandler languageHandler, string[] aliases, string examples, string help, ModuleType module, string delimiter)
      : base(CommandHandlerType.Choose, aliases, examples, help, module)
    {
      this.languageHandler = languageHandler;
      this.delimiter = delimiter;
    }

    public override IList<Response> Execute(SenderSettings senderDetail, IMessageDetail args)
    {
      StringBuilder sb = new StringBuilder();
      ServerSettings serverSettings = senderDetail.ServerSettings;
      CommandMessageHelper command = new CommandMessageHelper(serverSettings.CommandSymbol, args.Message);

      string commandDetail = command.CommandDetail;

      // Split by or delimiter, not space.
      if (command.FullCommandExcludingCommandPrefix.ToLowerInvariant().Contains(delimiter))
      {
        string[] orChoices = commandDetail.Split(delimiter);
        if (!orChoices.Any())
        {
          sb.AppendLine($"{command.CommandSymbol}{command.CommandLower} A{delimiter}B{delimiter}C...");
        }
        else
        {
          sb.AppendLine(orChoices[rand.Next(orChoices.Length)].Trim());
          sb.Append("_").Append(languageHandler.GetPhrase(senderDetail.ServerSettings.Language, "IgnoredOr")).Append("_");
        }
      }

      // Else split by space
      else
      {
        string[] choices = command.CommandArgs.ToArray();
        if (!choices.Any())
        {
          sb.AppendLine($"{command.CommandSymbol}{command.CommandLower} A B C ...");
        }
        else
        {
          sb.AppendLine(choices[rand.Next(choices.Length)]);
        }
      }

      return Response.CreateArrayFromString(sb.ToString());
    }

    protected override List<KeyValuePair<string, string>> ConstructExtraData()
    {
      var retVal = new List<KeyValuePair<string, string>>
      {
        { "Delimiter", delimiter }
      };

      return retVal;
    }
  }
}