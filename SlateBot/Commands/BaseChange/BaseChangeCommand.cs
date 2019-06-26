using SlateBot.Language;
using SlateBot.Utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SlateBot.Commands.BaseChange
{
  public class BaseChangeCommand : Command
  {
    private readonly int fromBase;
    private readonly LanguageHandler languageHandler;
    private readonly int toBase;

    internal BaseChangeCommand(LanguageHandler languageHandler, string[] aliases, string examples, string help, ModuleType module, int fromBase, int toBase)
      : base(CommandHandlerType.BaseChange, aliases, examples, help, module)
    {
      this.languageHandler = languageHandler;
      this.fromBase = fromBase;
      this.toBase = toBase;
    }

    public override IList<Response> Execute(SenderSettings senderDetail, IMessageDetail args)
    {
      StringBuilder sb = new StringBuilder();
      ServerSettings serverSettings = senderDetail.ServerSettings;
      CommandMessageHelper command = new CommandMessageHelper(serverSettings.CommandSymbol, args.Message);

      string commandDetail = command.CommandDetail;
      Discord.Color? embedColour = null;

      if (string.IsNullOrEmpty(commandDetail))
      {
        sb.AppendLine(Help);
      }
      else
      {
        // Remove decorators
        commandDetail = commandDetail.Replace("0x", "").Replace("0b", "").Replace("#", "");

        // Replace commas with spaces
        commandDetail = commandDetail.Replace(",", " ").Trim();

        // Special case hex to decimal in cases of 6 or 8 characters.
        bool is6Chars = commandDetail.Length == 6;
        bool is8Chars = commandDetail.Length == 8;
        if (fromBase == 16 && (is6Chars || is8Chars))
        {
          // Try and convert the source hex number into a colour.
          if (uint.TryParse(commandDetail, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint argb))
          {
            // Remove alpha channel (N.B. this usually makes it transparent as a = 0, but Discord fixes this to fully opaque)
            argb &= 0x00FFFFFF;
            embedColour = new Discord.Color(argb);
          }

          commandDetail += " ";
          commandDetail += commandDetail.Substring(0, 2);
          commandDetail += " ";
          commandDetail += commandDetail.Substring(2, 2);
          commandDetail += " ";
          commandDetail += commandDetail.Substring(4, 2);
          if (is8Chars)
          {
            commandDetail += " ";
            commandDetail += commandDetail.Substring(6, 2);
          }
        }

        string[] commandParams = commandDetail.Split(' ').Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();

        if (fromBase == 10)
        {
          // Try and convert the source decimal number(s) into a colour.
          if (commandParams.Length == 1)
          {
            if (uint.TryParse(commandParams[0], out uint argb))
            {
              // Remove alpha channel (N.B. this usually makes it transparent as a = 0, but Discord fixes this to fully opaque)
              argb &= 0x00FFFFFF;
              embedColour = new Discord.Color(argb);
            }
          }
          else if (commandParams.Length == 3 || commandParams.Length == 4)
          {
            // Using -n here as alpha could be included at the front.
            bool canParseColour = (byte.TryParse(commandParams[commandParams.Length - 3], out byte r));
            canParseColour &= (byte.TryParse(commandParams[commandParams.Length - 2], out byte g));
            canParseColour &= (byte.TryParse(commandParams[commandParams.Length - 1], out byte b));

            if (canParseColour)
            {
              embedColour = new Discord.Color(r, g, b);
            }
          }
        }

        foreach (string col in commandParams)
        {
          try
          {
            switch (fromBase)
            {
              case 2:
              case 8:
              case 10:
              case 16:
              {
                long part = Convert.ToInt64(col, fromBase);
                byte[] parts = BitConverter.GetBytes(part);
                if (toBase == 64)
                {
                  sb.Append(Convert.ToBase64String(parts) + " ");
                }
                else if (toBase == 16)
                {
                  sb.Append(part.ToString("X2") + " ");
                }
                else
                {
                  sb.Append(Convert.ToString(part, toBase) + " ");
                }
                break;
              }

              case 64:
              {
                byte[] parts = Convert.FromBase64String(col);
                long part = BitConverter.ToInt64(parts, 0);

                if (toBase == 64)
                {
                  sb.Append(Convert.ToBase64String(parts) + " ");
                }
                else if (toBase == 16)
                {
                  sb.Append(part.ToString("X2") + " ");
                }
                else
                {
                  sb.Append(Convert.ToString(part, toBase) + " ");
                }
                break;
              }
            }
          }
          catch (FormatException)
          {
            sb.AppendLine($"{Emojis.CrossSymbol} {languageHandler.GetPhrase(serverSettings.Language, "Error_IncorrectParameter")}: {col}.");
          }
          catch (Exception ex)
          {
            sb.AppendLine($"{Emojis.CrossSymbol} {languageHandler.GetPhrase(serverSettings.Language, "Error_Oops")}: {col} {ex.Message}");
          }
        }

        // If multiple things to translate, also provide the answer without spaces.
        if (command.CommandParams.Length > 2)
        {
          string outputWithoutSpace = sb.ToString().Replace(" ", "");
          sb.AppendLine();
          switch (toBase)
          {
            case 2: sb.Append("0b "); break;
            case 8: sb.Append("0o "); break;
            case 16: sb.Append("0x "); break;
          }
          sb.AppendLine(outputWithoutSpace);
        }
      }

      string output = sb.ToString();
      Response response = new Response
      {
        Embed = EmbedUtility.StringToEmbed(output, embedColour),
        Message = output,
        ResponseType = ResponseType.Default
      };

      return new[] { response };
    }

    protected override List<KeyValuePair<string, string>> ConstructExtraData()
    {
      var retVal = new List<KeyValuePair<string, string>>
      {
        { "FromBase", fromBase.ToString() },
        { "ToBase", toBase.ToString() }
      };

      return retVal;
    }
  }
}