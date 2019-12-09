namespace PokemonLibrary
{
  public class Form
  {
    /// <summary>
    /// The Pokémon's name
    /// </summary>
    public string Name { get; internal set; }

    /// <summary>
    /// The Pokémon's primary type
    /// </summary>
    public PokemonType PrimaryType { get; internal set; }

    /// <summary>
    /// The Pokémon's secondary type (or <see cref="PokemonType.NumberOfTypes"/> if not dual)
    /// </summary>
    public PokemonType SecondaryType { get; internal set; }
  }
}