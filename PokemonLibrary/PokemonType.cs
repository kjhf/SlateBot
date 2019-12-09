using CsHelper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace PokemonLibrary
{
  public enum PokemonType
  {
    Normal,
    Fighting,
    Flying,
    Poison,
    Ground,
    Rock,
    Bug,
    Ghost,
    Steel,
    Fire,
    Water,
    Grass,
    Electric,
    Psychic,
    Ice,
    Dragon,
    Dark,
    Fairy,

    //
    NumberOfTypes
  }

  public static class PokemonTypeHandler
  {
    /// <summary>
    /// Constant for the number of Pokémon types that exist.
    /// </summary>
    public const int NumberOfTypes = (int)PokemonType.NumberOfTypes;

    /// <summary>
    /// Read only collection of available Pokémon Types.
    /// </summary>
    public static readonly IReadOnlyList<PokemonType> PokemonTypes = new List<PokemonType>(Enum.GetValues(typeof(PokemonType)).Cast<PokemonType>());

    /// <summary>
    /// The effectiveness of each type. First index is attacking type, second index is defending type.
    /// </summary>
    public static double[,] Effectiveness = InitialiseEffectivenessArray();

    /// <summary>
    /// Get a double array of a type who is attacking.
    /// Index by <see cref="PokemonType"/>.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static double[] GetAttacking(PokemonType type)
    {
      double[] attacking = new double[NumberOfTypes];
      for (int i = 0; i < NumberOfTypes; i++)
      {
        attacking[i] = Effectiveness[(int)type, i];
      }
      return attacking;
    }

    /// <summary>
    /// Get a double array of a type who is defending.
    /// Index by <see cref="PokemonType"/>.
    /// </summary>
    /// <param name="defenderTypes">The types that the defending Pokémon has.</param>
    /// <returns></returns>
    public static double[] GetDefending(params PokemonType[] defenderTypes)
    {
      double[] defending = new double[NumberOfTypes];
      for (int i = 0; i < defending.Length; i++)
      {
        defending[i] = 1;
      }

      if (defenderTypes != null)
      {
        for (int i = 0; i < NumberOfTypes; i++)
        {
          foreach (PokemonType defType in defenderTypes)
          {
            defending[i] *= Effectiveness[i, (int)defType];
          }
        }
      }
      return defending;
    }

    /// <summary>
    /// Get the <see cref="PokemonType"/> from input in a given culture.
    /// Returns success.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="cultureInfo"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static bool GetTypeFromLocalisedName(string name, CultureInfo cultureInfo, out PokemonType result)
    {
      try
      {
        IEnumerable<string> matchingResources = LanguageResources.ResourceManager.GetResourceSet(cultureInfo, true, true).OfType<DictionaryEntry>()
              .Where(e => e.Value.ToString().CloseEquals(name)).Select(entry => entry.Key.ToString());

        string key = matchingResources.FirstOrDefault(k => k.StartsWith("Type_"));
        if (key != null)
        {
          // Success case.
          return Enum.TryParse(key.Substring("Type_".Length), out result);
        }
      }
      catch (Exception ex)
      {
        Debug.WriteLine(nameof(GetTypeFromLocalisedName) + ": " + ex.Message + " -- for name " + name + " and culture " + cultureInfo);
      }

      result = PokemonType.NumberOfTypes;
      return false;
    }

    public static string GetLocalisedName(PokemonType type, CultureInfo culture)
    {
      try
      {
        return LanguageResources.ResourceManager.GetString("Type_" + type.ToString(), culture);
      }
      catch (Exception ex)
      {
        Console.WriteLine(nameof(GetLocalisedName) + ": " + ex.Message);
        return type.ToString();
      }
    }

    public static string GetTypeStatusImmunityString(PokemonType type, CultureInfo culture)
    {
      switch (type)
      {
        case PokemonType.Electric:
          return LanguageResources.ResourceManager.GetString(nameof(LanguageResources.TypeStatusImmunity_Electric), culture);

        case PokemonType.Fire:
          return LanguageResources.ResourceManager.GetString(nameof(LanguageResources.TypeStatusImmunity_Fire), culture);

        case PokemonType.Ice:
          return LanguageResources.ResourceManager.GetString(nameof(LanguageResources.TypeStatusImmunity_Ice), culture);

        case PokemonType.Poison:
          return LanguageResources.ResourceManager.GetString(nameof(LanguageResources.TypeStatusImmunity_Poison), culture);

        case PokemonType.Steel:
          return LanguageResources.ResourceManager.GetString(nameof(LanguageResources.TypeStatusImmunity_Steel), culture);
      }

      return "";
    }

    private static double[,] InitialiseEffectivenessArray()
    {
      double[,] retVal = new double[NumberOfTypes, NumberOfTypes];

      // Initialise all to 1.0
      for (int x = 0; x < NumberOfTypes; x++)
      {
        for (int y = 0; y < NumberOfTypes; y++)
        {
          retVal[x, y] = 1.0;
        }
      }

      // Differences in Normal attacking
      retVal[(int)PokemonType.Normal, (int)PokemonType.Rock] = 0.5;
      retVal[(int)PokemonType.Normal, (int)PokemonType.Ghost] = 0;
      retVal[(int)PokemonType.Normal, (int)PokemonType.Steel] = 0.5;

      // Differences in Fighting attacking
      retVal[(int)PokemonType.Fighting, (int)PokemonType.Normal] = 2;
      retVal[(int)PokemonType.Fighting, (int)PokemonType.Flying] = 0.5;
      retVal[(int)PokemonType.Fighting, (int)PokemonType.Poison] = 0.5;
      retVal[(int)PokemonType.Fighting, (int)PokemonType.Rock] = 2;
      retVal[(int)PokemonType.Fighting, (int)PokemonType.Bug] = 0.5;
      retVal[(int)PokemonType.Fighting, (int)PokemonType.Ghost] = 0;
      retVal[(int)PokemonType.Fighting, (int)PokemonType.Steel] = 2;
      retVal[(int)PokemonType.Fighting, (int)PokemonType.Psychic] = 0.5;
      retVal[(int)PokemonType.Fighting, (int)PokemonType.Ice] = 2;
      retVal[(int)PokemonType.Fighting, (int)PokemonType.Dark] = 2;
      retVal[(int)PokemonType.Fighting, (int)PokemonType.Fairy] = 0.5;

      // Differences in Flying attacking
      retVal[(int)PokemonType.Flying, (int)PokemonType.Fighting] = 2;
      retVal[(int)PokemonType.Flying, (int)PokemonType.Rock] = 0.5;
      retVal[(int)PokemonType.Flying, (int)PokemonType.Bug] = 2;
      retVal[(int)PokemonType.Flying, (int)PokemonType.Steel] = 0.5;
      retVal[(int)PokemonType.Flying, (int)PokemonType.Grass] = 2;
      retVal[(int)PokemonType.Flying, (int)PokemonType.Electric] = 0.5;

      // Differences in Poison attacking
      retVal[(int)PokemonType.Poison, (int)PokemonType.Poison] = 0.5;
      retVal[(int)PokemonType.Poison, (int)PokemonType.Ground] = 0.5;
      retVal[(int)PokemonType.Poison, (int)PokemonType.Rock] = 0.5;
      retVal[(int)PokemonType.Poison, (int)PokemonType.Ghost] = 0.5;
      retVal[(int)PokemonType.Poison, (int)PokemonType.Steel] = 0;
      retVal[(int)PokemonType.Poison, (int)PokemonType.Grass] = 2;
      retVal[(int)PokemonType.Poison, (int)PokemonType.Fairy] = 2;

      // Differences in Ground attacking
      retVal[(int)PokemonType.Ground, (int)PokemonType.Flying] = 0;
      retVal[(int)PokemonType.Ground, (int)PokemonType.Poison] = 2;
      retVal[(int)PokemonType.Ground, (int)PokemonType.Rock] = 2;
      retVal[(int)PokemonType.Ground, (int)PokemonType.Bug] = 0.5;
      retVal[(int)PokemonType.Ground, (int)PokemonType.Steel] = 2;
      retVal[(int)PokemonType.Ground, (int)PokemonType.Fire] = 2;
      retVal[(int)PokemonType.Ground, (int)PokemonType.Grass] = 0.5;
      retVal[(int)PokemonType.Ground, (int)PokemonType.Electric] = 2;

      // Differences in Rock attacking
      retVal[(int)PokemonType.Rock, (int)PokemonType.Fighting] = 0.5;
      retVal[(int)PokemonType.Rock, (int)PokemonType.Flying] = 2;
      retVal[(int)PokemonType.Rock, (int)PokemonType.Ground] = 0.5;
      retVal[(int)PokemonType.Rock, (int)PokemonType.Bug] = 2;
      retVal[(int)PokemonType.Rock, (int)PokemonType.Steel] = 0.5;
      retVal[(int)PokemonType.Rock, (int)PokemonType.Fire] = 2;
      retVal[(int)PokemonType.Rock, (int)PokemonType.Ice] = 2;

      // Differences in Bug attacking
      retVal[(int)PokemonType.Bug, (int)PokemonType.Fighting] = 0.5;
      retVal[(int)PokemonType.Bug, (int)PokemonType.Flying] = 0.5;
      retVal[(int)PokemonType.Bug, (int)PokemonType.Poison] = 0.5;
      retVal[(int)PokemonType.Bug, (int)PokemonType.Ghost] = 0.5;
      retVal[(int)PokemonType.Bug, (int)PokemonType.Steel] = 0.5;
      retVal[(int)PokemonType.Bug, (int)PokemonType.Fire] = 0.5;
      retVal[(int)PokemonType.Bug, (int)PokemonType.Grass] = 2;
      retVal[(int)PokemonType.Bug, (int)PokemonType.Psychic] = 2;
      retVal[(int)PokemonType.Bug, (int)PokemonType.Dark] = 2;
      retVal[(int)PokemonType.Bug, (int)PokemonType.Fairy] = 0.5;

      // Differences in Ghost attacking
      retVal[(int)PokemonType.Ghost, (int)PokemonType.Normal] = 0;
      retVal[(int)PokemonType.Ghost, (int)PokemonType.Ghost] = 2;
      retVal[(int)PokemonType.Ghost, (int)PokemonType.Psychic] = 2;
      retVal[(int)PokemonType.Ghost, (int)PokemonType.Dark] = 0.5;

      // Differences in Steel attacking
      retVal[(int)PokemonType.Steel, (int)PokemonType.Rock] = 2;
      retVal[(int)PokemonType.Steel, (int)PokemonType.Steel] = 0.5;
      retVal[(int)PokemonType.Steel, (int)PokemonType.Fire] = 0.5;
      retVal[(int)PokemonType.Steel, (int)PokemonType.Water] = 0.5;
      retVal[(int)PokemonType.Steel, (int)PokemonType.Electric] = 0.5;
      retVal[(int)PokemonType.Steel, (int)PokemonType.Ice] = 2;
      retVal[(int)PokemonType.Steel, (int)PokemonType.Fairy] = 2;

      // Differences in Fire attacking
      retVal[(int)PokemonType.Fire, (int)PokemonType.Rock] = 0.5;
      retVal[(int)PokemonType.Fire, (int)PokemonType.Bug] = 2;
      retVal[(int)PokemonType.Fire, (int)PokemonType.Steel] = 2;
      retVal[(int)PokemonType.Fire, (int)PokemonType.Fire] = 0.5;
      retVal[(int)PokemonType.Fire, (int)PokemonType.Water] = 0.5;
      retVal[(int)PokemonType.Fire, (int)PokemonType.Grass] = 2;
      retVal[(int)PokemonType.Fire, (int)PokemonType.Ice] = 2;
      retVal[(int)PokemonType.Fire, (int)PokemonType.Dragon] = 0.5;

      // Differences in Water attacking
      retVal[(int)PokemonType.Water, (int)PokemonType.Ground] = 2;
      retVal[(int)PokemonType.Water, (int)PokemonType.Rock] = 2;
      retVal[(int)PokemonType.Water, (int)PokemonType.Fire] = 2;
      retVal[(int)PokemonType.Water, (int)PokemonType.Water] = 0.5;
      retVal[(int)PokemonType.Water, (int)PokemonType.Grass] = 0.5;
      retVal[(int)PokemonType.Water, (int)PokemonType.Dragon] = 0.5;

      // Differences in Grass attacking
      retVal[(int)PokemonType.Grass, (int)PokemonType.Flying] = 0.5;
      retVal[(int)PokemonType.Grass, (int)PokemonType.Poison] = 0.5;
      retVal[(int)PokemonType.Grass, (int)PokemonType.Ground] = 2;
      retVal[(int)PokemonType.Grass, (int)PokemonType.Rock] = 2;
      retVal[(int)PokemonType.Grass, (int)PokemonType.Bug] = 0.5;
      retVal[(int)PokemonType.Grass, (int)PokemonType.Steel] = 0.5;
      retVal[(int)PokemonType.Grass, (int)PokemonType.Fire] = 0.5;
      retVal[(int)PokemonType.Grass, (int)PokemonType.Water] = 2;
      retVal[(int)PokemonType.Grass, (int)PokemonType.Grass] = 0.5;
      retVal[(int)PokemonType.Grass, (int)PokemonType.Dragon] = 0.5;

      // Differences in Electric attacking
      retVal[(int)PokemonType.Electric, (int)PokemonType.Flying] = 2;
      retVal[(int)PokemonType.Electric, (int)PokemonType.Ground] = 0;
      retVal[(int)PokemonType.Electric, (int)PokemonType.Water] = 2;
      retVal[(int)PokemonType.Electric, (int)PokemonType.Grass] = 0.5;
      retVal[(int)PokemonType.Electric, (int)PokemonType.Electric] = 0.5;
      retVal[(int)PokemonType.Electric, (int)PokemonType.Dragon] = 0.5;

      // Differences in Psychic attacking
      retVal[(int)PokemonType.Psychic, (int)PokemonType.Fighting] = 2;
      retVal[(int)PokemonType.Psychic, (int)PokemonType.Poison] = 2;
      retVal[(int)PokemonType.Psychic, (int)PokemonType.Steel] = 0.5;
      retVal[(int)PokemonType.Psychic, (int)PokemonType.Psychic] = 0.5;
      retVal[(int)PokemonType.Psychic, (int)PokemonType.Dark] = 0;

      // Differences in Ice attacking
      retVal[(int)PokemonType.Ice, (int)PokemonType.Flying] = 2;
      retVal[(int)PokemonType.Ice, (int)PokemonType.Ground] = 2;
      retVal[(int)PokemonType.Ice, (int)PokemonType.Steel] = 0.5;
      retVal[(int)PokemonType.Ice, (int)PokemonType.Fire] = 0.5;
      retVal[(int)PokemonType.Ice, (int)PokemonType.Water] = 0.5;
      retVal[(int)PokemonType.Ice, (int)PokemonType.Grass] = 2;
      retVal[(int)PokemonType.Ice, (int)PokemonType.Ice] = 0.5;
      retVal[(int)PokemonType.Ice, (int)PokemonType.Dragon] = 2;

      // Differences in Dragon attacking
      retVal[(int)PokemonType.Dragon, (int)PokemonType.Steel] = 0.5;
      retVal[(int)PokemonType.Dragon, (int)PokemonType.Dragon] = 2;
      retVal[(int)PokemonType.Dragon, (int)PokemonType.Fairy] = 0;

      // Differences in Dark attacking
      retVal[(int)PokemonType.Dark, (int)PokemonType.Fighting] = 0.5;
      retVal[(int)PokemonType.Dark, (int)PokemonType.Ghost] = 2;
      retVal[(int)PokemonType.Dark, (int)PokemonType.Psychic] = 2;
      retVal[(int)PokemonType.Dark, (int)PokemonType.Dark] = 0.5;
      retVal[(int)PokemonType.Dark, (int)PokemonType.Fairy] = 0.5;

      // Differences in Fairy attacking
      retVal[(int)PokemonType.Fairy, (int)PokemonType.Fighting] = 2;
      retVal[(int)PokemonType.Fairy, (int)PokemonType.Poison] = 0.5;
      retVal[(int)PokemonType.Fairy, (int)PokemonType.Steel] = 0.5;
      retVal[(int)PokemonType.Fairy, (int)PokemonType.Fire] = 0.5;
      retVal[(int)PokemonType.Fairy, (int)PokemonType.Dragon] = 2;
      retVal[(int)PokemonType.Fairy, (int)PokemonType.Dark] = 2;

      return retVal;
    }
  }
}