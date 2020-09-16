using CsHelper;
using Newtonsoft.Json.Linq;
using SlateBot.Errors;
using SlateBot.Language;
using SlateBot.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SlateBot.Commands.Translate
{
  public class TranslateCommand : Command
  {
    private readonly IAsyncResponder asyncResponder;
    private readonly IErrorLogger errorLogger;
    private readonly LanguageHandler languageHandler;
    private const string TRANSLATE_HOMEPAGE = "https://translate.google.com";

    private static readonly HashSet<string> translateCodes = new HashSet<string>() { "af", "ga", "sq", "it", "ar", "ja", "az", "kn", "eu", "ko", "bn", "la", "be", "lv", "bg", "lt", "ca", "mk", "zh-CN", "ms", "zh-TW", "mt", "hr", "no", "cs", "fa", "da", "pl", "nl", "pt", "en", "ro", "eo", "ru", "et", "sr", "tl", "sk", "fi", "sl", "fr", "es", "gl", "sw", "ka", "sv", "de", "ta", "el", "te", "gu", "th", "ht", "tr", "iw", "uk", "hi", "ur", "hu", "vi", "is", "cy", "id", "yi" };

    internal TranslateCommand(IAsyncResponder asyncResponder, IErrorLogger errorLogger, LanguageHandler languageHandler, string[] aliases, string examples, string help, ModuleType module)
      : base(CommandHandlerType.Translate, aliases, examples, help, module, requiresSymbol: true, noSetAlias: true)
    {
      this.asyncResponder = asyncResponder;
      this.errorLogger = errorLogger;
      this.languageHandler = languageHandler;
    }

    public override IList<Response> Execute(SenderSettings senderDetail, IMessageDetail args)
    {
      ServerSettings serverSettings = senderDetail.ServerSettings;
      CommandMessageHelper command = new CommandMessageHelper(serverSettings.CommandSymbol, args.Message);

      // Verify command.
      string[] splitCommand = command.CommandLower.Split('>', StringSplitOptions.RemoveEmptyEntries);
      if (splitCommand.Length == 2)
      {
        var sourceLanguage = splitCommand[0];
        if (!translateCodes.Contains(sourceLanguage))
        {
          if (string.Equals(sourceLanguage, "?") || sourceLanguage.Equals("auto", StringComparison.OrdinalIgnoreCase))
          {
            sourceLanguage = "auto";
          }
          else
          {
            // If the source language is not found, then assume the command is not for us.
            return Response.NoResponse;
          }
        }

        // Command verified, continue with the translation.
        var targetLanguage = splitCommand[1];
        var toTranslate = command.CommandDetail;

        if (string.IsNullOrWhiteSpace(toTranslate))
        {
          return Response.CreateArrayFromString(TRANSLATE_HOMEPAGE);
        }
        else if (!translateCodes.Contains(targetLanguage))
        {
          string err = ($"{Emojis.CrossSymbol} {languageHandler.GetPhrase(senderDetail.ServerSettings.Language, "Error_IncorrectParameter")}: {targetLanguage}");
          return Response.CreateArrayFromString(err);
        }

        string query = Uri.EscapeDataString(toTranslate);
        string request =
          "https://translate.googleapis.com/translate_a/single?client=gtx&dt=t"
          + $"&ie=UTF-8"
          + $"&oe=UTF-8"
          + $"&sl={sourceLanguage}"
          + $"&tl={targetLanguage}"
          + $"&q={query}";
        errorLogger.LogDebug(request, false);

        // Responds asynchronously.
        Task.Run(async () =>
        {
          JContainer json;
          string message = null;

          try
          {
            json = (JContainer)await JSONHelper.GetJsonAsync(request).ConfigureAwait(false);
          }
          catch (Exception)
          {
            json = null;
            message = ($"{Emojis.NoEntry} {languageHandler.GetPhrase(senderDetail.ServerSettings.Language, "Error_ServerNotFound")}");
          }

          try
          {
            dynamic outer = json[0];
            if (outer.Count > 0)
            {
              StringBuilder translation = new StringBuilder();
              for (int i = 0; i < outer.Count; i++)
              {
                string translatedLine = outer[i][0]?.ToString();
                if (translatedLine != null)
                {
                  translation.AppendLine(translatedLine);
                }
              }

              if (translation.Length <= 0)
              {
                message = ($"{Emojis.Warning} {languageHandler.GetPhrase(senderDetail.ServerSettings.Language, "Error_NoResults")}");
              }
              else
              {
                message = translation.ToString();
              }
            }
            else
            {
              message = ($"{Emojis.Warning} {languageHandler.GetPhrase(senderDetail.ServerSettings.Language, "Error_NoResults")}");
            }
          }
          catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException ex)
          {
            // Extra information for debugging.
            errorLogger.LogException(ex, ErrorSeverity.Error);
            errorLogger.LogDebug("Unable to process the translation response: " + ex, true);
            errorLogger.LogDebug("Request: " + request, true);
            errorLogger.LogDebug("Response: " + json, true);

            // Let the usual error handling kick in.
            throw;
          }

          Response asyncResponse = Response.CreateFromString(message);
          await asyncResponder.SendResponseAsync(args, asyncResponse).ConfigureAwait(false);

        });

        // Return out the lifecycle with no response.
        return new[] { Response.WaitForAsync };
      }
      else
      {
        return Response.NoResponse;
      }
    }
  }
}