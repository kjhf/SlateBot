namespace PokemonLibrary
{
  public class Range
  {
    /// <summary>
    /// The minimum
    /// </summary>
    public int Min { get; set; }

    /// <summary>
    /// The maximum
    /// </summary>
    public int Max { get; set; }

    /// <summary>
    /// Get the average from the min and max.
    /// </summary>
    public double Average => (Max + Min) / 2;

    public Range()
    {
    }

    public Range(int min, int max)
    {
      Min = min;
      Max = max;
    }
  }
}