using SlateBot.Commands;
using SlateBot.Errors;
using SlateBot.Language;
using SlateBot.SaveData;
using System;
using System.Collections.Generic;

namespace SlateBot.SavedSettings
{
  internal class UserSettingsHandler : IController
  {
    private IErrorLogger ErrorLogger => controller.ErrorLogger;
    private DAL.SlateBotDAL ControllerDAL => controller.dal;
    private readonly SlateBotController controller;
    private Dictionary<ulong, UserSettings> userSettings;

    /// <summary>
    /// Constructor of the <see cref="UserSettingsHandler"/>.
    /// </summary>
    public UserSettingsHandler(SlateBotController controller)
    {
      this.controller = controller;
      controller.OnCommandReceived += Controller_OnCommandReceived;
    }

    private void Controller_OnCommandReceived(object sender, Events.CommandReceivedEventArgs args)
    {
      // Handle increment
      var userStats = args.senderSettings.UserSettings.UserStats;
      userStats.LastActiveChannelId = args.message.ChannelId;
      userStats.IncrementCommandsIssued(args.senderSettings);
      ControllerDAL.SaveUserSettingsFile(args.senderSettings.UserSettings);
    }

    /// <summary>
    /// Initialise the user settings handler.
    /// </summary>
    public void Initialise()
    {
      userSettings = new Dictionary<ulong, UserSettings>();
      var userSettingsFiles = ControllerDAL.ReadUserSettingsFiles();
      foreach (var file in userSettingsFiles)
      {
        try
        {
          ulong userId = ulong.Parse(file.UserId);
          UserSettings fromFile = new UserSettings(userId, new UserStats(userId, ulong.Parse(file.CommandsCount)));
          userSettings[fromFile.UserId] = fromFile;
          userSettings[fromFile.UserId].UserStats.AchievementUnlocked += UserStats_AchievementUnlocked;
        }
        catch (Exception ex)
        {
          ErrorLogger.LogException(ex, ErrorSeverity.Error);
        }
      }
    }

    private async void UserStats_AchievementUnlocked(object sender, Events.AchievementUnlockedEventsArgs args)
    {
      var lh = controller.languageHandler;
      Languages l = args.senderSettings.ServerSettings.Language;
      string achievementMessage = $"{Emojis.Trophy} {lh.GetPhrase(l, "Achievements_YouEarnedAnAchievement")} -= {args.achievement.GetDescription(lh, l)} =-";

      if (args.senderSettings.UserSettings.UserId == Constants.ConsoleId)
      {
        ErrorLogger.LogError(new Error(ErrorCode.ConsoleMessage, ErrorSeverity.Information, achievementMessage));
      }
      else
      {
        Response response = new Response
        {
          Embed = null,
          Message = achievementMessage,
          ResponseType = ResponseType.Default
        };
        await controller.SendResponseAsync(args.senderSettings.UserSettings.UserStats.LastActiveChannelId, response).ConfigureAwait(false);
      }
    }

    public UserSettings GetOrCreateUserSettings(ulong key)
    {
      // If it does this returns.
      if (!userSettings.TryGetValue(key, out UserSettings retVal))
      {
        retVal = new UserSettings(key);
        retVal.UserStats.AchievementUnlocked += UserStats_AchievementUnlocked;
        userSettings.Add(key, retVal);
        ControllerDAL.SaveUserSettingsFile(retVal);
      }
      return retVal;
    }
  }
}