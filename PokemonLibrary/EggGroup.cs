using CsHelper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace PokemonLibrary
{
  public enum EggGroup
  {
    Unknown,
    Monster,
    Water1,
    Bug,
    Flying,
    Field,
    Fairy,
    Grass,
    HumanLike,
    Water3,
    Mineral,
    Amorphous,
    Water2,
    Ditto,
    Dragon,
    Undiscovered,
  }

  public static class EggGroupHandler
  {
    /// <summary>
    /// Read only collection of available Pokémon Types.
    /// </summary>
    public static readonly IReadOnlyList<EggGroup> EggGroups = new List<EggGroup>(Enum.GetValues(typeof(EggGroup)).Cast<EggGroup>());

    /// <summary>
    /// Get the <see cref="PokemonType"/> from input in a given culture.
    /// Returns success.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="cultureInfo"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static bool GetGroupFromLocalisedName(string name, CultureInfo cultureInfo, out EggGroup result)
    {
      try
      {
        IEnumerable<string> matchingResources = LanguageResources.ResourceManager.GetResourceSet(cultureInfo, true, true).OfType<DictionaryEntry>()
              .Where(e => e.Value.ToString().CloseEquals(name)).Select(entry => entry.Key.ToString());

        string key = matchingResources.FirstOrDefault(k => k.StartsWith("EggGroup_"));
        if (key != null)
        {
          // Success case.
          return Enum.TryParse(key.Substring("EggGroup_".Length), out result);
        }
      }
      catch (Exception ex)
      {
        Debug.WriteLine(nameof(GetGroupFromLocalisedName) + ": " + ex.Message + " -- for name " + name + " and culture " + cultureInfo);
      }
      result = EggGroup.Unknown;
      return false;
    }

    public static string GetLocalisedName(EggGroup group, CultureInfo culture)
    {
      try
      {
        string result = LanguageResources.ResourceManager.GetString("EggGroup_" + group.ToString(), culture);
        return result;
      }
      catch (Exception ex)
      {
        Console.WriteLine(nameof(GetLocalisedName) + ": " + ex.Message);
        return group.ToString();
      }
    }
  }
}