using SlateBot.DAL.CommandFile;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SlateBot.Commands.Coin
{
  public class CoinCommand : Command
  {
    private readonly Language.LanguageHandler languageHandler;
    private string[] aliases = new[] { "coin" };
    private string examples = Constants.BotMention + " coin";
    private string help = "Flips heads/tails on a coin.";
    private ModuleType module = ModuleType.General;
    private ResponseType responseType = ResponseType.Default;

    internal CoinCommand(Language.LanguageHandler languageHandler)
    {
      this.languageHandler = languageHandler;
    }

    internal CoinCommand(Language.LanguageHandler languageHandler, string[] aliases, string examples, string help, ModuleType module, ResponseType responseType)
    {
      this.languageHandler = languageHandler;
      this.aliases = aliases;
      this.examples = examples;
      this.help = help;
      this.module = module;
      this.responseType = responseType;
    }

    public override string[] Aliases => aliases;
    public override CommandHandlerType CommandHandlerType => CommandHandlerType.Coin;
    public override string Examples => examples;
    public override Dictionary<string, string> ExtraData => ConstructExtraData();
    public override string Help => help;
    public override ModuleType Module => module;
    public override ResponseType ResponseType => responseType;

    public override string Execute(SenderDetail senderDetail, IMessageDetail args)
    {
      ServerSettings serverSettings = senderDetail.ServerSettings;
      CommandMessageHelper command = new CommandMessageHelper(serverSettings.CommandSymbol, args.Message);
      string[] commandParams = command.CommandParams;
      ushort flips;
      if (commandParams.Length == 1)
      {
        flips = 1;
      }
      else
      {
        if (!ushort.TryParse(commandParams[1], out flips))
        {
          return ($"{Emojis.CrossSymbol} {languageHandler.GetPhrase(serverSettings.Language, Errors.ErrorCode.IncorrectParameter)}: {commandParams[1]}. [0-{uint.MaxValue}].");
        }
      }

      string localeHead = languageHandler.GetPhrase(serverSettings.Language, "CoinHead");
      string localeTail = languageHandler.GetPhrase(serverSettings.Language, "CoinTail");
      string retVal;

      if (flips == 1)
      {
        switch (rand.Next(2))
        {
          default:
          case 0: retVal = localeHead; break;
          case 1: retVal = localeTail; break;
        }
      }
      else
      {
        char headLetter = (localeHead)[0];
        char tailLetter = (localeTail)[0];

        string flipped = languageHandler.GetPhrase(serverSettings.Language, "Flipped");
        if (flips > 100)
        {
          int heads = 0, tails = 0;
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
          int heads = 0, tails = 0;
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

      return retVal;
    }

    private Dictionary<string, string> ConstructExtraData()
    {
      Dictionary<string, string> retVal = new Dictionary<string, string>();
      // No extra data for the coin command.
      return retVal;
    }
  }
}