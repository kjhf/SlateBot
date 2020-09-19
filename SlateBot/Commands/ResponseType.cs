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
    Private,

    /// <summary>
    /// No synchronous response.
    /// </summary>
    None,

    /// <summary>
    /// This is a Please Wait message.
    /// </summary>
    PleaseWaitMessage,

    /// <summary>
    /// Use the default response handling (same channel context) as a TTS message.
    /// </summary>
    Default_TTS,

    /// <summary>
    /// Use the default response handling (same channel context) as a reaction (the message is converted into reacts).
    /// </summary>
    Default_React
  }
}