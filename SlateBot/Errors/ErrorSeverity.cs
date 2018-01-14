namespace SlateBot.Errors
{
  /// <summary>
  /// An enumeration of the sevierty of errors.
  /// </summary>
  public enum ErrorSeverity
  {
    /// <summary>
    /// This error is diagnostic or debug verbose information only and can be ignored.
    /// </summary>
    Debug,

    /// <summary>
    /// This error is informational only and may be of assistance in general operation.
    /// </summary>
    Information,

    /// <summary>
    /// This error may cause a future error or indicates abnormal running without being an actual error.
    /// </summary>
    Warning,

    /// <summary>
    /// This is an error.
    /// </summary>
    Error,

    /// <summary>
    /// This error is fatal to the application and cannot continue.
    /// </summary>
    Fatal
  }
}