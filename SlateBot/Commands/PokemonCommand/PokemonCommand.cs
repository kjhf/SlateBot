using CsHelper;
using Newtonsoft.Json.Linq;
using PokemonLibrary;
using SlateBot.DAL;
using SlateBot.Errors;
using SlateBot.Language;
using SlateBot.Utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SlateBot.Commands.PokemonCommand
{
  public class PokemonCommand : Command
  {
    private readonly LanguageHandler languageHandler;
    private readonly PleaseWaitHandler waitHandler;
    private readonly IAsyncResponder asyncResponder;
    private readonly IErrorLogger errorLogger;
    private readonly SlateBotDAL dal;
    private static readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    // private readonly List<CorrelationData> pokemonStdDevs;

    internal PokemonCommand(SlateBotController controller, string[] aliases, string examples, string help, ModuleType module)
      : base(CommandHandlerType.PokemonType, aliases, examples, help, module)
    {
      this.languageHandler = controller.languageHandler;
      this.waitHandler = controller.waitHandler;
      this.asyncResponder = controller;
      this.dal = controller.dal;
      this.errorLogger = dal.errorLogger;

      // Build the database
      // this.pokemonStdDevs = dal.PokemonSDevs;
    }

    public override IList<Response> Execute(SenderSettings senderDetail, IMessageDetail args)
    {
      ServerSettings serverSettings = senderDetail.ServerSettings;
      CultureInfo cultureInfo = languageHandler.GetCultureInfo(serverSettings.Language);
      CommandMessageHelper command = new CommandMessageHelper(serverSettings.CommandSymbol, args.Message);

      Discord.Color responseColor = Discord.Color.Green;
      // First, check the cache if we already have this pokémon.
      string query = command.CommandDetail;
      if (args.URLs.Length > 0)
      {
        var response = new[] { waitHandler.CreatePleaseWaitResponse(senderDetail.ServerSettings.Language) };

        Task.Run(async () =>
        {
          Response asyncResponse = await CorrelatePokemonAsync(senderDetail.ServerSettings.Language, args);
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

        return response;
      }

      query = query.Replace("(Pokemon)", "").Replace("(Pokémon)", "").Trim();
      query = query.Replace("(move)", "").Trim();

      StringBuilder output = new StringBuilder();
      Pokemon pokemon = KnowledgeBase.GetPokémon(query);
      string imageUrl = null;
      bool isCached = (pokemon != null);

      try
      {
        // For now, assume that it is a Pokémon.
        string url = "https://bulbapedia.bulbagarden.net/wiki/" + query.Capitalize() + "_(Pokémon)";
        string urlRaw = url + "?action=raw";

        if (!isCached)
        {
          // Assume correct URL
          pokemon = Pokemon.ParsePage(urlRaw);
        }

        if (pokemon != null)
        {
          string p = pokemon.Name + "_(Pokémon)";
          dynamic imageJson = null;
          output
            .Append("https://bulbapedia.bulbagarden.net/wiki/")
            .Append(p)
            .AppendLine("#") // # is for mobile where () is not correctly parsed in the URL parser
            .AppendLine(MakeAPokemonString(pokemon, cultureInfo, serverSettings.Language));

          try
          {
            cancellationTokenSource.CancelAfter(300000);
            Task.Run(async () =>
            {
              imageJson = await JSONHelper.GetJsonAsync(Uri.EscapeUriString("https://bulbapedia.bulbagarden.net/w/api.php?action=query&format=json&prop=pageimages&titles=" + p)).ConfigureAwait(false);
            }, cancellationTokenSource.Token).Wait();
            JToken token = imageJson["query"]["pages"];
            token = token.First.First;
            imageUrl = token["thumbnail"]["source"]?.ToString();
          }
          catch (TaskCanceledException tcex)
          {
            errorLogger.LogDebug("Did not query Bulbapedia in time to retrieve the Pokemon image.", true);
            errorLogger.LogException(tcex, ErrorSeverity.Warning);
          }
          catch (Exception ex)
          {
            errorLogger.LogDebug($"Exception occurred retrieving the Pokemon image for {pokemon?.Name}.\n" +
              $"imageUrl: {imageUrl}\n" +
              $"imageJson: {imageJson}", true);
            errorLogger.LogException(ex, ErrorSeverity.Error);
          }
        }
        else
        {
          output.AppendLine($"{Emojis.ExclamationSymbol} {languageHandler.GetPhrase(senderDetail.ServerSettings.Language, "Error_PokemonNotFound")}: {urlRaw}");
        }
      }
      catch (WebException)
      {
        output.AppendLine($"{Emojis.CrossSymbol} {languageHandler.GetPhrase(senderDetail.ServerSettings.Language, "Error_PokemonNotFound")}: {query}");
      }
      catch (Exception ex)
      {
        output.AppendLine($"{languageHandler.GetPhrase(senderDetail.ServerSettings.Language, "Error_Oops")}: {ex.Message}");
      }

      return Response.CreateArrayFromString(output.ToString(), responseColor, pokemon.Name, imageUrl);
    }

    private string MakeAPokemonString(Pokemon pokemon, CultureInfo culture, Languages language)
    {
      StringBuilder sb = new StringBuilder("```");
      sb.AppendLine(pokemon.NationalDexNumber.ToString("D3") + ": " + pokemon.Name + ": " + pokemon.FamilyDescription + " Pokémon " + (pokemon.GenerationIntroduced != 0 ? "(Gen " + pokemon.GenerationIntroduced + ")" : ""));
      if (pokemon.PrimaryType != PokemonType.NumberOfTypes)
      {
        foreach (Form form in pokemon.Forms)
        {
          sb.AppendLine($"{form.Name} {languageHandler.GetPhrase(language, "Pokemon_Type")}: " + PokemonTypeHandler.GetLocalisedName(form.PrimaryType, culture) + (form.SecondaryType == PokemonType.NumberOfTypes ? "" : ("-" + PokemonTypeHandler.GetLocalisedName(form.SecondaryType, culture))));
        }
      }

      if (pokemon.Height != 0)
      {
        const double AVERAGE_PERSON_HEIGHT = 1.77;
        const double AVERAGE_PERSON_WEIGHT = 80.7;
        string ratioToAveragePersonHeight = (pokemon.Height / AVERAGE_PERSON_HEIGHT).ToString("N2", culture.NumberFormat);
        string ratioToAveragePersonWeight = (pokemon.Weight / AVERAGE_PERSON_WEIGHT).ToString("N2", culture.NumberFormat);

        sb.AppendLine($"{languageHandler.GetPhrase(language, "Pokemon_Height")}: " + pokemon.Height.ToString("N1", culture.NumberFormat) + "m (" + ratioToAveragePersonHeight + "x " + languageHandler.GetPhrase(language, "Pokemon_OfAnAveragePerson") + ")");
        sb.AppendLine($"{languageHandler.GetPhrase(language, "Pokemon_Weight")}: " + pokemon.Height.ToString("N1", culture.NumberFormat) + "kg (" + ratioToAveragePersonWeight + "x " + languageHandler.GetPhrase(language, "Pokemon_OfAnAveragePerson") + ")");
      }

      if (pokemon.GenderRatio != null)
      {
        sb.AppendLine($"{languageHandler.GetPhrase(language, "Pokemon_GenderRatio")}: {pokemon.GenderRatio}% {languageHandler.GetPhrase(language, "Pokemon_Male")} / {100 - pokemon.GenderRatio}% {languageHandler.GetPhrase(language, "Pokemon_Female")}");
      }
      else
      {
        sb.AppendLine($"{languageHandler.GetPhrase(language, "Pokemon_GenderRatio")}: {languageHandler.GetPhrase(language, "Pokemon_GenderlessOrUnknown")}");
      }

      if (pokemon.Abilities != null && pokemon.Abilities.Length > 0)
      {
        sb.AppendLine($"{languageHandler.GetPhrase(language, "Pokemon_Abilities")}: {string.Join(", ", pokemon.Abilities.Select(a => a.Name))}");
      }

      if (pokemon.EggGroups != null && pokemon.EggGroups.Length > 0)
      {
        sb.AppendLine($"{languageHandler.GetPhrase(language, "Pokemon_EggGroups")}: {string.Join(", ", pokemon.EggGroups.Select(a => EggGroupHandler.GetLocalisedName(a, culture)))}");
      }

      if (pokemon.HatchTime.Max != 0)
      {
        sb.AppendLine($"{languageHandler.GetPhrase(language, "Pokemon_HatchSteps")}: {pokemon.HatchTime.Average} ± 128");
      }

      if (pokemon.Stats.BaseStats.Count != 0)
      {
        const int totalPadding = 18;
        if (pokemon.Stats.BaseStats.ContainsKey(Stat.HP))
        {
          string statName = languageHandler.GetPhrase(language, "Pokemon_BaseHP") + ": ";
          byte stat = pokemon.Stats.BaseStats[Stat.HP];
          string bar = GetProgressBar(stat, 255); // (Blissey)
          string statStr = stat.ToString().PadRight(totalPadding - statName.Length);
          sb.AppendLine(statName + statStr + bar);
        }

        if (pokemon.Stats.BaseStats.ContainsKey(Stat.Attack))
        {
          string statName = languageHandler.GetPhrase(language, "Pokemon_BaseAttack") + ": ";
          byte stat = pokemon.Stats.BaseStats[Stat.Attack];
          string bar = GetProgressBar(stat, 190); // (Mega Mewtwo X)
          string statStr = stat.ToString().PadRight(totalPadding - statName.Length);
          sb.AppendLine(statName + statStr + bar);
        }

        if (pokemon.Stats.BaseStats.ContainsKey(Stat.Defense))
        {
          string statName = languageHandler.GetPhrase(language, "Pokemon_BaseDefence") + ": ";
          byte stat = pokemon.Stats.BaseStats[Stat.Defense];
          string bar = GetProgressBar(stat, 230); // (Mega Aggron)
          string statStr = stat.ToString().PadRight(totalPadding - statName.Length);
          sb.AppendLine(statName + statStr + bar);
        }

        if (pokemon.Stats.BaseStats.ContainsKey(Stat.SpecialAttack))
        {
          string statName = languageHandler.GetPhrase(language, "Pokemon_BaseSpecialAttack") + ": ";
          byte stat = pokemon.Stats.BaseStats[Stat.SpecialAttack];
          string bar = GetProgressBar(stat, 194); // (Mega Mewtwo Y)
          string statStr = stat.ToString().PadRight(totalPadding - statName.Length);
          sb.AppendLine(statName + statStr + bar);
        }

        if (pokemon.Stats.BaseStats.ContainsKey(Stat.SpecialDefense))
        {
          string statName = languageHandler.GetPhrase(language, "Pokemon_BaseSpecialDefence") + ": ";
          byte stat = pokemon.Stats.BaseStats[Stat.SpecialDefense];
          string bar = GetProgressBar(stat, 230); // (Shuckle)
          string statStr = stat.ToString().PadRight(totalPadding - statName.Length);
          sb.AppendLine(statName + statStr + bar);
        }

        if (pokemon.Stats.BaseStats.ContainsKey(Stat.Speed))
        {
          string statName = languageHandler.GetPhrase(language, "Pokemon_BaseSpeed") + ": ";
          byte stat = pokemon.Stats.BaseStats[Stat.Speed];
          string bar = GetProgressBar(stat, 180); // (Deoxys (Speed))
          string statStr = stat.ToString().PadRight(totalPadding - statName.Length);
          sb.AppendLine(statName + statStr + bar);
        }
      }
      sb.AppendLine("```");
      return sb.ToString();
    }

    private async Task<Response> CorrelatePokemonAsync(Languages language, IMessageDetail m)
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
            // It is, download and perform the correlation
            var tuple = await WebHelper.DownloadFile(url).ConfigureAwait(false);
            if (tuple.Item2 != null)
            {
              response = Response.CreateFromString("URLs not yet supported.");
              //response = DoCorrelatePokemonAsync(language, tuple.Item2);
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
          errorLogger.LogDebug($"Exception downloading or handling Pokemon file: {url}", true);
          errorLogger.LogException(ex, ErrorSeverity.Information);
          // try other urls
        }
      }
      return response;
    }

    /*
    private Response DoCorrelatePokemonAsync(Languages language, byte[] file)
    {
      Response response = new Response
      {
        ResponseType = ResponseType.Default
      };

      try
      {
        var fileCorr = new CorrelationData(file, true);

        string minimum = "", maximum = "";
        double minimumScore = double.MaxValue, maximumScore = double.MinValue;

        foreach (var otherCorrData in pokemonStdDevs)
        {
          double corr = fileCorr.GetCorrelation(otherCorrData);
          if (corr < minimumScore)
          {
            minimum = (string)otherCorrData.Tag;
            minimumScore = corr;
          }
          if (corr > maximumScore)
          {
            maximum = (string)otherCorrData.Tag;
            maximumScore = corr;
          }
        }

        response.Message = ($"{Emojis.TickSymbol} Best result: {minimum} scored {minimumScore}. Worst result: {maximum} scored {maximumScore}.");
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
    */

    private static string GetProgressBar(int value, int capacity)
    {
      StringBuilder sb = new StringBuilder();
      sb.Append("[");
      int bars = Math.Min(10, (int)(((value + 1) * 10) / (double)capacity));
      for (int i = 0; i < bars; i++)
      {
        sb.Append("#");
      }
      for (int i = bars; i < 10; i++)
      {
        sb.Append(" ");
      }
      sb.Append("]");
      return sb.ToString();
    }
  }
}