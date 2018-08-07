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
    private readonly Language.LanguageHandler languageHandler;
    private string[] aliases = new string[0];
    private string examples = "";
    private string help = "The bot responds with a random line.";
    private ModuleType module = ModuleType.General;
    private ResponseType responseType = ResponseType.Default;

    /// <summary>
    /// Respond to the message with a random choice.
    /// </summary>
    /// <param name="aliases"></param>
    /// <param name="choices"></param>
    /// <param name="help"></param>
    public ResponseMessageListCommand(Language.LanguageHandler languageHandler, IEnumerable<string> aliases, string[][] choices, string choiceFormat, string examples, string help, ModuleType module, ResponseType responseType, bool requiresSymbol = true)
    {
      this.languageHandler = languageHandler;
      this.aliases = aliases?.ToArray();
      this.choices = choices;
      this.choiceFormat = choiceFormat;
      this.examples = examples;
      this.help = help;
      this.module = module;
      this.responseType = responseType;
      this.requiresSymbol = requiresSymbol;
    }

    public override string[] Aliases => aliases;
    public override CommandHandlerType CommandHandlerType => CommandHandlerType.ResponseMessageList;
    public override string Examples => examples;
    public override List<KeyValuePair<string, string>> ExtraData => ConstructExtraData();
    public override string Help => help;
    public override ModuleType Module => module;

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
        command = this,
        embed = null,
        message = result,
        responseType = responseType
      };
      return new[] { response };
    }

    private List<KeyValuePair<string, string>> ConstructExtraData()
    {
      var retVal = new List<KeyValuePair<string, string>>();

      // Extra data is ResponseType
      retVal.Add("ResponseType", responseType.ToString());

      // And RequiresSymbol
      retVal.Add("RequiresSymbol", requiresSymbol.ToString());

      // And ChoiceFormat
      retVal.Add("ChoiceFormat", choiceFormat);

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