using SlateBot.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlateBot.Commands.Help
{
  public class HelpCommand : Command
  {
    private readonly CommandController commandController;
    private readonly LanguageHandler languageHandler;

    internal HelpCommand(CommandController commandController, LanguageHandler languageHandler, string[] aliases, string examples, string help, ModuleType module)
      : base(CommandHandlerType.Help, aliases, examples, help, module)
    {
      this.commandController = commandController;
      this.languageHandler = languageHandler;
    }

    public override IList<Response> Execute(SenderSettings senderDetail, IMessageDetail args)
    {
      StringBuilder retVal = new StringBuilder();
      ServerSettings serverSettings = senderDetail.ServerSettings;
      CommandMessageHelper command = new CommandMessageHelper(serverSettings.CommandSymbol, args.Message);
      string[] commandParams = command.CommandParams;
      Command[] commands = commandController.GetCommandsForLanguage(senderDetail.ServerSettings.Language);
      Discord.Color responseColor = Discord.Color.Green;

      if (string.IsNullOrWhiteSpace(command.CommandDetail))
      {
        var commandsByModule = commands.GroupBy(x => x.Module).ToDictionary(grouping => grouping.Key, grouping => grouping.ToList());
        foreach (var pair in commandsByModule)
        {
          retVal.Append("**");
          retVal.Append(pair.Key.ToString());
          retVal.AppendLine("**");
          retVal.AppendLine(string.Join(", ", pair.Value.Select(c => c.Aliases[0])));
          retVal.AppendLine();
        }
      }
      else
      {
        string search = command.CommandArgs.First().ToLowerInvariant();
        Command found = commands.FirstOrDefault(c => c.Aliases.Contains(search, StringComparer.OrdinalIgnoreCase));
        if (found != null)
        {
          retVal.AppendLine(found.Help + "\r\n" + found.Examples);
        }
        else
        {
          retVal.AppendLine($"{Emojis.QuestionSymbol} {languageHandler.GetPhrase(senderDetail.ServerSettings.Language, "Error_IncorrectParameter")}");
          responseColor = Discord.Color.Red;
        }
      }

      string message = retVal.ToString();
      Response response = new Response
      {
        Embed = Utility.EmbedUtility.ToEmbed(message, responseColor),
        Message = message,
        ResponseType = ResponseType.Default
      };
      return new[] { response };
    }
  }
}