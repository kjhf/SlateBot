namespace PokemonLibrary
{
  public class EvolutionChain
  {
    public Pokemon BabyPokemon { get; internal set; }
    public EvolutionMethod BabyToBasicMethod { get; internal set; }
    public Pokemon BasicPokemon { get; internal set; }
    public EvolutionMethod BasicToStage1Method { get; internal set; }
    public Pokemon Stage1Pokemon { get; internal set; }
    public EvolutionMethod Stage1ToStage2Method { get; internal set; }
    public Pokemon Stage2Pokemon { get; internal set; }

    private bool PokemonDoesNotEvolve => BasicPokemon != null && (BabyPokemon == null && Stage1Pokemon == null && Stage2Pokemon == null);
    private bool PokemonEvolves => !PokemonDoesNotEvolve;
    private bool ChainHasBaby => BabyPokemon != null;
    private bool ChainHasStage1 => Stage1Pokemon != null;
    private bool ChainHasStage2 => Stage2Pokemon != null;
  }
}