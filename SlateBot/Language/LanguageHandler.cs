using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlateBot.Language
{
  /// <summary>
  /// Handles the language and localisation of phrases in the bot.
  /// </summary>
  public class LanguageHandler : IController
  {
    private static readonly IReadOnlyDictionary<Languages, CultureInfo> cultureInfo = new Dictionary<Languages, CultureInfo>()
    {
      { Languages.Default, CultureInfo.InvariantCulture  },
      { Languages.English, CultureInfo.CurrentUICulture  },
      { Languages.French, new CultureInfo("fr-FR") }
    };

    private Dictionary<Languages, LanguageDefinitions> languageDefinitions = new Dictionary<Languages, LanguageDefinitions>();
    private readonly DAL.SlateBotDAL dal;

    /// <summary>
    /// Constructor of the <see cref="LanguageHandler"/>.
    /// </summary>
    internal LanguageHandler(DAL.SlateBotDAL dal)
    {
      this.dal = dal;
    }

    /// <summary>
    /// Initialise the language handler.
    /// </summary>
    public void Initialise()
    {
      languageDefinitions = dal.ReadLanguagesFiles();
    }

    /// <summary>
    /// Get culture for a specified language.
    /// </summary>
    /// <param name="language"></param>
    /// <returns></returns>
    public CultureInfo GetCultureInfo(Languages language)
    {
      return cultureInfo[language];
    }

    /// <summary>
    /// Get a timespan in localised form Days : Hours : Minutes : Seconds
    /// </summary>
    public string GetLocalisedTimeSpan(Languages language, TimeSpan ts)
    {
      return $"{ts.Days} {GetPhrase(language, "Days")} : {ts.Hours} {GetPhrase(language, "Hours")} : {ts.Minutes} {GetPhrase(language, "Minutes")} : {ts.Seconds} {GetPhrase(language, "Seconds")}";
    }

    /// <summary>
    /// Get a localised phrase.
    /// </summary>
    /// <param name="language">The language to search</param>
    /// <param name="key">The key of the phrase to find</param>
    /// <returns>The found phrase, or a default phrase, or the key (in that order)</returns>
    public string GetPhrase(Languages language, string key)
    {
      string retVal = key;

      bool success = languageDefinitions.TryGetValue(language, out LanguageDefinitions languageDefinition);
      if (success)
      {
        string result = languageDefinition.GetPhrase(key);
        if (result == null)
        {
          // Not found
          // If not in the default language, search the default language.
          // Otherwise if the default language failed, use the key.
          if (language != Languages.Default)
          {
            retVal = GetPhrase(Languages.Default, key);
          } 
        }
        else
        {
          // Found, set.
          retVal = result;
        }
      }

      return retVal;
    }

    /// <summary>
    /// Get a localised phrase of a given <see cref="Commands.ModuleType"/>.
    /// </summary>
    /// <param name="language"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public string GetPhrase(Languages language, Commands.ModuleType key)
    {
      return GetPhrase(language, $"ModuleType_{key}");
    }

    /// <summary>
    /// Get a localised phrase of a given <see cref="Errors.ErrorCode"/>.
    /// </summary>
    /// <param name="language"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public string GetPhrase(Languages language, Errors.ErrorCode key)
    {
      return GetPhrase(language, $"Error_{key}");
    }
  }
}
