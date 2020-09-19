namespace SlateBot.DAL
{
  /// <summary>
  /// An interface of data that can be loaded and saved to file.
  /// </summary>
  internal interface ISaveData
  {
    /// <summary>
    /// Convert the data into serializable XML
    /// </summary>
    /// <returns>A string of file contents</returns>
    string ToXML();

    /// <summary>
    /// Convert the data from serializable XML
    /// </summary>
    /// <returns></returns>
    bool FromXML(string xml);
  }
}