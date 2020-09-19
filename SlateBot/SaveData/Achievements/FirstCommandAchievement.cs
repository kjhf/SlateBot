namespace SlateBot.SaveData.Achievements
{
  public class FirstCommandAchievement : Achievement
  {
    /// <summary>
    /// Get if the change in a stat will result in this achievement unlocking.
    /// </summary>
    /// <param name="propertyChanged">The name of the stat changing.</param>
    /// <param name="oldValue">The old value of the stat.</param>
    /// <param name="newValue">The new value of the stat.</param>
    public override bool DoesTrigger(string propertyChanged, object oldValue, object newValue)
    {
      bool triggers = false;

      if (propertyChanged == nameof(UserStats.CommandsIssued))
      {
        triggers = ((ulong)oldValue < 1) && ((ulong)newValue >= 1);
      }

      return triggers;
    }

    /// <summary>
    /// Get the localised description of the achievement.
    /// </summary>
    /// <param name="languageHandler"></param>
    /// <returns></returns>
    public override string GetDescription(Language.LanguageHandler languageHandler, Language.Languages language)
    {
      return languageHandler.GetPhrase(language, "Achievements_FirstCommand");
    }

    /// <summary>
    /// Get if the achievement criteria has been fulfilled from the user's stats.
    /// </summary>
    /// <param name="stats"></param>
    /// <returns></returns>
    public override bool HasAchieved(UserStats stats)
    {
      return stats.CommandsIssued > 0;
    }
  }
}