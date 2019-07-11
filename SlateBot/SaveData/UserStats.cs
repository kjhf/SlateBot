using SlateBot.Events;
using SlateBot.SaveData.Achievements;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SlateBot.SaveData
{
  // TODO:
  // - Achievements:
  //    - The remaning achievements from KJBot
  //    - Translatable descriptions for n commands achievements
  // - Game stats
  // - Friendship with the bot
  public class UserStats
  {
    /// <summary>
    /// The user id that these stats belong to.
    /// </summary>
    public readonly ulong userId;

    /// <summary>
    /// Back store to <see cref="CommandsIssued"/>
    /// </summary>
    private ulong commandsIssued;

    /// <summary>
    /// Back store to <see cref="LastActiveChannelId"/>
    /// </summary>
    private ulong lastActiveChannelId;

    public UserStats(ulong userId, ulong commandsIssued)
    {
      this.userId = userId;
      this.commandsIssued = commandsIssued;
    }
    
    public event AchievementUnlockedEvent AchievementUnlocked;

    /// <summary>
    /// The number of commands that have been issued by this user.
    /// </summary>
    public ulong CommandsIssued
    {
      get => commandsIssued;
    }

    /// <summary>
    /// The id for the channel last active for this user.
    /// </summary>
    public ulong LastActiveChannelId
    {
      get => lastActiveChannelId;
      set => lastActiveChannelId = value;
    }

    public void IncrementCommandsIssued(Commands.SenderSettings context)
    {
      var old = commandsIssued;
      ++commandsIssued;
      CheckForAchievementUnlocked(nameof(CommandsIssued), old, commandsIssued, context);
    }

    /// <summary>
    /// Get a list of achievements these stats shows the user has achieved.
    /// </summary>
    /// <returns></returns>
    public List<Achievement> GetAchievements()
    {
      List<Achievement> result = new List<Achievement>();

      foreach (Achievement a in Achievement.AllAchievements)
      {
        if (a.HasAchieved(this))
        {
          result.Add(a);
        }
      }

      return result;
    }

    private void CheckForAchievementUnlocked(string changedProperty, object oldValue, object newValue, Commands.SenderSettings context)
    {
      // If the stats has a subscriber to the event.
      if (AchievementUnlocked != null)
      {
        foreach (Achievement a in Achievement.AllAchievements)
        {
          if (a.DoesTrigger(changedProperty, oldValue, newValue))
          {
            Task.Run(() => AchievementUnlocked.Invoke(this, new AchievementUnlockedEventsArgs(a, context)));
          }
        }
      }
    }
  }
}