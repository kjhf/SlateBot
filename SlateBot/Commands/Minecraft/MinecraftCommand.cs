using CsHelper;
using SlateBot.Errors;
using SlateBot.Language;
using SlateBot.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SlateBot.Commands.Minecraft
{
  public class MinecraftCommand : Command
  {
    private readonly IErrorLogger errorLogger;
    private readonly LanguageHandler languageHandler;
    private readonly IAsyncResponder asyncResponder;

    private const string RedirectString = "#REDIRECT";
    private const string CraftingString = "{{crafting";
    private const string OutputNameString = "Output=";

    internal MinecraftCommand(IErrorLogger errorLogger, LanguageHandler languageHandler, IAsyncResponder asyncResponder, string[] aliases, string examples, string help, ModuleType module)
      : base(CommandHandlerType.Minecraft, aliases, examples, help, module)
    {
      this.errorLogger = errorLogger;
      this.languageHandler = languageHandler;
      this.asyncResponder = asyncResponder;
    }

    public override IList<Response> Execute(SenderSettings senderDetail, IMessageDetail args)
    {
      // Responds asynchronously.
      Task.Run(async () =>
      {
        CommandMessageHelper command = new CommandMessageHelper(senderDetail.ServerSettings.CommandSymbol, args.Message);
        string manualUrl = "http://minecraft.gamepedia.com/index.php?search=" + command.EscapedCommandDetail + "&title=Special%3ASearch&go=Go";
        StringBuilder output = new StringBuilder(manualUrl + " \n");
        string[] craftingTemplateParams = new string[] { "\n", "|" };

        string query = command.EscapedCommandDetail;
        string json = "";

        try
        {
          bool redirectResolved = true;

          // Resolve redirects
          do
          {
            json = await WebHelper.GetTextAsync($"http://minecraft.gamepedia.com/api.php?format=json&action=query&titles={query}&prop=revisions&rvprop=content&callback=?");

            if (json.Contains(RedirectString))
            {
              // Resolve redirect
              TextParser parser = new TextParser(json);
              parser.MovePast(RedirectString, true);
              parser.MovePast("[[", true);
              string article = parser.ExtractUntil("]]", false, true);
              string newQuery = Uri.EscapeDataString(article);
              if (query == newQuery)
              {
                output.AppendLine(Emojis.ExclamationSymbol + " infinite recursion error on: " + query);
                redirectResolved = false;
                break;
              }
              else
              {
                query = newQuery;
              }
            }
          }
          while (json.Contains(RedirectString));

          if (redirectResolved)
          {
            string url = "http://minecraft.gamepedia.com/" + query;

            // Get the recipe.
            if (!json.ContainsIgnore(CraftingString, StringComparison.OrdinalIgnoreCase, ' ', '\n', '\r'))
            {
              string error = Emojis.InfoSymbol + " " + languageHandler.GetPhrase(senderDetail.ServerSettings.Language, "Error_UnknownRecipe");
              output.AppendLine(error + " " + command.CommandDetail + ": " + url);
            }
            else
            {
              // We're in business.
              TextParser parser = new TextParser(json);
              parser.MovePast(CraftingString, true);
              string recipeTemplate = parser.ExtractUntil("}}", false, true).Replace("|head=0", "").Replace("|head=1", "").Replace("|showdescription=0", "").Replace("|showdescription=1", "").Replace("|showname=0", "").Replace("|showname=1", "");
              parser = new TextParser(recipeTemplate);
              parser.MovePast("|", true); // First parameter is template name

              string[,] recipe = new string[3, 3];
              // Initialise all elements with an empty string.
              for (int x = 0; x < 3; x++)
              {
                for (int y = 0; y < 3; y++)
                {
                  recipe[x, y] = "";
                }
              }

              if (recipeTemplate.Contains("A2") || recipeTemplate.Contains("B2") || recipeTemplate.Contains("C2"))
              {
                // Method 1, cells specified
                string a1Cell = parser.ExtractBetween("A1", craftingTemplateParams, true);
                recipe[0, 0] = TidyRecipeParameter(a1Cell);

                string b1Cell = parser.ExtractBetween("B1", craftingTemplateParams, true);
                recipe[1, 0] = TidyRecipeParameter(b1Cell);

                string c1Cell = parser.ExtractBetween("C1", craftingTemplateParams, true);
                recipe[2, 0] = TidyRecipeParameter(c1Cell);

                string a2Cell = parser.ExtractBetween("A2", craftingTemplateParams, true);
                recipe[0, 1] = TidyRecipeParameter(a2Cell);

                string b2Cell = parser.ExtractBetween("B2", craftingTemplateParams, true);
                recipe[1, 1] = TidyRecipeParameter(b2Cell);

                string c2Cell = parser.ExtractBetween("C2", craftingTemplateParams, true);
                recipe[2, 1] = TidyRecipeParameter(c2Cell);

                string a3Cell = parser.ExtractBetween("A3", craftingTemplateParams, true);
                recipe[0, 2] = TidyRecipeParameter(a3Cell);

                string b3Cell = parser.ExtractBetween("B3", craftingTemplateParams, true);
                recipe[1, 2] = TidyRecipeParameter(b3Cell);

                string c3Cell = parser.ExtractBetween("C3", craftingTemplateParams, true);
                recipe[2, 2] = TidyRecipeParameter(c3Cell);
              }
              else
              {
                // Method 2, all fields present -OR- clockwise layout.
                List<string> items = new List<string>();
                for (int i = 0; i < 9; i++)
                {
                  string candidateItem = TidyRecipeParameter(parser.ExtractUntil(craftingTemplateParams, false, true));
                  if (candidateItem.Contains("output", StringComparison.OrdinalIgnoreCase))
                  {
                    break;
                  }
                  else
                  {
                    items.Add(candidateItem);
                    parser.MovePast("|");
                  }
                }

                // If we have less than 4, then it's constructed in a clockwise fashion.
                switch (items.Count)
                {
                  case 0:
                    break;

                  case 1:
                    recipe[0, 1] = items[0];
                    break;

                  case 2:
                    recipe[0, 1] = items[0];
                    recipe[1, 1] = items[1];
                    break;

                  case 3:
                    recipe[0, 1] = items[0];
                    recipe[1, 1] = items[1];
                    recipe[1, 2] = items[2];
                    break;

                  case 4:
                    recipe[0, 1] = items[0];
                    recipe[1, 1] = items[1];
                    recipe[1, 2] = items[2];
                    recipe[0, 2] = items[3];
                    break;

                  default:
                  {
                    recipe[0, 1] = items[0];
                    recipe[1, 1] = items[1];
                    recipe[1, 2] = items[2];
                    recipe[0, 2] = items[3];
                    break;
                  }
                  case 9:
                    // Otherwise, all parameters are specified and just go from top left to bottom right.
                    for (int x = 0; x < 3; x++)
                    {
                      for (int y = 0; y < 3; y++)
                      {
                        recipe[x, y] = items[(x * 3) + y];
                      }
                    }
                    break;
                }
              }

              parser.MovePast(OutputNameString, true);
              string outputName = TidyRecipeParameter(parser.ExtractUntil(craftingTemplateParams, false, true));
              if (string.IsNullOrWhiteSpace(outputName))
              {
                string error = Emojis.ExclamationSymbol + " " + languageHandler.GetPhrase(senderDetail.ServerSettings.Language, "Error_UnknownRecipe");
                output.AppendLine($"{error}{command.CommandDetail}: " + url);
              }
              else
              {
                const int pad = 14;
                output.AppendLine($"```"); // Mono-space engage!
                output.AppendLine($"|--------------|--------------|--------------|");
                output.AppendLine($"|{recipe[0, 0].CentreString(pad)}|{recipe[1, 0].CentreString(pad)}|{recipe[2, 0].CentreString(pad)}|");
                output.AppendLine($"|--------------|--------------|--------------|");
                output.AppendLine($"|{recipe[0, 1].CentreString(pad)}|{recipe[1, 1].CentreString(pad)}|{recipe[2, 1].CentreString(pad)}|");
                output.AppendLine($"|--------------|--------------|--------------|");
                output.AppendLine($"|{recipe[0, 2].CentreString(pad)}|{recipe[1, 2].CentreString(pad)}|{recipe[2, 2].CentreString(pad)}|");
                output.AppendLine($"|--------------|--------------|--------------|");
                output.AppendLine($"```");
                output.AppendLine($"{outputName}");
              }
            }
          }
        }
        catch (Exception ex)
        {
          string error = Emojis.CrossSymbol + " " + languageHandler.GetPhrase(senderDetail.ServerSettings.Language, "Error_Oops");
          output.AppendLine(error + " " + command.CommandDetail);
          errorLogger.LogDebug("Query: " + query, true);
          errorLogger.LogDebug("JSON: " + json, true);
          errorLogger.LogDebug(ex.ToString(), true);
        }

        Response asyncResponse = new Response
        {
          ResponseType = ResponseType.Default,
          Embed = EmbedUtility.StringToEmbed(output.ToString(), null),
          Message = output.ToString()
        };

        await asyncResponder.SendResponseAsync(args, asyncResponse);
      });
      
      // Return out the lifecycle with no response.
      return new[] { Response.WaitForAsync };
    }

    private static string TidyRecipeParameter(string param)
    {
      return param.Replace("\\r", "").Replace("\\n", "").Replace(",", " x").Replace(";", " or ").Replace("  ", " ").Trim('\r', '\n', '|', '=', ' ');
    }
  }
}