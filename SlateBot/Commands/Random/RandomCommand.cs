using SlateBot.Language;
using SlateBot.Utility;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SlateBot.Commands.Random
{
  public class RandomCommand : Command
  {
    private readonly LanguageHandler languageHandler;

    internal RandomCommand(LanguageHandler languageHandler, string[] aliases, string examples, string help, ModuleType module)
      : base(CommandHandlerType.Random, aliases, examples, help, module)
    {
      this.languageHandler = languageHandler;
    }

    public override IList<Response> Execute(SenderSettings senderDetail, IMessageDetail args)
    {
      StringBuilder output = new StringBuilder();
      ServerSettings serverSettings = senderDetail.ServerSettings;
      CommandMessageHelper command = new CommandMessageHelper(serverSettings.CommandSymbol, args.Message);
      string[] commandParams = command.CommandParams;

      long min = 0;
      long max;
      if (commandParams.Length == 1)
      {
        max = 10;
      }
      else if (commandParams.Length == 2)
      {
        if (!long.TryParse(commandParams[1], NumberStyles.Any, CultureInfo.InvariantCulture, out max))
        {
          if (commandParams[1] == "∞")
          {
            max = long.MaxValue;
          }
          else
          {
            output.AppendLine($"{Emojis.CrossSymbol} {languageHandler.GetPhrase(serverSettings.Language, Errors.ErrorCode.IncorrectParameter)}: {commandParams[1]}.");
            max = 10;
          }
        }
      }
      else
      {
        if (!long.TryParse(commandParams[1], NumberStyles.Any, CultureInfo.InvariantCulture, out min))
        {
          output.AppendLine($"{Emojis.CrossSymbol} {languageHandler.GetPhrase(serverSettings.Language, Errors.ErrorCode.IncorrectParameter)}: {commandParams[1]}.");
          min = 0;
        }

        if (!long.TryParse(commandParams[2], NumberStyles.Any, CultureInfo.InvariantCulture, out max))
        {
          if (commandParams[2] == "∞")
          {
            max = long.MaxValue;
          }
          else
          {
            output.AppendLine($"{Emojis.CrossSymbol} {languageHandler.GetPhrase(serverSettings.Language, Errors.ErrorCode.IncorrectParameter)}: {commandParams[2]}.");
            max = 10;
          }
        }
      }
      if (min > max)
      {
        // Swap
        long temp = max;
        max = min;
        min = temp;
      }
      long num = rand.NextLong(min, max + 1);
      output.AppendLine((min) + " -> " + (max) + ": " + num);
      var responseColor = new Discord.Color((uint)Imaging.ImageManipulator.FromAHSB(255, (float)min / max, 1.0f, 1.0f).ToArgb());

      string retVal = output.ToString();
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