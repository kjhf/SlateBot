using SlateBot.Utility;
using System.Collections.Generic;
using System.Linq;

namespace SlateBot.Commands.ResponseMessage
{
  public class ResponseMessageCommand : Command
  {
    public readonly bool requiresSymbol;
    protected readonly string[] choices;
    private readonly Language.LanguageHandler languageHandler;
    private string[] aliases = new string[0];
    private string examples = "";
    private string help = "The bot responds with a random line.";
    private ModuleType module = ModuleType.General;
    private ResponseType responseType = ResponseType.Default;

    /// <summary>
    /// Respond to the message with a random choice.
    /// Of course if there's one choice then it will only use that one...
    /// </summary>
    /// <param name="aliases"></param>
    /// <param name="choice"></param>
    /// <param name="help"></param>
    /// <param name="requiresSymbol"</param>
    public ResponseMessageCommand(Language.LanguageHandler languageHandler, IEnumerable<string> aliases, string choice, string examples, string help, ModuleType module, ResponseType responseType, bool requiresSymbol = true)
      : this(languageHandler, aliases, new string[] { choice }, examples, help, module, responseType, requiresSymbol)
    {
    }

    /// <summary>
    /// Respond to the message with a random choice.
    /// </summary>
    /// <param name="aliases"></param>
    /// <param name="choices"></param>
    /// <param name="help"></param>
    public ResponseMessageCommand(Language.LanguageHandler languageHandler, IEnumerable<string> aliases, IEnumerable<string> choices, string examples, string help, ModuleType module, ResponseType responseType, bool requiresSymbol = true)
    {
      this.languageHandler = languageHandler;
      this.aliases = aliases?.ToArray();
      this.choices = choices?.ToArray();
      this.examples = examples;
      this.help = help;
      this.module = module;
      this.responseType = responseType;
      this.requiresSymbol = requiresSymbol;
    }

    public override string[] Aliases => aliases;
    public override CommandHandlerType CommandHandlerType => CommandHandlerType.ResponseMessage;
    public override string Examples => examples;
    public override List<KeyValuePair<string, string>> ExtraData => ConstructExtraData();
    public override string Help => help;
    public override ModuleType Module => module;

    public override IList<Response> Execute(SenderSettings senderDetail, IMessageDetail args)
    {
      ServerSettings serverSettings = senderDetail.ServerSettings;
      CommandMessageHelper command = new CommandMessageHelper(serverSettings.CommandSymbol, args.Message);
      string[] commandParams = command.CommandParams;

      string choice = choices[rand.Next(choices.Length)];
      string result = VariableStrings.Replace(choice, args.Username, args.UserId.ToString(), args.GuildName, command.CommandDetail);

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

      // And the choices
      for (int i = 0; i < choices.Length; i++)
      {
        retVal.Add("C" + i, choices[i]);
      }

      return retVal;
    }
  }
}