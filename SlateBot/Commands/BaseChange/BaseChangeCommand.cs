using SlateBot.DAL.CommandFile;
using SlateBot.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlateBot.Commands.BaseChange
{
  public class BaseChangeCommand : Command
  {
    private readonly Language.LanguageHandler languageHandler;
    private string[] aliases;
    private string examples;
    private string help;
    private ModuleType module = ModuleType.General;
    private ResponseType responseType = ResponseType.Default;
    private readonly int fromBase;
    private readonly int toBase;

    internal BaseChangeCommand(Language.LanguageHandler languageHandler)
    {
      this.languageHandler = languageHandler;
    }

    internal BaseChangeCommand(Language.LanguageHandler languageHandler, string[] aliases, string examples, string help, ModuleType module, ResponseType responseType, int fromBase, int toBase)
    {
      this.languageHandler = languageHandler;
      this.aliases = aliases;
      this.examples = examples;
      this.help = help;
      this.module = module;
      this.responseType = responseType;
      this.fromBase = fromBase;
      this.toBase = toBase;
    }

    public override string[] Aliases => aliases;
    public override CommandHandlerType CommandHandlerType => CommandHandlerType.Coin;
    public override string Examples => examples;
    public override List<KeyValuePair<string, string>> ExtraData => ConstructExtraData();
    public override string Help => help;
    public override ModuleType Module => module;
    public override ResponseType ResponseType => responseType;

    public override string Execute(SenderSettings senderDetail, IMessageDetail args)
    {
      StringBuilder output = new StringBuilder();
      ServerSettings serverSettings = senderDetail.ServerSettings;
      CommandMessageHelper command = new CommandMessageHelper(serverSettings.CommandSymbol, args.Message);

      string commandDetail = command.CommandDetail;
      if (string.IsNullOrEmpty(commandDetail))
      {
        output.AppendLine(command.CommandSymbol + command.CommandParams[0] + " " + "100");
      }
      else
      {
        // Remove decorators
        commandDetail = commandDetail.Replace("0x", "").Replace("0b", "").Replace("#", "");

        // Replace commas with spaces
        commandDetail = commandDetail.Replace(",", " ").Trim();

        // Special case hex to decimal in cases of 6 or 8 characters.
        bool is8Chars = commandDetail.Length == 8;
        if (fromBase == 16 && (commandDetail.Length == 6 || is8Chars))
        {
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
                  output.Append(Convert.ToBase64String(parts) + " ");
                }
                else if (toBase == 16)
                {
                  output.Append(part.ToString("X2") + " ");
                }
                else
                {
                  output.Append(Convert.ToString(part, toBase) + " ");
                }
                break;
              }

              case 64:
              {
                byte[] parts = Convert.FromBase64String(col);
                long part = BitConverter.ToInt64(parts, 0);

                if (toBase == 64)
                {
                  output.Append(Convert.ToBase64String(parts) + " ");
                }
                else if (toBase == 16)
                {
                  output.Append(part.ToString("X2") + " ");
                }
                else
                {
                  output.Append(Convert.ToString(part, toBase) + " ");
                }
                break;
              }
            }
          }
          catch (FormatException)
          {
            output.AppendLine($"{Emojis.CrossSymbol} {languageHandler.GetPhrase(serverSettings.Language, "Error_IncorrectParameter")}: {col}.");
          }
          catch (Exception ex)
          {
            output.AppendLine($"{Emojis.CrossSymbol} {languageHandler.GetPhrase(serverSettings.Language, "Error_Oops")}: {col} {ex.Message}");
          }
        }

        // If multiple things to translate, also provide the answer without spaces.
        if (command.CommandParams.Length > 2)
        {
          string outputWithoutSpace = output.ToString().Replace(" ", "");
          output.AppendLine();
          output.AppendLine(outputWithoutSpace);
        }
      }

      return output.ToString();
    }

    private List<KeyValuePair<string, string>> ConstructExtraData()
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