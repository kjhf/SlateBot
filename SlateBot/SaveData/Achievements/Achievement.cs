using System.Collections.ObjectModel;

namespace SlateBot.SaveData.Achievements
{
  public abstract class Achievement
  {
    public static readonly ReadOnlyCollection<Achievement> AllAchievements = new ReadOnlyCollection<Achievement>(new Achievement[]
    {
      new FirstCommandAchievement(),
      new TenCommandsAchievement()
    });

    /// <summary>
    /// Get if the change in a stat will result in this achievement unlocking.
    /// </summary>
    /// <param name="propertyChanged">The name of the stat changing.</param>
    /// <param name="oldValue">The old value of the stat.</param>
    /// <param name="newValue">The new value of the stat.</param>
    public abstract bool DoesTrigger(string propertyChanged, object oldValue, object newValue);

    /// <summary>
    /// Get the localised description of the achievement.
    /// </summary>
    /// <param name="languageHandler"></param>
    /// <returns></returns>
    public abstract string GetDescription(Language.LanguageHandler languageHandler, Language.Languages language);

    /// <summary>
    /// Get if the achievement criteria has been fulfilled from the user's stats.
    /// </summary>
    /// <param name="stats"></param>
    /// <returns></returns>
    public abstract bool HasAchieved(UserStats stats);
  }
}