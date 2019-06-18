using SlateBot.Language;
using SlateBot.Utility;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlateBot.Commands.ResponseMessageList
{
  public class ResponseMessageListCommand : Command
  {
    public readonly bool requiresSymbol;
    protected readonly string choiceFormat;
    protected readonly string[][] choices;
    private readonly ResponseType responseType = ResponseType.Default;

    /// <summary>
    /// Respond to the message with a random choice.
    /// </summary>
    /// <param name="aliases"></param>
    /// <param name="choices"></param>
    /// <param name="help"></param>
    public ResponseMessageListCommand(IEnumerable<string> aliases, string[][] choices, string choiceFormat, string examples, string help, ModuleType module, ResponseType responseType, bool requiresSymbol = true)
      : base(CommandHandlerType.ResponseMessageList, aliases?.ToArray(), examples, help, module)
    {
      this.choices = choices;
      this.choiceFormat = choiceFormat;
      this.responseType = responseType;
      this.requiresSymbol = requiresSymbol;
    }

    public override IList<Response> Execute(SenderSettings senderDetail, IMessageDetail args)
    {
      ServerSettings serverSettings = senderDetail.ServerSettings;
      CommandMessageHelper command = new CommandMessageHelper(serverSettings.CommandSymbol, args.Message);
      StringBuilder choice = new StringBuilder(choiceFormat);

      for (int i = 0; i < choices.Length; i++)
      {
        choice.Replace($"{{{i}}}", choices[i][rand.Next(choices[i].Length)]);
      }

      string result = VariableStrings.Replace(choice, args.Username, args.UserId.ToString(), args.GuildName, command.CommandDetail).ToString();

      Response response = new Response
      {
        embed = null,
        message = result,
        responseType = responseType
      };
      return new[] { response };
    }

    protected override List<KeyValuePair<string, string>> ConstructExtraData()
    {
      var retVal = new List<KeyValuePair<string, string>>(3 + (choices.Length * 2)) // Assume two choices per list to begin with
      {
        // Extra data is ResponseType
        { "ResponseType", responseType.ToString() },

        // And RequiresSymbol
        { "RequiresSymbol", requiresSymbol.ToString() },

        // And ChoiceFormat
        { "ChoiceFormat", choiceFormat }
      };

      // And the choices
      for (int i = 0; i < choices.Length; i++)
      {
        string section = ((char)('A' + i)).ToString();
        for (int j = 0; j < choices[i].Length; j++)
        {
          retVal.Add(section, choices[i][j]);
        }
      }

      return retVal;
    }
  }
}