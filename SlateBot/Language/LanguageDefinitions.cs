using System.Collections.Generic;

namespace SlateBot.Language
{
  internal class LanguageDefinitions
  {
    /// <summary>
    /// Dictionary of language phrases,
    /// Keyed by phrase keys, values is the phrase.
    /// </summary>
    private readonly Dictionary<string, string> phrases;

    internal readonly Languages language;

    /// <summary>
    /// Construct <see cref="LanguageDefinitions"/> with the language
    /// </summary>
    public LanguageDefinitions(Languages language, Dictionary<string, string> phrases)
    {
      this.language = language;
      this.phrases = phrases;
    }

    /// <summary>
    /// Get a phrase from this language dictionary.
    /// Returns null if not found.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public string GetPhrase(string key)
    {
      phrases.TryGetValue(key, out string result);
      return result;
    }

    /// <summary>
    /// Get the complete dictionary of phrases.
    /// </summary>
    /// <returns></returns>
    public IReadOnlyDictionary<string, string> GetPhrases()
    {
      return phrases;
    }
  }
}