using SlateBot.Errors;
using SlateBot.Imaging;
using SlateBot.Language;
using SlateBot.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DrawingCore;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SlateBot.Commands.ASCII
{
  public class ASCIICommand : Command
  {
    private readonly PleaseWaitHandler waitHandler;
    private readonly IAsyncResponder asyncResponder;
    private readonly LanguageHandler languageHandler;
    private readonly IErrorLogger errorLogger;

    internal ASCIICommand(IErrorLogger errorLogger, PleaseWaitHandler waitHandler, IAsyncResponder asyncResponder, LanguageHandler languageHandler, string[] aliases, string examples, string help, ModuleType module)
      : base(CommandHandlerType.ASCII, aliases, examples, help, module)
    {
      this.errorLogger = errorLogger;
      this.waitHandler = waitHandler;
      this.asyncResponder = asyncResponder;
      this.languageHandler = languageHandler;
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
          Response asyncResponse = await BuildAsciiImageAsync(senderDetail.ServerSettings.Language, args);
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

    private async Task<Response> BuildAsciiImageAsync(Languages language, IMessageDetail m)
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
              response = DoBuildAsciiImage(language, tuple.Item2);
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
        catch (Exception ex)
        {
          errorLogger.LogDebug($"URL exception downloading file: {url}", true);
          errorLogger.LogException(ex, ErrorSeverity.Information);
        }
      }
      return response;
    }

    private Response DoBuildAsciiImage(Languages language, byte[] file)
    {
      Response response = new Response
      {
        ResponseType = ResponseType.Default
      };

      try
      {
        const int MaximumNumberOfCharacters = Constants.MessageLimit;
        const int MaximumWidth = 130;
        const int MaximumHeight = 60;

        using (Image original = Image.FromStream(new MemoryStream(file)))
        {
          using (Bitmap bmp = new Bitmap(original))
          {
            float width = original.Width;
            float height = original.Height;

            // width + 1 for new line
            while (((((int)width + 1) * (int)height) > MaximumNumberOfCharacters) || (width > MaximumWidth) || (height > MaximumHeight))
            {
              width *= 0.9995f;
              height *= 0.9995f;

              if (width < 1)
              {
                width = 1;
              }
              if (height < 1)
              {
                height = 1;
              }
            }

            using (Bitmap newCanvas = new Bitmap((int)width, (int)height))
            {
              using (Graphics g = Graphics.FromImage(newCanvas))
              {
                g.Clear(Color.White);
                g.DrawImage(original, 0, 0, (int)width, (int)height);
                g.Save();
              }
              response.Message = ($"```{ImageManipulator.GreyscaleImageToASCII(newCanvas)}```");
            }
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

  }
}