namespace SlateBot.Errors
{
  public enum ErrorCode
  {
    /// <summary>
    /// No error or operation was successful.
    /// </summary>
    Success,

    /// <summary>
    /// This is a console message.
    /// </summary>
    ConsoleMessage,
    
    /// <summary>
    /// The message was not sent because the bot is only listening for messages.
    /// </summary>
    NotSendingMessage,

    /// <summary>
    /// The specified language file is missing or corrupt.
    /// </summary>
    MissingLanguageFile,

    /// <summary>
    /// The specified XML file is missing or corrupt.
    /// </summary>
    FailedToLoadXML,

    /// <summary>
    /// The lifecycle received an unexpected event.
    /// </summary>
    UnexpectedEvent,

    /// <summary>
    /// A Command Handler Type has been defined but a handler for it has not been implemented.
    /// </summary>
    CommandHandlerNotImplemented,

    /// <summary>
    /// A parameter specified to a command was incorrect.
    /// </summary>
    IncorrectParameter,
  }
}