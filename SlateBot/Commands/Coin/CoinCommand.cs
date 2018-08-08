using SlateBot.Language;
using System.Collections.Generic;
using System.Text;

namespace SlateBot.Commands.Coin
{
  public class CoinCommand : Command
  {
    private LanguageHandler languageHandler;

    internal CoinCommand(LanguageHandler languageHandler, string[] aliases, string examples, string help, ModuleType module)
      : base(CommandHandlerType.Coin, aliases, examples, help, module)
    {
      this.languageHandler = languageHandler;
    }

    public override IList<Response> Execute(SenderSettings senderDetail, IMessageDetail args)
    {
      string retVal = "";
      ServerSettings serverSettings = senderDetail.ServerSettings;
      CommandMessageHelper command = new CommandMessageHelper(serverSettings.CommandSymbol, args.Message);
      string[] commandParams = command.CommandParams;
      ushort flips;
      Discord.Color responseColor = new Discord.Color(0);

      if (commandParams.Length == 1)
      {
        flips = 1;
      }
      else
      {
        if (!ushort.TryParse(commandParams[1], out flips) || flips <= 0)
        {
          retVal = ($"{Emojis.CrossSymbol} {languageHandler.GetPhrase(serverSettings.Language, Errors.ErrorCode.IncorrectParameter)}: {commandParams[1]}. [1-{uint.MaxValue}].");
        }
      }

      if (flips > 0)
      {
        int heads = 0, tails = 0;
        string localeHead = languageHandler.GetPhrase(serverSettings.Language, "CoinHead");
        string localeTail = languageHandler.GetPhrase(serverSettings.Language, "CoinTail");

        if (flips == 1)
        {
          switch (rand.Next(2))
          {
            default:
            case 0: retVal = localeHead; heads++; break;
            case 1: retVal = localeTail; tails++; break;
          }
        }
        else
        {
          char headLetter = (localeHead)[0];
          char tailLetter = (localeTail)[0];

          string flipped = languageHandler.GetPhrase(serverSettings.Language, "Flipped");
          if (flips > 100)
          {
            for (int i = 0; i < flips; i++)
            {
              switch (rand.Next(2))
              {
                case 0: heads++; break;
                case 1: tails++; break;
              }
            }
            retVal = ($"{flipped} {flips}x, `{localeHead}:` {heads}, `{localeTail}:` {tails}");
          }
          else
          {
            StringBuilder coinflips = new StringBuilder();
            for (int i = 0; i < flips; i++)
            {
              switch (rand.Next(2))
              {
                case 0: coinflips.Append(headLetter); heads++; break;
                case 1: coinflips.Append(tailLetter); tails++; break;
              }
            }

            retVal = ($"{flipped} {flips}x, `{localeHead}:` {heads}, `{localeTail}:` {tails}: {coinflips.ToString()}");
          }
        }

        if (heads < tails)
        {
          responseColor = new Discord.Color(200, 50, 50);
        }
        else if (heads > tails)
        {
          responseColor = new Discord.Color(50, 200, 50);
        }
        else
        {
          responseColor = new Discord.Color(200, 200, 50);
        }
      }

      Response response = new Response
      {
        command = this,
        embed = Utility.EmbedUtility.StringToEmbed(retVal, responseColor),
        message = retVal,
        responseType = ResponseType.Default
      };
      return new[] { response };
    }
  }
}