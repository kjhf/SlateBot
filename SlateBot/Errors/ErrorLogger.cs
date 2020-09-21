using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading;

namespace SlateBot.Errors
{
  /// <summary>
  /// The Error Logger handles errors and exceptions.
  /// </summary>
  internal class ErrorLogger : IErrorLogger, IController
  {
    private static readonly ConsoleColor defaultConsoleColor = Console.ForegroundColor;
    private readonly BackgroundWorker backgroundWorker;
    private readonly string parentDirectory;
    private readonly ConcurrentQueue<Error> errorsToLog = new ConcurrentQueue<Error>();
    private readonly ConcurrentQueue<Tuple<Exception, ErrorSeverity>> exceptionsToLog = new ConcurrentQueue<Tuple<Exception, ErrorSeverity>>();

    private string ErrorFilePath => Path.Combine(parentDirectory, DateTime.Now.ToString("yyyy-MM-dd ") + nameof(SlateBot) + "Errors.txt");

    /// <summary>
    /// Create an <see cref="ErrorLogger"/> with a parent directory to save errors to.
    /// </summary>
    /// <param name="parentDirectory"></param>
    public ErrorLogger(string parentDirectory)
    {
      backgroundWorker = new BackgroundWorker
      {
        WorkerReportsProgress = false,
        WorkerSupportsCancellation = true
      };

      backgroundWorker.DoWork += BackgroundWorker_DoWork;

      this.parentDirectory = parentDirectory;
    }

    ~ErrorLogger()
    {
      if (backgroundWorker != null)
      {
        backgroundWorker.CancelAsync();
      }
    }

    private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
    {
      StringBuilder sb = new StringBuilder();
      while (!backgroundWorker.CancellationPending)
      {
        bool didWork = false;
        string timestamp = null; // Lazy initialisation.

        while (!errorsToLog.IsEmpty)
        {
          didWork = true;
          bool dequeued = errorsToLog.TryDequeue(out Error err);
          if (dequeued)
          {
            if (timestamp == null)
            {
              timestamp = DateTime.Now.ToString("[HH:mm:ss.fff]");
            }
            sb.AppendLine($"{timestamp} {err.severity}: {err.errorCode} {(err.extraData == null ? "" : $"({err.extraData})")}");
          }
        }

        while (!exceptionsToLog.IsEmpty)
        {
          didWork = true;
          bool dequeued = exceptionsToLog.TryDequeue(out Tuple<Exception, ErrorSeverity> err);
          if (dequeued)
          {
            if (timestamp == null)
            {
              timestamp = DateTime.Now.ToString("[HH:mm:ss.fff]");
            }
            sb.AppendLine($"{timestamp} {err.Item2}: {err.Item1}");
          }
        }

        if (didWork)
        {
          try
          {
            File.AppendAllText(ErrorFilePath, sb.ToString());
            sb.Clear();
            Thread.Sleep(1);
          }
          catch (Exception)
          {
            // Awkward. Don't clear the string builder so we can try again.
            Thread.Sleep(999);
          }
        }
        else
        {
          Thread.Sleep(1000);
        }
      }
    }

    /// <summary>
    /// Initialise the ErrorLogger by starting the background worker.
    /// </summary>
    public void Initialise()
    {
      Directory.CreateDirectory(parentDirectory);
      if (!backgroundWorker.IsBusy)
      {
        backgroundWorker.RunWorkerAsync();
      }
    }

    /// <summary>
    /// Log a debug error to log.
    /// </summary>
    /// <param name="error"></param>
    public void LogDebug(string message, bool outputToConsole = false)
    {
      Error error = new Error(ErrorCode.Success, ErrorSeverity.Debug, message);
      errorsToLog.Enqueue(error);

      if (outputToConsole)
      {
        OutputToConsole(message, ErrorSeverity.Information);
      }
    }

    /// <summary>
    /// Log an error to console and log.
    /// </summary>
    /// <param name="error"></param>
    public void LogError(Error error)
    {
      errorsToLog.Enqueue(error);
      if (error.errorCode == ErrorCode.ConsoleMessage)
      {
        if (error.extraData is string)
        {
          OutputToConsole((string)error.extraData, error.severity);
        }
        else if (error.extraData is Tuple<string, ConsoleColor> tuple)
        {
          OutputToConsole(tuple.Item1, error.severity, tuple.Item2);
        }
        else
        {
          OutputToConsole(error.extraData.ToString(), error.severity);
        }
      }
      else
      {
        OutputToConsole(error.ToString(), error.severity);
      }
    }

    /// <summary>
    /// Log an exception to console and log.
    /// </summary>
    /// <param name="ex"></param>
    /// <param name="severity"></param>
    public void LogException(Exception ex, ErrorSeverity severity)
    {
      exceptionsToLog.Enqueue(new Tuple<Exception, ErrorSeverity>(ex, severity));
      OutputToConsole(ex.ToString(), severity);
    }

    /// <summary>
    /// Print a message to the console in a severity colour.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="severity"></param>
    private void OutputToConsole(string message, ErrorSeverity severity = ErrorSeverity.Debug, ConsoleColor? customColor = null)
    {
      // Don't output debug to console.
      if (severity == ErrorSeverity.Debug)
      {
        return;
      }

      // Otherwise ...
      if (customColor != null)
      {
        Console.ForegroundColor = (ConsoleColor)customColor;
      }
      else
      {
        switch (severity)
        {
          default:
          case ErrorSeverity.Information:
            Console.ForegroundColor = defaultConsoleColor;
            break;

          case ErrorSeverity.Warning:
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            break;

          case ErrorSeverity.Error:
            Console.ForegroundColor = ConsoleColor.DarkRed;
            break;

          case ErrorSeverity.Fatal:
            Console.ForegroundColor = ConsoleColor.Red;
            break;
        }
      }
      message = DateTime.Now.ToString("[HH:mm:ss.fff] ") + message;
      Console.WriteLine(message);
      Console.ForegroundColor = defaultConsoleColor;
    }
  }
}