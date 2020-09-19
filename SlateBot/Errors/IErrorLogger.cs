using System;

namespace SlateBot.Errors
{
  public interface IErrorLogger
  {
    void LogDebug(string message, bool outputToConsole);

    void LogError(Error error);

    void LogException(Exception ex, ErrorSeverity severity);
  }
}