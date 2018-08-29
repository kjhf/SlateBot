using SlateBot.Language;
using SlateBot.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlateBot.Commands.Replace
{
  public class ReplaceCommand : Command
  {
    private readonly LanguageHandler languageHandler;
    private readonly IDictionary<string, string> replaceTable;
    private readonly bool ignoreCase;

    /// <summary>
    /// Replace the message with the supplied characters.
    /// </summary>
    /// <param name="aliases"></param>
    /// <param name="choices"></param>
    /// <param name="help"></param>
    public ReplaceCommand(LanguageHandler languageHandler, IEnumerable<string> aliases, string examples, string help, ModuleType module, IDictionary<string, string> replacements, bool ignoreCase)
      : base(CommandHandlerType.Replace, aliases?.ToArray(), examples, help, module)
    {
      this.languageHandler = languageHandler;
      this.replaceTable = replacements;
      this.ignoreCase = ignoreCase;
    }

    public override IList<Response> Execute(SenderSettings senderDetail, IMessageDetail args)
    {
      ServerSettings serverSettings = senderDetail.ServerSettings;
      CommandMessageHelper command = new CommandMessageHelper(serverSettings.CommandSymbol, args.Message);
      string[] commandParams = command.CommandParams;

      // First, substitute the variables.
      string message = VariableStrings.Replace(command.CommandDetail, args.Username, args.UserId.ToString(), args.GuildName, command.CommandDetail);

      if (string.IsNullOrWhiteSpace(message))
      {
        // Use the command's name instead.
        message = command.FullCommandExcludingCommandPrefix;
      }

      // Shrug.
      message = message.StripAccents();

      // Now substitute the replacements from the table.
      StringBuilder sb = new StringBuilder();
      foreach (char c in message)
      {
        string letter = c.ToString();
        foreach (var replacementPair in replaceTable)
        {
          letter = letter.Replace(replacementPair.Key, replacementPair.Value, (ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal));

          // If a replacement was made.
          if (letter != c.ToString())
          {
            break;
          }
        }
        sb.Append(letter);
      }

      Response response = new Response
      {
        command = this,
        embed = null,
        message = sb.ToString(),
        responseType = ResponseType.Default
      };
      return new[] { response };
    }

    protected override List<KeyValuePair<string, string>> ConstructExtraData()
    {
      var retVal = new List<KeyValuePair<string, string>>(1 + (replaceTable.Count * 2))
      {
        // Extra data is IgnoreCase
        { "IgnoreCase", ignoreCase.ToString() }
      };

      // And the replacements
      foreach (var pair in replaceTable)
      {
        retVal.Add("Old", pair.Key);
        retVal.Add("New", pair.Value);
      }

      return retVal;
    }
  }
}