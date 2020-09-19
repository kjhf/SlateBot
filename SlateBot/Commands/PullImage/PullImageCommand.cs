using CsHelper;
using Newtonsoft.Json.Linq;
using SlateBot.Language;
using SlateBot.Utility;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SlateBot.Commands.PullImage
{
  public class PullImageCommand : Command
  {
    private readonly LanguageHandler languageHandler;
    private readonly IAsyncResponder asyncResponder;
    private readonly string url;
    private readonly string formattedResponseURL;
    private readonly string jsonProperty;

    internal PullImageCommand(LanguageHandler languageHandler, IAsyncResponder asyncResponder, string[] aliases, string examples, string help, ModuleType module, string url, string formattedResponseURL, string jsonProperty)
      : base(CommandHandlerType.PullImage, aliases, examples, help, module)
    {
      this.languageHandler = languageHandler;
      this.asyncResponder = asyncResponder;
      this.url = url;
      this.formattedResponseURL = formattedResponseURL;
      this.jsonProperty = jsonProperty;
    }

    public override IList<Response> Execute(SenderSettings senderDetail, IMessageDetail args)
    {
      // Responds asynchronously.
      Task.Run(async () =>
      {
        JContainer json;
        string message = null;

        try
        {
          json = (JContainer)await JSONHelper.GetJsonAsync(url);
        }
        catch (Exception)
        {
          json = null;
          message = ($"{Emojis.NoEntry} {languageHandler.GetPhrase(senderDetail.ServerSettings.Language, "Error_ServerNotFound")}");
        }

        string file = null;
        if (json != null)
        {
          file = formattedResponseURL.Replace("%file%", json[jsonProperty].ToString());
          message = file;
        }

        Response asyncResponse = new Response
        {
          ResponseType = ResponseType.Default,
          Embed = (file == null) ? null : EmbedUtility.ToEmbed(imageURL: file),
          Message = message
        };

        await asyncResponder.SendResponseAsync(args, asyncResponse);
      });

      // Return out the lifecycle with no response.
      return new[] { Response.WaitForAsync };
    }

    protected override List<KeyValuePair<string, string>> ConstructExtraData()
    {
      var retVal = new List<KeyValuePair<string, string>>
      {
        { "URL", url.ToString() },
        { "FormattedResponseURL", formattedResponseURL.ToString() },
        { "JSONProperty", jsonProperty.ToString() }
      };

      return retVal;
    }
  }
}