namespace SlateBot.Commands
{
  public enum ResponseType
  {
    /// <summary> 
    /// Use the default response handling (same channel context).
    /// </summary>
    Default,

    /// <summary> 
    /// Send the reply to the console and log only.
    /// </summary>
    LogOnly,

    /// <summary> 
    /// Reply to the user in private.
    /// </summary>
    Private
  }
}