using CsHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlateBot.Commands.Replace
{
  public class ReplaceCommand : Command
  {
    private readonly IDictionary<string, string> replaceTable;
    private readonly bool ignoreCase, reverse, doTTS;

    /// <summary>
    /// Replace the message with the supplied characters.
    /// </summary>
    /// <param name="aliases"></param>
    /// <param name="choices"></param>
    /// <param name="help"></param>
    public ReplaceCommand(IEnumerable<string> aliases, string examples, string help, ModuleType module, IDictionary<string, string> replacements, bool ignoreCase, bool reverse = false, bool doTTS = false)
      : base(CommandHandlerType.Replace, aliases?.ToArray(), examples, help, module)
    {
      this.replaceTable = replacements;
      this.ignoreCase = ignoreCase;
      this.reverse = reverse;
      this.doTTS = doTTS;
    }

    public override IList<Response> Execute(SenderSettings senderDetail, IMessageDetail args)
    {
      ServerSettings serverSettings = senderDetail.ServerSettings;
      CommandMessageHelper command = new CommandMessageHelper(serverSettings.CommandSymbol, args.Message);

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

      string output = reverse ? new string(sb.ToString().Reverse().ToArray()) : sb.ToString();
      List<Response> responses = new List<Response>
      {
        new Response
        {
          Embed = null,
          Message = output,
          ResponseType = ResponseType.Default
        }
      };

      if (doTTS)
      {
        responses.Add(
          new Response
          {
            Embed = null,
            Message = output,
            ResponseType = ResponseType.Default_TTS
          }
        );
      }
      return responses.ToArray();
    }

    protected override List<KeyValuePair<string, string>> ConstructExtraData()
    {
      var retVal = new List<KeyValuePair<string, string>>(1 + (replaceTable.Count * 2))
      {
        // Extra data is IgnoreCase, Reverse, DoTTS
        {
          "IgnoreCase", ignoreCase.ToString()
        },
        {
          "Reverse", reverse.ToString()
        },
        {
          "DoTTS", doTTS.ToString()
        }
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