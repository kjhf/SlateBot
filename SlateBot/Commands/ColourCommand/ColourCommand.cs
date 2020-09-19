using SlateBot.Imaging;
using System;
using System.Collections.Generic;
using System.DrawingCore;

namespace SlateBot.Commands.Colour
{
  public class ColourCommand : Command
  {
    internal ColourCommand(string[] aliases, string examples, string help, ModuleType module)
      : base(CommandHandlerType.Colour, aliases, examples, help, module)
    {
    }

    public override IList<Response> Execute(SenderSettings senderDetail, IMessageDetail args)
    {
      ServerSettings serverSettings = senderDetail.ServerSettings;
      CommandMessageHelper command = new CommandMessageHelper(serverSettings.CommandSymbol, args.Message);

      Discord.Color responseColor;
      string message;

      if (ImageManipulator.FindHexColour(command.CommandDetail, out string foundColour))
      {
        var color = ImageManipulator.FromHexCode(foundColour);
        responseColor = new Discord.Color(color.R, color.G, color.B);

        var knownColor = color.ToKnownColor();
        message = knownColor != 0 ? $"✓ {knownColor}" : $"(~) {color.ToConsoleColor()}";
      }
      else if (Enum.TryParse(command.CommandDetail, true, out KnownColor knownColor))
      {
        var color = Color.FromKnownColor(knownColor);
        responseColor = new Discord.Color(color.R, color.G, color.B);
        message = $"✓ {knownColor}";
      }
      else
      {
        responseColor = Discord.Color.Default;
        message = $"{Emojis.QuestionSymbol} {command.CommandDetail}";
      }

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