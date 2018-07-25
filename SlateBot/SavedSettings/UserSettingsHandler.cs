using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SlateBot.Commands;
using SlateBot.Errors;
using SlateBot.Language;

namespace SlateBot.SavedSettings
{
  class UserSettingsHandler : IController
  {
    private readonly IErrorLogger errorLogger;
    private readonly DAL.SlateBotDAL dal;
    private Dictionary<ulong, UserSettings> userSettings;

    /// <summary>
    /// Constructor of the <see cref="UserSettingsHandler"/>.
    /// </summary>
    public UserSettingsHandler(SlateBotController controller)
    {
      this.errorLogger = controller.ErrorLogger;
      this.dal = controller.dal;
      controller.OnCommandReceived += Controller_OnCommandReceived;
    }

    private void Controller_OnCommandReceived(object sender, Events.CommandReceivedEventArgs args)
    {
      // Handle increment
      ++args.senderDetail.UserSettings.UserStats.commandsIssued;
      dal.SaveUserSettingsFile(args.senderDetail.UserSettings);
    }

    /// <summary>
    /// Initialise the user settings handler.
    /// </summary>
    public void Initialise()
    {
      userSettings = new Dictionary<ulong, UserSettings>();
      var userSettingsFiles = dal.ReadUserSettingsFiles();
      foreach (var file in userSettingsFiles)
      {
        try
        {
          ulong userId = ulong.Parse(file.UserId);
          UserSettings fromFile = new UserSettings(userId,
            new UserStats()
            {
              commandsIssued = ulong.Parse(file.CommandsCount)
            }
          );
          userSettings[fromFile.UserId] = fromFile;
        }
        catch (Exception ex)
        {
          errorLogger.LogException(ex, ErrorSeverity.Error);
        }
      }
    }
    
    public UserSettings GetOrCreateUserSettings(ulong key)
    {
      // If it does this returns.
      if (!userSettings.TryGetValue(key, out UserSettings retVal))
      {
        retVal = new UserSettings(key);
        userSettings.Add(key, retVal);
        dal.SaveUserSettingsFile(retVal);
      }
      return retVal;
    }

  }
}
