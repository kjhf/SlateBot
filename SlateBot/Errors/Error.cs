namespace SlateBot.Errors
{
  public class Error
  {
    public readonly ErrorCode errorCode;
    public readonly ErrorSeverity severity;
    public readonly object extraData;

    /// <summary>
    /// Construct an <see cref="Error"/> with its details.
    /// </summary>
    /// <param name="errorCode"></param>
    /// <param name="severity"></param>
    public Error(ErrorCode errorCode, ErrorSeverity severity, object extraData = null)
    {
      this.errorCode = errorCode;
      this.severity = severity;
      this.extraData = extraData;
    }

    /// <summary>
    /// Overridden ToString. Returns {severity}: {errorCode} ({extraData}).
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      return ($"{severity}: {errorCode} {(extraData == null ? "" : $"({extraData})")}");
    }
  }
}