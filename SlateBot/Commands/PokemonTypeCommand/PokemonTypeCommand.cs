using PokemonLibrary;
using SlateBot.Language;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SlateBot.Commands.PokemonTypeCommand
{
  public class PokemonTypeCommand : Command
  {
    private readonly LanguageHandler languageHandler;

    internal PokemonTypeCommand(LanguageHandler languageHandler, string[] aliases, string examples, string help, ModuleType module)
      : base(CommandHandlerType.PokemonType, aliases, examples, help, module)
    {
      this.languageHandler = languageHandler;
    }

    public override IList<Response> Execute(SenderSettings senderDetail, IMessageDetail args)
    {
      ServerSettings serverSettings = senderDetail.ServerSettings;
      CultureInfo cultureInfo = languageHandler.GetCultureInfo(serverSettings.Language);
      CommandMessageHelper command = new CommandMessageHelper(serverSettings.CommandSymbol, args.Message);
      string[] commandParams = command.CommandParams;
      Discord.Color responseColor = Discord.Color.Green;

      string[] typeStrings;

      if (command.CommandDetail.Contains('-'))
      {
        // User wrote e.g. normal-flying
        typeStrings = command.CommandDetail.Split('-');
      }
      else
      {
        typeStrings = command.CommandArgs.Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
      }

      if (typeStrings.Length == 0)
      {
        // No types specified.
        return Response.CreateArrayFromString("http://bulbapedia.bulbagarden.net/wiki/Type");
      }

      List<string> incorrectTypes = new List<string>();
      List<string> duplicateTypes = new List<string>();

      var foundTypes = new HashSet<PokemonType>();

      // TODO - translate.
      if (typeStrings.Length == 1 && typeStrings[0].Equals("all", StringComparison.OrdinalIgnoreCase))
      {
        // Add all types.
        foreach (PokemonType t in PokemonTypeHandler.PokemonTypes)
        {
          if (t != PokemonType.NumberOfTypes)
          {
            foundTypes.Add(t);
          }
        }
      }
      else
      {
        foreach (string typeStr in typeStrings)
        {
          bool found = PokemonTypeHandler.GetTypeFromLocalisedName(typeStr, cultureInfo, out PokemonType t);
          if (found)
          {
            bool added = foundTypes.Add(t);
            if (!added)
            {
              duplicateTypes.Add(typeStr);
            }
          }
          else
          {
            incorrectTypes.Add(typeStr);
          }
        }
      }

      bool foundAny = foundTypes.Any();

      if (!foundAny)
      {
        // Check if is actually a Pokémon.
        Pokemon pokemon = (KnowledgeBase.GetOrFetchPokémon(command.CommandDetail));
        if (pokemon != null)
        {
          foundTypes.Add(pokemon.PrimaryType);
          if (pokemon.SecondaryType != PokemonType.NumberOfTypes)
          {
            foundTypes.Add(pokemon.SecondaryType);
          }

          foundAny = true;

          // Ignore the typing - it's a Pokémon.
          incorrectTypes.Clear();
          duplicateTypes.Clear();
        }
      }

      StringBuilder sb = new StringBuilder();

      // Incorrect types
      if (incorrectTypes.Any())
      {
        sb.Append($"{Emojis.CrossSymbol} {languageHandler.GetPhrase(senderDetail.ServerSettings.Language, "Error_IncorrectParameter")}");
        sb.Append(": ");
        sb.AppendLine(string.Join(" ", incorrectTypes));
      }

      // Duplicate types
      if (duplicateTypes.Any())
      {
        sb.Append($"{Emojis.ExclamationSymbol} {languageHandler.GetPhrase(senderDetail.ServerSettings.Language, "Warning_SameType")}");
        sb.Append(": ");
        sb.AppendLine(string.Join(" ", duplicateTypes));
      }

      if (foundAny)
      {
        sb.AppendLine("```objectivec");
        sb.Append("// ");

        foreach (PokemonType foundType in foundTypes)
        {
          string foundTypeName = PokemonTypeHandler.GetLocalisedName(foundType, cultureInfo);
          sb.Append(foundTypeName);
          sb.Append(" ");
        }
        sb.AppendLine();

        // Attacking
        if (foundTypes.Count < 3)
        {
          sb.AppendLine($"# {languageHandler.GetPhrase(senderDetail.ServerSettings.Language, "Attacking")}");
          foreach (PokemonType foundType in foundTypes)
          {
            double[] attackingType1 = PokemonTypeHandler.GetAttacking(foundType);
            for (int i = 0; i < attackingType1.Length; i++)
            {
              double eff = attackingType1[i];
              if (eff != 1.0)
              {
                sb.Append(eff.ToString("G", cultureInfo.NumberFormat));
                sb.Append($"x {languageHandler.GetPhrase(senderDetail.ServerSettings.Language, "Against")} ");
                sb.Append(PokemonTypeHandler.GetLocalisedName(PokemonTypeHandler.PokemonTypes[i], cultureInfo));
                sb.Append($" {languageHandler.GetPhrase(senderDetail.ServerSettings.Language, "WhenAttackingWith")} ");
                sb.Append(PokemonTypeHandler.GetLocalisedName(foundType, cultureInfo));
                sb.AppendLine();
              }
            }
          }
        }
        sb.AppendLine();

        // Defending
        sb.AppendLine($"# {languageHandler.GetPhrase(senderDetail.ServerSettings.Language, "Defending")}");
        double[] defending = PokemonTypeHandler.GetDefending(foundTypes.ToArray());
        for (int i = 0; i < defending.Length; i++)
        {
          double eff = defending[i];
          if (eff != 1.0)
          {
            sb.Append(eff.ToString("G", cultureInfo.NumberFormat));
            sb.Append($"x {languageHandler.GetPhrase(senderDetail.ServerSettings.Language, "DamageTakenFrom")} ");
            sb.AppendLine(PokemonTypeHandler.GetLocalisedName(PokemonTypeHandler.PokemonTypes[i], cultureInfo));
          }
        }

        // Add on status immunities.
        foreach (PokemonType foundType in foundTypes)
        {
          string immunity = PokemonTypeHandler.GetTypeStatusImmunityString(foundType, cultureInfo);
          if (immunity != string.Empty)
          {
            sb.AppendLine(immunity);
          }
        }

        sb.Append("```");
      }

      return Response.CreateArrayFromString(sb.ToString(), responseColor);
    }
  }
}