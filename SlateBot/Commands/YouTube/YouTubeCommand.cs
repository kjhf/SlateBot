using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SlateBot.Errors;
using SlateBot.Imaging;
using SlateBot.Language;
using SlateBot.Utility;
using System;
using System.Collections.Generic;
using System.DrawingCore;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SlateBot.Commands.YouTube
{
  public class YouTubeCommand : Command
  {
    private static readonly Regex youtubeRegex = new Regex("(?:youtu\\.be\\/|v=)(?<id>[\\da-zA-Z\\-_]*)", RegexOptions.Compiled);
    private readonly PleaseWaitHandler waitHandler;
    private readonly IAsyncResponder asyncResponder;
    private readonly LanguageHandler languageHandler;
    private readonly YouTubeCommandType youTubeCommandType;
    private readonly IErrorLogger errorLogger;

    internal YouTubeCommand(IErrorLogger errorLogger, PleaseWaitHandler waitHandler, IAsyncResponder asyncResponder, LanguageHandler languageHandler, string[] aliases, string examples, string help, ModuleType module, YouTubeCommandType youTubeCommandType)
      : base(CommandHandlerType.YouTube, aliases, examples, help, module)
    {
      this.errorLogger = errorLogger;
      this.waitHandler = waitHandler;
      this.asyncResponder = asyncResponder;
      this.languageHandler = languageHandler;
      this.youTubeCommandType = youTubeCommandType;
    }

    public override IList<Response> Execute(SenderSettings senderDetail, IMessageDetail args)
    {
      Response response = new Response
      {
        Message = "https://www.youtube.com",
        ResponseType = ResponseType.Default
      };

      ServerSettings serverSettings = senderDetail.ServerSettings;
      CommandMessageHelper command = new CommandMessageHelper(serverSettings.CommandSymbol, args.Message);

      string keywords = command.CommandDetail;

      if (!string.IsNullOrWhiteSpace(keywords))
      {
        if (keywords.Length > 150)
        {
          response.Message = $"{Emojis.CrossSymbol} {languageHandler.GetPhrase(serverSettings.Language, "Error_IncorrectParameter")}: {languageHandler.GetPhrase(serverSettings.Language, "Text")} < 150";
        }
        else
        {
          // Already a YouTube URL? Extract the id and prepend it with youtube.com?v=
          var match = youtubeRegex.Match(keywords);
          if (match.Length > 1)
          {
            response.Message = $"https://www.youtube.com/watch?v={match.Groups["id"].Value}";
          }
          else
          {
            response = waitHandler.CreatePleaseWaitResponse(senderDetail.ServerSettings.Language);

            Task.Run(async () =>
            {
              string asyncRetVal;
              string query = $"https://www.googleapis.com/youtube/v3/search?" +
                                      $"part=snippet&maxResults=8" +
                                      $"&q={Uri.EscapeDataString(keywords)}" +
                                      $"&key={Tokens.GoogleAPIKey}";
              string json = await GetResponseStringAsync(query);
              dynamic data = JsonConvert.DeserializeObject(json);

              if (data.items.Count > 0)
              {
                // Attempt to get the first item that is a video.
                int itemIndex = 0;
                string videoId;

                do
                {
                  videoId = (data.items[itemIndex].id.videoId);
                  itemIndex++;
                }
                while (itemIndex < data.items.Count && string.IsNullOrWhiteSpace(videoId));

                if (string.IsNullOrWhiteSpace(videoId))
                {
                  asyncRetVal = ($"{Emojis.Warning} {languageHandler.GetPhrase(serverSettings.Language, "Error_NoResults")}");
                }
                else
                {
                  string youTubeURL = ("https://www.youtube.com/watch?v=" + videoId);
                  switch (youTubeCommandType)
                  {
                    case YouTubeCommandType.YouTube:
                      asyncRetVal = youTubeURL;
                      break;

                    case YouTubeCommandType.SaveFrom:
                      asyncRetVal = ("http://en.savefrom.net/#url=" + youTubeURL);
                      break;

                    case YouTubeCommandType.Repeat:
                      asyncRetVal = ("http://www.youtubeonrepeat.com/watch?v=" + videoId);
                      break;

                    default:
                      asyncRetVal = $"{Emojis.CrossSymbol} {languageHandler.GetPhrase(serverSettings.Language, "Error_Oops")}: {youTubeCommandType}";
                      break;
                  }
                }
              }
              else
              {
                asyncRetVal = $"{Emojis.Warning} {languageHandler.GetPhrase(serverSettings.Language, "Error_NoResults")}";
              }

              Response asyncResponse = new Response
              {
                Embed = (EmbedUtility.StringToEmbed(asyncRetVal)),
                Message = asyncRetVal,
                ResponseType = ResponseType.Default
              };

              await asyncResponder.SendResponseAsync(args, asyncResponse);
              waitHandler.PopPleaseWaitMessage();
            });
          }
        }
      }

      response.Embed = EmbedUtility.StringToEmbed(response.Message);
      return new[] { response };
    }

    public static async Task<string> GetResponseStringAsync(string url,
            IEnumerable<KeyValuePair<string, string>> headers = null)
    {
      if (string.IsNullOrWhiteSpace(url))
        throw new ArgumentNullException(nameof(url));

      using (var cl = new HttpClient())
      {
        cl.DefaultRequestHeaders.Clear();

        if (headers != null)
        {
          foreach (var header in headers)
          {
            cl.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
          }
        }
        return await cl.GetStringAsync(url).ConfigureAwait(false);
      }
    }

    protected override List<KeyValuePair<string, string>> ConstructExtraData()
    {
      var retVal = new List<KeyValuePair<string, string>>
      {
        { "YouTubeType", youTubeCommandType.ToString() }
      };

      return retVal;
    }
  }
}