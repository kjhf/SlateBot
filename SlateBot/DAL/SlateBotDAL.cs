using SlateBot.DAL.CommandFile;
using SlateBot.DAL.LanguagesFile;
using SlateBot.DAL.ServerSettingsFile;
using SlateBot.DAL.UserSettingsFile;
using SlateBot.Errors;
using SlateBot.Language;
using System;
using System.Collections.Generic;
using System.IO;

namespace SlateBot.DAL
{
  /// <summary>
  /// The Data Abstraction Layer handles saving and loading of bot and user data.
  /// </summary>
  public class SlateBotDAL : IController
  {
    internal readonly ErrorLogger errorLogger;
    private readonly Dictionary<Languages, CommandFileDAL> commandFileDALs;
    private readonly LanguagesFileDAL languagesFileDAL;
    private readonly ServerSettingsFileDAL serverSettingsDAL;
    private readonly UserSettingsFileDAL userSettingsDAL;
    private readonly string saveDataFolder;
    private readonly string commandsParentFolder;

    public SlateBotDAL()
    {
      string currentFolder = Directory.GetCurrentDirectory();
      currentFolder = currentFolder.Substring(0, currentFolder.LastIndexOf(nameof(SlateBot)) + nameof(SlateBot).Length);
      this.saveDataFolder = Path.Combine(currentFolder, "SaveData");
      this.commandsParentFolder = Path.Combine(saveDataFolder, "Commands");

      this.errorLogger = new ErrorLogger(Path.Combine(saveDataFolder, "Logs"));
      this.languagesFileDAL = new LanguagesFileDAL(errorLogger, Path.Combine(saveDataFolder, "Languages"));
      this.serverSettingsDAL = new ServerSettingsFileDAL(errorLogger, Path.Combine(saveDataFolder, "ServerSettings"));
      this.userSettingsDAL = new UserSettingsFileDAL(errorLogger, Path.Combine(saveDataFolder, "UserSettings"));

      this.commandFileDALs = new Dictionary<Languages, CommandFileDAL>();
      foreach (Languages language in Enum.GetValues(typeof(Languages)))
      {
        commandFileDALs[language] = new CommandFileDAL(errorLogger, Path.Combine(commandsParentFolder, language.ToString()));
      }
    }

    public void Initialise()
    {
      errorLogger.Initialise();
    }

    internal Dictionary<Languages, List<CommandFile.CommandFile>> ReadCommandFiles()
    {
      Dictionary<Languages, List<CommandFile.CommandFile>> result = new Dictionary<Languages, List<CommandFile.CommandFile>>();
      foreach (Languages language in Enum.GetValues(typeof(Languages)))
      {
        result[language] = commandFileDALs[language].LoadFiles();
      }
      return result;
    }

    internal Dictionary<Languages, LanguageDefinitions> ReadLanguagesFiles()
    {
      return languagesFileDAL.LoadFiles();
    }

    internal List<ServerSettingsFile.ServerSettingsFile> ReadServerSettingsFiles()
    {
      return serverSettingsDAL.LoadFiles();
    }

    internal List<UserSettingsFile.UserSettingsFile> ReadUserSettingsFiles()
    {
      return userSettingsDAL.LoadFiles();
    }

    internal void SaveServerSettingsFile(ServerSettings serverSettingsToSave)
    {
      serverSettingsDAL.SaveFileAsync(serverSettingsToSave, serverSettingsToSave.ServerId.ToString());
    }

    internal void SaveUserSettingsFile(UserSettings userSettingsToSave)
    {
      userSettingsDAL.SaveFileAsync(userSettingsToSave, userSettingsToSave.UserId.ToString());
    }
  }
}