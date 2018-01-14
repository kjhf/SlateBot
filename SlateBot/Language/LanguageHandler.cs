﻿using System;
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
  class LanguageHandler : IHandler
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
    public LanguageHandler(DAL.SlateBotDAL dal)
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
      }

      return retVal;
    }
  }
}
