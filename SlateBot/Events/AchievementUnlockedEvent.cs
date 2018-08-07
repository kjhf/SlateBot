using SlateBot.Commands;
using SlateBot.SaveData;
using SlateBot.SaveData.Achievements;
using System;

namespace SlateBot.Events
{
  public delegate void AchievementUnlockedEvent(object sender, AchievementUnlockedEventsArgs args);

  public class AchievementUnlockedEventsArgs : EventArgs
  {
    public readonly Achievement achievement;
    public readonly SenderSettings senderSettings;

    public AchievementUnlockedEventsArgs(Achievement achievement, SenderSettings senderSettings)
    {
      this.achievement = achievement ?? throw new ArgumentNullException(nameof(achievement));
      this.senderSettings = senderSettings ?? throw new ArgumentNullException(nameof(senderSettings));
    }
  }
}