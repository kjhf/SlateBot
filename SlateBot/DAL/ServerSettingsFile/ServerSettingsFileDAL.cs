using Newtonsoft.Json;
using SlateBot.Commands;
using SlateBot.Commands.Schedule;
using SlateBot.Errors;
using SlateBot.Language;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    public List<ServerSettings> LoadFiles()
    {
      List<ServerSettings> result = new List<ServerSettings>();
      List<string> pathsToDelete = new List<string>();

      string oldSaveDir = Path.Combine(saveDirectory, "old");
      Directory.CreateDirectory(oldSaveDir);

      foreach (var path in Directory.EnumerateFiles(saveDirectory))
      {
        ServerSettings loadedSettings = null;
        string contents = File.ReadAllText(path);
        if (Path.GetExtension(path).Equals(".json", StringComparison.OrdinalIgnoreCase))
        {
          loadedSettings = JsonConvert.DeserializeObject<ServerSettings>(contents);
          if (loadedSettings != null)
          {
            result.Add(loadedSettings);
          }
        }
        else
        {
          // Deprecated XML handling
          ServerSettingsFile file = new ServerSettingsFile(errorLogger);
          if (file.FromXML(contents))
          {
            try
            {
              ulong serverId = ulong.Parse(file.ServerId);
              loadedSettings = new ServerSettings(serverId)
              {
                BlockedModules = new HashSet<ModuleType>(file.BlockedModules.Select(m => (ModuleType)Enum.Parse(typeof(ModuleType), m))),
                CommandSymbol = file.CommandSymbol,
                JoinQuitChannelId = ulong.Parse(file.JoinQuitChannelId),
                JoinServerMessages = file.JoinServerMessages,
                Language = (Languages)Enum.Parse(typeof(Languages), file.Language),
                QuitServerMessages = file.QuitServerMessages,
                RateChannels = new HashSet<ulong>(file.RateChannels.Select(c => ulong.Parse(c))),
                ScheduledMessages = new List<ScheduledMessageData>(file.ScheduledMessages),
                Splatoon2RotationChannels = new HashSet<ulong>(file.Splatoon2RotationChannels.Select(c => ulong.Parse(c))),
                TrackDeletedMessages = file.TrackDeletedMessages
              };

              File.WriteAllText(Path.ChangeExtension(path, ".json"), JsonConvert.SerializeObject(loadedSettings));
              result.Add(loadedSettings);
            }
            catch (Exception ex)
            {
              errorLogger.LogException(ex, ErrorSeverity.Error);
            }
          }
        }

        // If the server settings are defaults, then delete the file
        if (loadedSettings != null
          && (loadedSettings.BlockedModules.Count == 0)
          && (loadedSettings.CommandSymbol == "!")
          && (loadedSettings.JoinQuitChannelId == null || loadedSettings.JoinQuitChannelId == Constants.ErrorId)
          && (loadedSettings.JoinServerMessages.Count == 0)
          && (loadedSettings.Language == Languages.English)
          && (loadedSettings.QuitServerMessages.Count == 0)
          && (loadedSettings.RateChannels.Count == 0)
          && (loadedSettings.ScheduledMessages.Count == 0)
          && (loadedSettings.Splatoon2RotationChannels.Count == 0)
          && (!loadedSettings.TrackDeletedMessages)
          && (loadedSettings.ServerId != Constants.ConsoleId))
        {
          pathsToDelete.Add(path);
        }
        else if (!Path.GetExtension(path).Equals(".json", StringComparison.OrdinalIgnoreCase))
        {
          File.Move(path, Path.Combine(oldSaveDir, Path.GetFileName(path)));
        }
      }

      if (pathsToDelete.Count > 0)
      {
        try
        {
          errorLogger.LogDebug($"Deleting {pathsToDelete.Count} server files that are defaults.", true);
          pathsToDelete.ForEach(File.Delete);
        }
        catch (IOException ex)
        {
          errorLogger.LogException(ex, ErrorSeverity.Error);
        }
      }

      return result;
    }

    public async void SaveFileAsync(ServerSettings settingsToSave, string filename)
    {
      await semaphore.WaitAsync().ConfigureAwait(false);

      try
      {
        File.WriteAllText(Path.ChangeExtension(filename, ".json"), JsonConvert.SerializeObject(settingsToSave));
      }
      catch (Exception ex)
      {
        errorLogger.LogException(ex, ErrorSeverity.Error);
      }

      semaphore.Release();
    }
  }
}