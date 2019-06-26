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
      string retVal = "";
      ServerSettings serverSettings = senderDetail.ServerSettings;
      CommandMessageHelper command = new CommandMessageHelper(serverSettings.CommandSymbol, args.Message);
      string[] commandParams = command.CommandParams;
      Command[] commands = commandController.GetCommandsForLanguage(senderDetail.ServerSettings.Language);
      Discord.Color responseColor = Discord.Color.Green;

      if (string.IsNullOrWhiteSpace(command.CommandDetail))
      {
        retVal = string.Join(", ", commands.Select(c => c.Aliases.First()));
      }
      else
      {
        string search = command.CommandArgs.First().ToLowerInvariant();
        Command found = commands.FirstOrDefault(c => c.Aliases.Contains(search, StringComparer.OrdinalIgnoreCase));
        if (found != null)
        {
          retVal = found.Help + "\r\n" + found.Examples;
        }
        else
        {
          retVal = ($"{Emojis.QuestionSymbol} {languageHandler.GetPhrase(senderDetail.ServerSettings.Language, "Error_IncorrectParameter")}");
          responseColor = Discord.Color.Red;
        }
      }

      Response response = new Response
      {
        Embed = Utility.EmbedUtility.StringToEmbed(retVal, responseColor),
        Message = retVal,
        ResponseType = ResponseType.Default
      };
      return new[] { response };
    }
  }
}