using SlateBot.Errors;
using System.Collections.Generic;
using System.Linq;

namespace SlateBot.SavedSettings
{
  public class ServerSettingsHandler : IController
  {
    private readonly IErrorLogger errorLogger;
    private readonly DAL.SlateBotDAL dal;
    private Dictionary<ulong, ServerSettings> serverSettings;
    internal IReadOnlyCollection<ServerSettings> ServerSettings => serverSettings.Values;

    /// <summary>
    /// Constructor of the <see cref="ServerSettingsHandler"/>.
    /// </summary>
    public ServerSettingsHandler(IErrorLogger errorLogger, DAL.SlateBotDAL dal)
    {
      this.errorLogger = errorLogger;
      this.dal = dal;
    }

    /// <summary>
    /// Initialise the server settings handler.
    /// </summary>
    public void Initialise()
    {
      serverSettings = dal.ReadServerSettingsFiles().ToDictionary(file => file.ServerId, file => file);
    }

    public ServerSettings GetOrCreateServerSettings(ulong key)
    {
      // If it does this returns.
      if (!serverSettings.TryGetValue(key, out ServerSettings retVal))
      {
        retVal = new ServerSettings(key);
        serverSettings.Add(key, retVal);

        // Don't write a server file until we need to save custom settings.
        // WriteServerSettings(retVal);
      }
      return retVal;
    }

    public void WriteServerSettings(ServerSettings settingsToSave)
    {
      dal.SaveServerSettingsFile(settingsToSave);
    }
  }
}