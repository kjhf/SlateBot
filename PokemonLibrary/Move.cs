namespace PokemonLibrary
{
  public class Move
  {
    /// <summary>
    /// The name of this move.
    /// </summary>
    public string Name { get; internal set; }

    /// <summary>
    /// The Pokémon type of the move.
    /// </summary>
    public PokemonType Type { get; internal set; }

    /// <summary>
    /// The category of the move.
    /// </summary>
    public MoveCategory MoveCategory { get; internal set; }

    /// <summary>
    /// The power rating of the move, or null if it does not attack.
    /// </summary>
    public uint? Power { get; internal set; }

    /// <summary>
    /// The accuracy rating of the move, or null if it does not attack.
    /// </summary>
    public uint? Accuracy { get; internal set; }

    /// <summary>
    /// The Power Points of the move.
    /// </summary>
    public uint PP { get; internal set; }
  }
}