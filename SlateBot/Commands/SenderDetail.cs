namespace SlateBot.Commands
{
  public class SenderDetail
  {
    public ServerSettings ServerSettings { get; }
    //public UserSettings UserSettings { get; }

    public SenderDetail(ServerSettings serverSettings /*, UserSettings userSettings*/)
    {
      this.ServerSettings = serverSettings;
      //this.UserSettings = userSettings;
    }
  }
}