using CsHelper;
using System.Collections.Generic;
using System.Linq;

namespace SlateBot.Commands.ResponseMessage
{
  public class ResponseMessageCommand : Command
  {
    private readonly string[] choices;
    private readonly ResponseType responseType;

    /// <summary>
    /// Respond to the message with a random choice.
    /// </summary>
    /// <param name="aliases"></param>
    /// <param name="choices"></param>
    /// <param name="help"></param>
    public ResponseMessageCommand(IEnumerable<string> aliases, IEnumerable<string> choices, string examples, string help, ModuleType module, ResponseType responseType, bool requiresSymbol)
      : base(CommandHandlerType.ResponseMessage, aliases?.ToArray(), examples, help, module, requiresSymbol)
    {
      this.choices = choices?.ToArray();
      this.responseType = responseType;
    }

    public override IList<Response> Execute(SenderSettings senderDetail, IMessageDetail args)
    {
      ServerSettings serverSettings = senderDetail.ServerSettings;
      CommandMessageHelper command = new CommandMessageHelper(serverSettings.CommandSymbol, args.Message);

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
        { "ResponseType", responseType.ToString() }
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