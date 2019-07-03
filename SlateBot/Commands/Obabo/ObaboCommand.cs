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
using System.Threading.Tasks;

namespace SlateBot.Commands.Obabo
{
  public class ObaboCommand : Command
  {
    private readonly PleaseWaitHandler waitHandler;
    private readonly IAsyncResponder asyncResponder;
    private readonly LanguageHandler languageHandler;
    private readonly MirrorType mirrorType;
    private readonly IErrorLogger errorLogger;

    internal ObaboCommand(IErrorLogger errorLogger, PleaseWaitHandler waitHandler, IAsyncResponder asyncResponder, LanguageHandler languageHandler, string[] aliases, string examples, string help, ModuleType module, MirrorType mirrorType)
      : base(CommandHandlerType.Obabo, aliases, examples, help, module)
    {
      this.errorLogger = errorLogger;
      this.waitHandler = waitHandler;
      this.asyncResponder = asyncResponder;
      this.languageHandler = languageHandler;
      this.mirrorType = mirrorType;
    }

    public override IList<Response> Execute(SenderSettings senderDetail, IMessageDetail args)
    {
      Response response = new Response
      {
        Embed = EmbedUtility.StringToEmbed(Help),
        Message = Help,
        ResponseType = ResponseType.Default
      };

      ServerSettings serverSettings = senderDetail.ServerSettings;
      CommandMessageHelper command = new CommandMessageHelper(serverSettings.CommandSymbol, args.Message);

      string commandDetail = command.CommandDetail;

      if (args.URLs.Length > 0)
      {
        response = waitHandler.CreatePleaseWaitResponse(senderDetail.ServerSettings.Language);

        Task.Run(async () =>
        {
          Response asyncResponse = await BuildObaboImageAsync(senderDetail.ServerSettings.Language, args);
          if (asyncResponse == null)
          {
            string err = ($"{Emojis.NoEntry} {languageHandler.GetPhrase(senderDetail.ServerSettings.Language, "Error_NoImageMessages")}");
            asyncResponse = new Response
            {
              Embed = EmbedUtility.StringToEmbed(err),
              Message = err,
              ResponseType = ResponseType.Default
            };
          }
          await asyncResponder.SendResponseAsync(args, asyncResponse);
          waitHandler.PopPleaseWaitMessage();
        });
      }

      return new[] { response };
    }

    private async Task<Response> BuildObaboImageAsync(Languages language, IMessageDetail m)
    {
      string[] urls = m.URLs;
      Response response = null;
      for (int i = 0; i < urls.Length; i++)
      {
        string url = urls[i];
        try
        {
          // Check if the url is a file
          if (HTTPHelper.IsImageUrl(url))
          {
            // It is, download and perform obabo
            var tuple = await HTTPHelper.DownloadFile(url);
            if (tuple.Item2 != null)
            {
              response = DoBuildObaboImage(language, tuple.Item2);
            }
            else
            {
              // We failed, return a response indicating the failure.
              string err = Emojis.NoEntrySign + " " + tuple.Item1.ReasonPhrase;
              response = new Response
              {
                Embed = EmbedUtility.StringToEmbed(err),
                Message = err,
                ResponseType = ResponseType.Default
              };
            }
            break;
          }
        }
        catch (HttpRequestException ex)
        {
          errorLogger.LogDebug($"HttpRequestException exception downloading file: {url}. Assuming file too big.", true);
          errorLogger.LogException(ex, ErrorSeverity.Information);
          string err = ($"{Emojis.NoEntry} {languageHandler.GetPhrase(language, "Error_NotAFile")}");
          response = new Response
          {
            Embed = EmbedUtility.StringToEmbed(err),
            Message = err,
            ResponseType = ResponseType.Default
          };
          break;
        }
        catch (Exception ex)
        {
          errorLogger.LogDebug($"Exception downloading or handling Obabo file: {url}", true);
          errorLogger.LogException(ex, ErrorSeverity.Information);
          // try other urls
        }
      }
      return response;
    }

    private Response DoBuildObaboImage(Languages language, byte[] file)
    {
      Response response = new Response
      {
        ResponseType = ResponseType.Default
      };

      try
      {

        using (Image original = Image.FromStream(new MemoryStream(file)))
        {
          using (Bitmap bmp = new Bitmap(original))
          {
            ImageManipulator.PerformObabo(bmp, mirrorType);
            response.FilePath = Path.GetTempFileName() + ".png";
            bmp.Save(response.FilePath);
          }
        }
      }
      catch (OutOfMemoryException)
      {
        response.Message = ($"{Emojis.CrossSymbol} {languageHandler.GetPhrase(language, "Error_NotAFile")}");
      }
      catch (IOException iox)
      {
        // e.g. path error
        response.Message = ($"{Emojis.ExclamationSymbol} {iox.Message}");
        errorLogger.LogException(iox, ErrorSeverity.Information);
      }
      catch (Exception sysEx)
      {
        // everything else
        response.Message = ($"{Emojis.ExclamationSymbol} {sysEx.Message}");
        errorLogger.LogException(sysEx, ErrorSeverity.Error);
      }

      return response;
    }

    protected override List<KeyValuePair<string, string>> ConstructExtraData()
    {
      var retVal = new List<KeyValuePair<string, string>>
      {
        { "MirrorType", mirrorType.ToString() }
      };

      return retVal;
    }
  }
}