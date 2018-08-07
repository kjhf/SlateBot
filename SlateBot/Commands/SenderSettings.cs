namespace SlateBot.Commands
{
  public class SenderSettings
  {
    public ServerSettings ServerSettings { get; }
    public UserSettings UserSettings { get; }

    public SenderSettings(ServerSettings serverSettings, UserSettings userSettings)
    {
      this.ServerSettings = serverSettings;
      this.UserSettings = userSettings;
    }
  }
}