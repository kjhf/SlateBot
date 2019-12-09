using CsHelper;
using SlateBot.Language;
using SlateBot.Utility;
using System;
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
          if (commandParams[1] == "∞" || commandParams[1].StartsWith("inf"))
          {
            max = long.MaxValue;
          }
          else if (commandParams[1] == "-∞" || commandParams[1].StartsWith("-inf"))
          {
            max = long.MinValue;
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
          if (commandParams[1] == "∞" || commandParams[1].StartsWith("inf"))
          {
            min = long.MaxValue;
          }
          else if (commandParams[1] == "-∞" || commandParams[1].StartsWith("-inf"))
          {
            min = long.MinValue;
          }
          else
          {
            output.AppendLine($"{Emojis.CrossSymbol} {languageHandler.GetPhrase(serverSettings.Language, Errors.ErrorCode.IncorrectParameter)}: {commandParams[1]}.");
            min = 0;
          }
        }

        if (!long.TryParse(commandParams[2], NumberStyles.Any, CultureInfo.InvariantCulture, out max))
        {
          if (commandParams[2] == "∞" || commandParams[2].StartsWith("inf"))
          {
            max = long.MaxValue;
          }
          else if (commandParams[2] == "-∞" || commandParams[2].StartsWith("-inf"))
          {
            max = long.MinValue;
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

      long num = rand.NextLong(min, max == long.MaxValue ? long.MaxValue : max + 1);
      output.AppendLine((min) + " -> " + (max) + ": " + num);

      ulong range = (ulong)(max - min);
      if (range == 0) range = 1;

      long normNum = (num - min);      
      float percentage = (((float)normNum) / range) * 360;
      if (percentage > 360) { percentage = 360; }
      var drawingColour = Imaging.ImageManipulator.FromAHSB(255, percentage, 0.8f, 0.5f);
      var responseColor = new Discord.Color(drawingColour.R, drawingColour.G, drawingColour.B);

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