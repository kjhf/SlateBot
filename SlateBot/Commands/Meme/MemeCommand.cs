using CsHelper;
using SlateBot.Errors;
using SlateBot.Imaging;
using SlateBot.Language;
using SlateBot.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DrawingCore;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace SlateBot.Commands.Meme
{
  public class MemeCommand : Command
  {
    private readonly PleaseWaitHandler waitHandler;
    private readonly IAsyncResponder asyncResponder;
    private readonly LanguageHandler languageHandler;
    private readonly IErrorLogger errorLogger;
    private readonly string templatePath;
    private readonly Point topLeft, topRight, bottomLeft;

    internal MemeCommand(IErrorLogger errorLogger, PleaseWaitHandler waitHandler, IAsyncResponder asyncResponder, LanguageHandler languageHandler, string[] aliases, string examples, string help, ModuleType module, string templatePath)
      : base(CommandHandlerType.Meme, aliases, examples, help, module)
    {
      this.errorLogger = errorLogger;
      this.waitHandler = waitHandler;
      this.asyncResponder = asyncResponder;
      this.languageHandler = languageHandler;
      this.templatePath = templatePath;

      // Calculate the points.
      // CalculatePoints();
      int lowX = int.MaxValue, lowY = int.MaxValue;
      int highX = int.MinValue, highY = int.MinValue;

      using (Bitmap template = new Bitmap(templatePath))
      {
        for (int x = 0; x < template.Width; x++)
        {
          for (int y = 0; y < template.Height; y++)
          {
            if (template.GetPixel(x, y).A == 0)
            {
              if (x < lowX)
              {
                lowX = x;
              }
              if (x > highX)
              {
                highX = x;
              }
              if (y < lowY)
              {
                lowY = y;
              }
              if (y > highY)
              {
                highY = y;
              }
            }
          }
        }
      }

      this.topLeft = new Point(lowX, lowY);
      this.topRight = new Point(highX, lowY);
      this.bottomLeft = new Point(lowX, highY);

      Debug.Assert(topLeft.Y < bottomLeft.Y && topRight.Y < bottomLeft.Y && bottomLeft.X < topRight.X && topLeft.X < topRight.X);
    }

    internal MemeCommand(IErrorLogger errorLogger, PleaseWaitHandler waitHandler, IAsyncResponder asyncResponder, LanguageHandler languageHandler, string[] aliases, string examples, string help, ModuleType module, string template, Point topLeft, Point topRight, Point bottomLeft)
      : this(errorLogger, waitHandler, asyncResponder, languageHandler, aliases, examples, help, module, template)
    {
      this.topLeft = topLeft;
      this.topRight = topRight;
      this.bottomLeft = bottomLeft;
      Debug.Assert(topLeft.Y < bottomLeft.Y && topRight.Y < bottomLeft.Y && bottomLeft.X < topRight.X && topLeft.X < topRight.X);
    }

    public override IList<Response> Execute(SenderSettings senderDetail, IMessageDetail args)
    {
      Response response = new Response
      {
        Embed = EmbedUtility.ToEmbed(Help),
        Message = Help,
        ResponseType = ResponseType.Default
      };

      ServerSettings serverSettings = senderDetail.ServerSettings;
      CommandMessageHelper command = new CommandMessageHelper(serverSettings.CommandSymbol, args.Message);

      string commandDetail = command.CommandDetail;

      response = waitHandler.CreatePleaseWaitResponse(senderDetail.ServerSettings.Language);

      Task.Run(async () =>
      {
        Response asyncResponse = await BuildMemeAsync(senderDetail.ServerSettings.Language, args);
        if (asyncResponse == null)
        {
          string err = ($"{Emojis.NoEntry} {languageHandler.GetPhrase(senderDetail.ServerSettings.Language, "Error_NoImageMessages")}");
          asyncResponse = new Response
          {
            Embed = EmbedUtility.ToEmbed(err),
            Message = err,
            ResponseType = ResponseType.Default
          };
        }
        await asyncResponder.SendResponseAsync(args, asyncResponse);
        waitHandler.PopPleaseWaitMessage();
      });

      return new[] { response };
    }

    private async Task<Response> BuildMemeAsync(Languages language, IMessageDetail m)
    {
      string[] urls = m.URLs;
      Response response = null;
      for (int i = 0; i < urls.Length; i++)
      {
        string url = urls[i];
        try
        {
          // Check if the url is a file
          if (WebHelper.IsImageUrl(url))
          {
            // It is, download and perform meme
            var tuple = await WebHelper.DownloadFile(url);
            if (tuple.Item2 != null)
            {
              response = DoBuildMemeImage(language, tuple.Item2);
            }
            else
            {
              // We failed, return a response indicating the failure.
              string err = Emojis.NoEntrySign + " " + tuple.Item1.ReasonPhrase;
              response = new Response
              {
                Embed = EmbedUtility.ToEmbed(err),
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
            Embed = EmbedUtility.ToEmbed(err),
            Message = err,
            ResponseType = ResponseType.Default
          };
          break;
        }
        catch (Exception ex)
        {
          errorLogger.LogDebug($"Exception downloading or handling meme file: {url}", true);
          errorLogger.LogException(ex, ErrorSeverity.Information);
          // try other urls
        }
      }
      return response;
    }

    private Response DoBuildMemeImage(Languages language, byte[] file)
    {
      Response response = new Response
      {
        ResponseType = ResponseType.Default
      };

      try
      {
        using (Image i = Image.FromStream(new MemoryStream(file)))
        {
          using (Bitmap original = new Bitmap(i))
          {
            Point[] destinationPoints = { topLeft, topRight, bottomLeft };
            response.FilePath = ImageManipulator.PerformMeme(original, templatePath, destinationPoints);
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
        { "Template", templatePath },
        { "TopLeft", $"{topLeft.X},{topLeft.Y}" },
        { "TopRight", $"{topRight.X},{topRight.Y}" },
        { "BottomLeft", $"{bottomLeft.X},{bottomLeft.Y}" },
      };

      return retVal;
    }
  }
}