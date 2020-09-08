using CsHelper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SlateBot.Language;
using SlateBot.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlateBot.Commands.SplatBuilds
{
  public class SplatBuildsCommand : Command
  {
    private readonly LanguageHandler languageHandler;
    private readonly IAsyncResponder asyncResponder;
    private const string url = "https://sendou.ink/graphql";
    private const string origin = "https://sendou.ink";

    internal SplatBuildsCommand(LanguageHandler languageHandler, IAsyncResponder asyncResponder, string[] aliases, string examples, string help, ModuleType module)
      : base(CommandHandlerType.SplatBuilds, aliases, examples, help, module)
    {
      this.languageHandler = languageHandler;
      this.asyncResponder = asyncResponder;
    }

    public override IList<Response> Execute(SenderSettings senderDetail, IMessageDetail args)
    {
      ServerSettings serverSettings = senderDetail.ServerSettings;
      CommandMessageHelper command = new CommandMessageHelper(serverSettings.CommandSymbol, args.Message);
      string query = command.CommandDetail;

      // Responds asynchronously.
      Task.Run(async () =>
      {
        JContainer json;
        string message = null;

        string weapon = Splatoon.SplatoonDefs.TryFindWeapon(query);
        if (weapon != null)
        {

          try
          {
            string buildsQuery = $@"query {{
  searchForBuilds(weapon: ""{weapon}"")
  {{
    headgear
    clothing
    shoes
  }}  
}}";
            string builtUrl = $"{url}?query={Uri.EscapeUriString(buildsQuery)}";
            json = (JContainer)JsonConvert.DeserializeObject(await RequestsHelper.CurlGetCommand(builtUrl, origin).ConfigureAwait(false));
          }
          catch (Exception ex)
          {
            json = null;
            message = ($"{Emojis.NoEntry} {languageHandler.GetPhrase(senderDetail.ServerSettings.Language, "Error_ServerNotFound")}");
            Console.WriteLine(ex);
          }

          try
          {
            dynamic jsonResult = json["data"]["searchForBuilds"];
            int count = 0;
            foreach (var node in jsonResult)
            {
              JArray headgear = node["headgear"];
              JArray clothing = node["clothing"];
              JArray shoes = node["shoes"];
              string[] mains = new string[] { headgear[0].ToString(), clothing[0].ToString(), shoes[0].ToString() };
              List<string> subs = new List<string>();
              subs.AddRange(headgear.Skip<JToken>(1).Values<string>());
              subs.AddRange(clothing.Skip<JToken>(1).Values<string>());
              subs.AddRange(shoes.Skip<JToken>(1).Values<string>());
              var mainsDict = new Dictionary<string, int>(mains.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count()).OrderByDescending(pair => pair.Value));
              var subsDict = new Dictionary<string, int>(subs.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count()).OrderByDescending(pair => pair.Value));
              StringBuilder sb = new StringBuilder();
              foreach (var pair in mainsDict)
              {
                sb.Append(pair.Value).Append("m");
                if (subsDict.ContainsKey(pair.Key))
                {
                  sb.Append(pair.Value).Append("s");
                  subsDict.Remove(pair.Key);
                }
                sb.Append(" ").Append(pair.Key).Append(", ");
              }
              foreach (var pair in subsDict)
              {
                sb.Append(pair.Value).Append("s");
                sb.Append(" ").Append(pair.Key).Append(", ");
              }
              sb.AppendLine();
              message += sb.ToString();
              count++;
              if (count > 6) break;
            }
          }
          catch (Exception ex)
          {
            message = ($"{Emojis.NoEntry} {languageHandler.GetPhrase(senderDetail.ServerSettings.Language, "Error_Oops")}");
            Console.WriteLine(ex);
          }
        }
        else
        {
          message = ($"{Emojis.QuestionSymbol} {languageHandler.GetPhrase(senderDetail.ServerSettings.Language, "Error_NoResults")}");
        }

        Response asyncResponse = new Response
        {
          ResponseType = ResponseType.Default,
          Embed = EmbedUtility.StringToEmbed(message, null),
          Message = message
        };

        await asyncResponder.SendResponseAsync(args, asyncResponse);
      });

      // Return out the lifecycle with no response.
      return new[] { Response.WaitForAsync };
    }
  }
}