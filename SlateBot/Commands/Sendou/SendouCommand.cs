using CsHelper;
using Discord;
using SlateBot.Language;
using SplatTagCore;
using SplatTagDatabase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SlateBot.Commands.Sendou
{
  public class SendouCommand : Command
  {
    private static readonly SplatTagController splatTagController;
    private static readonly SplatTagJsonDatabase jsonDatabase;
    private static readonly MultiDatabase splatTagDatabase;
    private static readonly GenericFilesImporter multiSourceImporter;
    private static readonly string saveFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SplatTag");
    private static readonly Task initialiseTask;

    private readonly LanguageHandler languageHandler;
    private readonly IAsyncResponder asyncResponder;

    internal SendouCommand(LanguageHandler languageHandler, IAsyncResponder asyncResponder, string[] aliases, string examples, string help, ModuleType module)
      : base(CommandHandlerType.Sendou, aliases, examples, help, module)
    {
      this.languageHandler = languageHandler;
      this.asyncResponder = asyncResponder;
    }

    public override IList<Response> Execute(SenderSettings senderDetail, IMessageDetail args)
    {
      
      ServerSettings serverSettings = senderDetail.ServerSettings;
      CommandMessageHelper command = new CommandMessageHelper(serverSettings.CommandSymbol, args.Message);
      string query = command.CommandDetail;

      StringBuilder output = new StringBuilder();

      try
      {
        const string url = "https://wiki.teamfortress.com/wiki/Crafting";
        const string urlRaw = url + "?action=raw";

        if (cachedText == null)
        {
          cachedText = WebHelper.GetText(urlRaw);
        }

        string rawText = cachedText;

        // First we need to decide on what we're searching for.
        bool multiLookup =
          (!query.Contains("Fabricate", StringComparison.OrdinalIgnoreCase) &&
          !query.Contains("Smelt", StringComparison.OrdinalIgnoreCase) &&
          !query.Contains("Combine", StringComparison.OrdinalIgnoreCase));

        if (multiLookup)
        {
          query = "Fabricate " + query;
        }

        bool querySatisfied = rawText.ContainsIgnore(query, StringComparison.OrdinalIgnoreCase, " ", "_", "-", ".");

        if (!querySatisfied && multiLookup)
        {
          query = "Smelt " + query;
          querySatisfied = rawText.ContainsIgnore(query, StringComparison.OrdinalIgnoreCase, " ", "_", "-", ".");
        }

        if (!querySatisfied && multiLookup)
        {
          query = "Combine " + query;
          querySatisfied = rawText.ContainsIgnore(query, StringComparison.OrdinalIgnoreCase, " ", "_", "-", ".");
        }

        if (!querySatisfied)
        {
          string error = ($"{Emojis.ExclamationSymbol} {languageHandler.GetPhrase(senderDetail.ServerSettings.Language, "Error_UnknownRecipe")}");
          output.AppendLine(error + " " + query + ": " + url);
        }
        else
        {
          // We're in business.
          TextParser parser = new TextParser(rawText);
          parser.MovePast(query, true);
          parser.MovePast("|", true); // First parameter is blueprint name
          string recipeTemplate = parser.ExtractUntil("bprow", false, true);
          if (string.IsNullOrEmpty(recipeTemplate))
          {
            // Go to the end of the table instead.
            recipeTemplate = parser.ExtractUntil("</", false, true);
          }
          recipeTemplate = recipeTemplate.Replace("{{Item icon|", "").Replace("'''", "").Replace("''", "").Replace("{", "").Replace("}", "").Replace("\r", "").Replace("\n", "").Replace("\\r", "").Replace("\\n", "").Replace("  ", " ").Replace("<small>", " ").Replace("</small>", " ").Replace("=", "").Replace("Ã—", "x").Replace("â†’", "→").Trim('|', ' ');
          string[] templateArgs = recipeTemplate.Split('|');

          if (templateArgs.Length < 2)
          {
            string error = ($"{Emojis.CrossSymbol} {languageHandler.GetPhrase(senderDetail.ServerSettings.Language, "Error_Oops")}");
            output.Append(error).Append(' ').Append(query).Append(": ").AppendLine(url);
          }
          else
          {
            output.AppendLine("`Requirements: " + templateArgs[0].Trim() + "`");
            output.AppendLine("`Produces: " + templateArgs[1].Trim() + "`");
            for (int i = 2; i < templateArgs.Length; i++)
            {
              if (templateArgs[i].Contains("cost") || i == templateArgs.Length - 1)
              {
                string costStr = templateArgs[i].Replace("cost", "");
                if (costStr.Contains("</"))
                {
                  costStr = (costStr.Substring(0, costStr.IndexOf("</")));
                }
                output.AppendLine("`Cost: " + costStr.Trim() + " weapon units.`");
                break;
              }
              else
              {
                output.AppendLine("`" + templateArgs[i].Trim() + "`");
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        string error = ($"{Emojis.CrossSymbol} {languageHandler.GetPhrase(senderDetail.ServerSettings.Language, "Error_Oops")}");
        output.AppendLine(error + " " + query + " " + ex.Message);
      }

      return Response.CreateArrayFromString(output.ToString());
    }
  }
}