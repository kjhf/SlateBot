using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SlateBot.Commands;
using SlateBot.DAL.ServerSettingsFile;
using SlateBot.Errors;
using SlateBot.Language;

namespace SlateBot.SavedSettings
{
  class ServerSettingsHandler : IController
  {
    public readonly ServerSettings consoleServerSettings;
    private readonly IErrorLogger errorLogger;
    private readonly DAL.SlateBotDAL dal;
    private Dictionary<ulong, ServerSettings> serverSettings;

    /// <summary>
    /// Constructor of the <see cref="ServerSettingsHandler"/>.
    /// </summary>
    public ServerSettingsHandler(IErrorLogger errorLogger, DAL.SlateBotDAL dal)
    {
      this.errorLogger = errorLogger;
      this.dal = dal;
      consoleServerSettings = new ServerSettings(Constants.ErrorId);
    }

    /// <summary>
    /// Initialise the server settings handler.
    /// </summary>
    public void Initialise()
    {
      serverSettings = new Dictionary<ulong, ServerSettings>();
      var serverSettingsFiles = dal.ReadServerSettingsFiles();
      foreach (var file in serverSettingsFiles)
      {
        try
        {
          ulong serverId = ulong.Parse(file.ServerId);
          ServerSettings fromFile = new ServerSettings(serverId)
          {
            BlockedModules = new HashSet<ModuleType>(file.BlockedModules.Select(m => (ModuleType)Enum.Parse(typeof(ModuleType), m))),
            CommandSymbol = file.CommandSymbol,
            JoinQuitChannelId = ulong.Parse(file.JoinQuitChannelId),
            JoinServerMessages = file.JoinServerMessages,
            Language = (Languages)Enum.Parse(typeof(Languages), file.Language),
            QuitServerMessages = file.QuitServerMessages,
            RateChannels = new HashSet<ulong>(file.RateChannels.Select(c => ulong.Parse(c))),
            ServerId = serverId,
            Splatoon2RotationChannels = new HashSet<ulong>(file.Splatoon2RotationChannels.Select(c => ulong.Parse(c))),
            TrackDeletedMessages = file.TrackDeletedMessages
          };
          serverSettings[fromFile.ServerId] = fromFile;
        }
        catch (Exception ex)
        {
          errorLogger.LogException(ex, ErrorSeverity.Error);
        }
      }
    }
    
    public ServerSettings GetOrCreateServerSettings(ulong key)
    {
      // If it does this returns.
      if (!serverSettings.TryGetValue(key, out ServerSettings retVal))
      {
        retVal = new ServerSettings(key);
        serverSettings.Add(key, retVal);
        dal.SaveServerSettingsFile(retVal);
      }
      return retVal;
    }

  }
}
