using CsHelper;
using SlateBot.Language;
using System.Collections.Generic;
using System.Text;

namespace SlateBot.Commands.Cheerleader
{
  public class CheerleaderCommand : Command
  {
    private readonly LanguageHandler languageHandler;

    /// <summary>
    /// Respond to the message with a random choice.
    /// </summary>
    /// <param name="aliases"></param>
    /// <param name="choices"></param>
    /// <param name="help"></param>
    public CheerleaderCommand(LanguageHandler languageHandler, string[] aliases, string examples, string help, ModuleType module)
      : base(CommandHandlerType.Cheerleader, aliases, examples, help, module)
    {
      this.languageHandler = languageHandler;
    }

    public override IList<Response> Execute(SenderSettings senderDetail, IMessageDetail args)
    {
      ServerSettings serverSettings = senderDetail.ServerSettings;
      CommandMessageHelper command = new CommandMessageHelper(serverSettings.CommandSymbol, args.Message);

      StringBuilder sb = new StringBuilder();
      string charArray = command.CommandDetail.StripAccents();
      int length = charArray.Length;

      if (length > 80)
      {
        string err = ($"{Emojis.NoEntry} {languageHandler.GetPhrase(serverSettings.Language, "Error_NotAFile")}");
        sb.AppendLine(err);
      }
      else
      {
        sb.AppendLine();
        foreach (char c in charArray)
        {
          if (char.IsLetterOrDigit(c))
          {
            sb.Append(":regional_indicator_").Append(char.ToLowerInvariant(c)).Append(": ");
          }
          else if (c == ' ')
          {
            sb.Append(":blue_heart: ");
          }
          else if (c == '!')
          {
            sb.Append(":grey_exclamation: ");
          }
          else if (c == '?')
          {
            sb.Append(":grey_question: ");
          }
          else if (c == '\'')
          {
            sb.Append(":arrow_down_small: ");
          }
          else
          {
            sb.Append(c);
          }
        }

        sb.AppendLine();

        foreach (char _ in charArray)
        {
          sb.Append(Emojis.CheerleaderSymbols[rand.Next(0, Emojis.CheerleaderSymbols.Count)]).Append(' ');
        }
      }

      return Response.CreateArrayFromString(sb.ToString());
    }
  }
}