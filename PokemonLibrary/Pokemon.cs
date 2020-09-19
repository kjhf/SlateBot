using CsHelper;
using System;
using System.Collections.Generic;

namespace PokemonLibrary
{
  public class Pokemon
  {
    private static readonly string[] templateParams = new string[] { "\n", "|" };

    /// <summary>
    /// The Pokémon's name
    /// </summary>
    public string Name { get; internal set; }

    /// <summary>
    /// The Pokémon's family description.
    /// </summary>
    public string FamilyDescription { get; internal set; }

    /// <summary>
    /// The Pokémon's different forms.
    /// </summary>
    public Form[] Forms { get; internal set; }

    /// <summary>
    /// The (normal form) Pokémon's primary type
    /// </summary>
    public PokemonType PrimaryType => Forms[0].PrimaryType;

    /// <summary>
    /// The (normal form) Pokémon's secondary type (or <see cref="PokemonType.NumberOfTypes"/> if not dual)
    /// </summary>
    public PokemonType SecondaryType => Forms[0].SecondaryType;

    /// <summary>
    /// The National Pokédex identifier for this Pokémon.
    /// </summary>
    public ushort NationalDexNumber { get; internal set; }

    /// <summary>
    /// The generation this Pokémon was introduced
    /// </summary>
    public byte GenerationIntroduced { get; internal set; }

    /// <summary>
    /// A percentage of male/female distribution, where 0 is 0% male, 100% female, to 100 is 100% male, 0% female.
    /// May be null for unknown.
    /// </summary>
    public double? GenderRatio { get; internal set; }

    /// <summary>
    /// List of moves this Pokémon can learn
    /// </summary>
    public Move[] Moves { get; internal set; }

    /// <summary>
    /// The Pokémon's abilities.
    /// </summary>
    public PokemonAbility[] Abilities { get; internal set; }

    /// <summary>
    /// The evolution chain this Pokémon participates in
    /// </summary>
    public EvolutionChain EvolutionChain { get; internal set; }

    /// <summary>
    /// The Egg Groups this Pokémon is in.
    /// </summary>
    public EggGroup[] EggGroups { get; internal set; }

    /// <summary>
    /// The number of steps to hatch the pokémon
    /// </summary>
    public Range HatchTime { get; internal set; }

    /// <summary>
    /// Height of the Pokémon in metres.
    /// </summary>
    public double Height { get; internal set; }

    /// <summary>
    /// Weight of the Pokémon in kg.
    /// </summary>
    public double Weight { get; internal set; }

    /// <summary>
    /// The stats of the Pokémon.
    /// </summary>
    public Stats Stats { get; internal set; } = new Stats();

    internal Pokemon()
    {
    }

    /// <summary>
    /// Parse a Pokemon page given its url.
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public static Pokemon ParsePage(string url)
    {
      string text = WebHelper.GetText(url);
      int indexOfHeader = text.IndexOf("==");
      if (indexOfHeader >= 0)
      {
        // Stop at the first header.
        TextParser parser = new TextParser(text.Substring(0, indexOfHeader));
        if (parser.MovePast("Infobox"))
        {
          Pokemon pokemon = new Pokemon();
          parser.MovePast("|");

          // At the least, we must be able to find the name.
          string dexNumber = TidyParameter(parser.ExtractBetween("ndex", templateParams));
          ushort dummyDexNumber;
          if (ushort.TryParse(dexNumber, out dummyDexNumber))
          {
            pokemon.NationalDexNumber = dummyDexNumber;
          }

          List<Form> forms = new List<Form>();
          forms.Add(new Form());
          pokemon.Name = TidyParameter(parser.ExtractBetween("name", templateParams));
          forms[0].Name = pokemon.Name;
          forms[0].PrimaryType = PokemonType.NumberOfTypes;
          forms[0].SecondaryType = PokemonType.NumberOfTypes;
          pokemon.FamilyDescription = TidyParameter(parser.ExtractBetween("category", "\n")); // Until end of line in case of templates inside description.

          PokemonType type1;
          PokemonType type2;
          // Needs pipe to protect from form types
          bool foundType1 = Enum.TryParse(TidyParameter(parser.ExtractBetween("|type1", templateParams)), true, out type1);
          bool foundType2 = Enum.TryParse(TidyParameter(parser.ExtractBetween("|type2", templateParams)), true, out type2);
          if (foundType1)
          {
            forms[0].PrimaryType = type1;
          }
          else
          {
            // Try again without safety pipe.
            foundType1 = Enum.TryParse(TidyParameter(parser.ExtractBetween("type1", templateParams)), true, out type1);
            if (foundType1)
            {
              forms[0].PrimaryType = type1;
            }

            foundType2 = Enum.TryParse(TidyParameter(parser.ExtractBetween("type2", templateParams)), true, out type2);
          }

          if (foundType2 && type2 != type1)
          {
            forms[0].SecondaryType = type2;
          }
          else
          {
            type2 = forms[0].SecondaryType = PokemonType.NumberOfTypes;
          }

          string ability1Name = TidyParameter(parser.ExtractBetween("ability1", templateParams));
          string ability2Name = TidyParameter(parser.ExtractBetween("ability2", templateParams));
          string abilityDreamName = TidyParameter(parser.ExtractBetween("abilityd", templateParams));
          List<PokemonAbility> abilities = new List<PokemonAbility>();
          if (ability1Name != string.Empty)
          {
            abilities.Add(new PokemonAbility { Name = ability1Name });
          }
          if (ability2Name != string.Empty)
          {
            abilities.Add(new PokemonAbility { Name = ability2Name });
          }
          if (abilityDreamName != string.Empty)
          {
            abilities.Add(new PokemonAbility { Name = abilityDreamName });
          }
          pokemon.Abilities = abilities.ToArray();

          List<EggGroup> eggGroups = new List<EggGroup>();
          EggGroup eggGroup1;
          EggGroup eggGroup2;
          bool foundEggGroup1 = Enum.TryParse(TidyParameter(parser.ExtractBetween("egggroup1", templateParams).Replace(" ", "").Replace("-", "")), true, out eggGroup1);
          bool foundEggGroup2 = Enum.TryParse(TidyParameter(parser.ExtractBetween("egggroup2", templateParams).Replace(" ", "").Replace("-", "")), true, out eggGroup2);
          if (foundEggGroup1)
          {
            eggGroups.Add(eggGroup1);
          }
          if (foundEggGroup2)
          {
            eggGroups.Add(eggGroup2);
          }
          pokemon.EggGroups = eggGroups.ToArray();

          string heightStr = TidyParameter(parser.ExtractBetween("height-m", templateParams));
          double dummyHeight;
          bool foundHeight = double.TryParse(heightStr, out dummyHeight);
          if (foundHeight)
          {
            pokemon.Height = dummyHeight;
          }

          string weightStr = TidyParameter(parser.ExtractBetween("weight-kg", templateParams));
          double dummyWeight;
          bool foundWeight = double.TryParse(weightStr, out dummyWeight);
          if (foundWeight)
          {
            pokemon.Weight = dummyWeight;
          }

          string eggCyclesStr = TidyParameter(parser.ExtractBetween("eggcycles", templateParams));
          int dummyEggCycles;
          bool foundEggCycles = int.TryParse(eggCyclesStr, out dummyEggCycles);
          if (foundEggCycles)
          {
            pokemon.HatchTime = new Range { Min = (dummyEggCycles * 257), Max = (dummyEggCycles * 257) + 256 };
          }

          string generationString = TidyParameter(parser.ExtractBetween("generation", templateParams));
          byte dummyGeneration;
          if (byte.TryParse(generationString, out dummyGeneration))
          {
            pokemon.GenerationIntroduced = dummyGeneration;
          }

          string genderRatio = TidyParameter(parser.ExtractBetween("gendercode", templateParams));
          byte dummyGenderRatio;
          if (byte.TryParse(genderRatio, out dummyGenderRatio))
          {
            switch (dummyGenderRatio)
            {
              case 255:
                pokemon.GenderRatio = null;
                break;

              case 254:
                pokemon.GenderRatio = 0;
                break;

              case 223:
                pokemon.GenderRatio = 12.5;
                break;

              case 191:
                pokemon.GenderRatio = 25;
                break;

              case 127:
                pokemon.GenderRatio = 50;
                break;

              case 63:
                pokemon.GenderRatio = 75;
                break;

              case 31:
                pokemon.GenderRatio = 87.5;
                break;

              case 0:
                pokemon.GenderRatio = 100;
                break;
            }
          }

          // Other forms
          for (int i = 2; i < 20; i++)
          {
            string formName = TidyParameter(parser.ExtractBetween("form" + i, templateParams));
            if (string.IsNullOrEmpty(formName))
            {
              // Form does not exist.
              break;
            }

            // else
            Form thisForm = new Form();
            thisForm.Name = formName;

            // Assume same type as normal form - changes if different.
            thisForm.PrimaryType = type1;
            thisForm.SecondaryType = type2;

            PokemonType formType1;
            PokemonType formType2;
            bool foundFormType1 = Enum.TryParse(TidyParameter(parser.ExtractBetween("form" + i + "type1", templateParams)), true, out formType1);
            bool foundFormType2 = Enum.TryParse(TidyParameter(parser.ExtractBetween("form" + i + "type2", templateParams)), true, out formType2);
            if (foundFormType1)
            {
              thisForm.PrimaryType = formType1;
            }
            if (foundFormType2 && formType2 != formType1)
            {
              thisForm.SecondaryType = formType2;
            }

            forms.Add(thisForm);
          }
          pokemon.Forms = forms.ToArray();

          // Find the stats.
          TextParser statsParser = new TextParser(text);
          bool foundStats = statsParser.MovePast("{{stats", true);
          if (!foundStats)
          {
            foundStats = statsParser.MovePast("{{BaseStats", true);
          }
          if (!foundStats)
          {
            foundStats = statsParser.MovePast("{{Base Stats", true);
          }

          if (foundStats)
          {
            string template = statsParser.ExtractUntil("}}", true);
            statsParser = new TextParser(template);
            var stats = pokemon.Stats;
            string hpStr = TidyParameter(statsParser.ExtractBetween("HP", new[] { "\n", "|", "}" }));
            string attackStr = TidyParameter(statsParser.ExtractBetween("Attack", new[] { "\n", "|", "}" }));
            string defenseStr = TidyParameter(statsParser.ExtractBetween("Defense", new[] { "\n", "|", "}" }));
            string spAttack = TidyParameter(statsParser.ExtractBetween("SpAtk", new[] { "\n", "|", "}" }));
            string spDefense = TidyParameter(statsParser.ExtractBetween("SpDef", new[] { "\n", "|", "}" }));
            string speed = TidyParameter(statsParser.ExtractBetween("Speed", new[] { "\n", "|", "}" }));

            byte dummy;
            if (byte.TryParse(hpStr, out dummy))
            {
              stats.BaseStats.Add(Stat.HP, dummy);
            }
            if (byte.TryParse(attackStr, out dummy))
            {
              stats.BaseStats.Add(Stat.Attack, dummy);
            }
            if (byte.TryParse(defenseStr, out dummy))
            {
              stats.BaseStats.Add(Stat.Defense, dummy);
            }
            if (byte.TryParse(spAttack, out dummy))
            {
              stats.BaseStats.Add(Stat.SpecialAttack, dummy);
            }
            if (byte.TryParse(spDefense, out dummy))
            {
              stats.BaseStats.Add(Stat.SpecialDefense, dummy);
            }
            if (byte.TryParse(speed, out dummy))
            {
              stats.BaseStats.Add(Stat.Speed, dummy);
            }
          }

          if (!string.IsNullOrEmpty(pokemon.Name))
          {
            KnowledgeBase.AddPokémon(pokemon);
          }
          return pokemon;
        }
      }

      return null;
    }

    private static string TidyParameter(string param)
    {
      return param.Replace("\\r", "").Replace("\\n", "").Replace(",", " x").Replace(";", " or ").Replace("  ", " ").Trim('\r', '\n', '|', '=', ' ');
    }
  }
}