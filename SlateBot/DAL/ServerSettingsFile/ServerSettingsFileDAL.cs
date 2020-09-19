using SlateBot.Errors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace SlateBot.DAL.ServerSettingsFile
{
  /// <summary>
  /// Command File Abstraction Layer handles the loading of the command XML files.
  /// </summary>
  internal class ServerSettingsFileDAL
  {
    private readonly IErrorLogger errorLogger;
    private readonly string saveDirectory;
    private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

    public ServerSettingsFileDAL(IErrorLogger errorLogger, string saveDirectory)
    {
      this.errorLogger = errorLogger ?? throw new ArgumentNullException(nameof(errorLogger));
      this.saveDirectory = saveDirectory ?? throw new ArgumentNullException(nameof(saveDirectory));
      Directory.CreateDirectory(saveDirectory);
    }

    /// <summary>
    /// Loads command XML files present in the saveDirectory into <see cref="CommandFile"/>s
    /// </summary>
    /// <returns></returns>
    public List<ServerSettingsFile> LoadFiles()
    {
      List<ServerSettingsFile> result = new List<ServerSettingsFile>();

      foreach (var path in Directory.EnumerateFiles(saveDirectory))
      {
        string contents = File.ReadAllText(path);
        ServerSettingsFile file = new ServerSettingsFile(errorLogger);
        bool loaded = file.FromXML(contents);
        if (loaded)
        {
          result.Add(file);
        }
      }

      return result;
    }

    public async void SaveFileAsync(ServerSettings settingsToSave, string filename)
    {
      await semaphore.WaitAsync();

      try
      {
        ServerSettingsFile file = new ServerSettingsFile(errorLogger);
        file.Initialise(settingsToSave);
        string contents = file.ToXML();
        if (contents != null)
        {
          if (!filename.EndsWith(".xml"))
          {
            filename += ".xml";
          }
          File.WriteAllText(Path.Combine(saveDirectory, filename), contents);
        }
      }
      catch (Exception ex)
      {
        errorLogger.LogException(ex, ErrorSeverity.Error);
      }

      semaphore.Release();
    }
  }
}