using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PokemonLibrary
{
  public static class KnowledgeBase
  {
    /// <summary>
    /// A lookup of pokémon, keyed by LOWER CASE pokémon name.
    /// </summary>
    private static readonly Dictionary<string, Pokemon> Pokémon = new Dictionary<string, Pokemon>();

    /// <summary>
    /// A list of pokémon moves.
    /// </summary>
    private static readonly Dictionary<string, Move> Moves = new Dictionary<string, Move>();

    public static void AddPokémon(Pokemon pokemon)
    {
      Pokémon.Add(pokemon.Name.ToLowerInvariant(), pokemon);
    }
    
    /// <summary>
    /// Returns a Pokémon from its name. 
    /// </summary>
    /// <param name="pokémonName">The pokémon's name.</param>
    /// <returns></returns>
    public static Pokemon GetPokémon(string pokémonName)
    {
      Contract.Requires(pokémonName != null);
      Contract.EndContractBlock();

      string toLower = pokémonName.ToLowerInvariant();
      return (Pokémon.ContainsKey(toLower)) ? Pokémon[toLower] : null;
    }

    /// <summary>
    /// Returns a Pokémon from its name. 
    /// If not cached, this will search Bulbapedia for the Pokémon.
    /// Returns null if not found ...
    /// </summary>
    /// <param name="pokémonName">The pokémon's name.</param>
    /// <returns></returns>
    public static Pokemon GetOrFetchPokémon(string pokémonName)
    {
      Contract.Requires(pokémonName != null);
      Contract.EndContractBlock();

      Pokemon pokemon = GetPokémon(pokémonName);
      if (pokemon == null)
      {
        try
        {
          // For now, assume that it is a Pokémon.
          string url = "http://bulbapedia.bulbagarden.net/wiki/" + Uri.EscapeDataString(pokémonName + " (Pokémon)");
          string urlRaw = url + "?action=raw";
          pokemon = Pokemon.ParsePage(urlRaw);
        }
        catch (WebException)
        {
          // Pokémon not found.
        }
      }

      return pokemon;
    }

    /// <summary>
    /// Returns a Pokémon from its name. 
    /// </summary>
    /// <param name="moveName">The pokémon move's name.</param>
    /// <returns></returns>
    public static Move GetPokémonMove(string moveName)
    {
      Contract.Requires(moveName != null);
      Contract.EndContractBlock();

      string toLower = moveName.ToLowerInvariant();
      return (Moves.ContainsKey(toLower)) ? Moves[toLower] : null;
    }
  }
}
