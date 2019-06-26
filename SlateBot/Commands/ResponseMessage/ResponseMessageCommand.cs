using SlateBot.Language;
using SlateBot.Utility;
using System.Collections.Generic;
using System.Linq;

namespace SlateBot.Commands.ResponseMessage
{
  public class ResponseMessageCommand : Command
  {
    public readonly bool requiresSymbol;
    private readonly string[] choices;
    private readonly ResponseType responseType;

    /// <summary>
    /// Respond to the message with a random choice.
    /// Of course if there's one choice then it will only use that one...
    /// </summary>
    /// <param name="aliases"></param>
    /// <param name="choice"></param>
    /// <param name="help"></param>
    /// <param name="requiresSymbol"</param>
    public ResponseMessageCommand(IEnumerable<string> aliases, string choice, string examples, string help, ModuleType module, ResponseType responseType, bool requiresSymbol = true)
      : this(aliases, new string[] { choice }, examples, help, module, responseType, requiresSymbol)
    {
    }

    /// <summary>
    /// Respond to the message with a random choice.
    /// </summary>
    /// <param name="aliases"></param>
    /// <param name="choices"></param>
    /// <param name="help"></param>
    public ResponseMessageCommand(IEnumerable<string> aliases, IEnumerable<string> choices, string examples, string help, ModuleType module, ResponseType responseType, bool requiresSymbol = true)
      : base(CommandHandlerType.ResponseMessage, aliases?.ToArray(), examples, help, module)
    {
      this.choices = choices?.ToArray();
      this.responseType = responseType;
      this.requiresSymbol = requiresSymbol;
    }

    public override IList<Response> Execute(SenderSettings senderDetail, IMessageDetail args)
    {
      ServerSettings serverSettings = senderDetail.ServerSettings;
      CommandMessageHelper command = new CommandMessageHelper(serverSettings.CommandSymbol, args.Message);
      string[] commandParams = command.CommandParams;

      string choice = choices[rand.Next(choices.Length)];
      string result = VariableStrings.Replace(choice, args.Username, args.UserId.ToString(), args.GuildName, command.CommandDetail);

      Response response = new Response
      {
        Embed = null,
        Message = result,
        ResponseType = responseType
      };
      return new[] { response };
    }

    protected override List<KeyValuePair<string, string>> ConstructExtraData()
    {
      var retVal = new List<KeyValuePair<string, string>>(2 + choices.Length)
      {
        // Extra data is ResponseType
        { "ResponseType", responseType.ToString() },

        // And RequiresSymbol
        { "RequiresSymbol", requiresSymbol.ToString() }
      };

      // And the choices
      for (int i = 0; i < choices.Length; i++)
      {
        retVal.Add("C" + i, choices[i]);
      }

      return retVal;
    }
  }
}