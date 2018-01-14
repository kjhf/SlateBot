using SlateBot.DAL.LanguagesFile;
using SlateBot.Errors;
using SlateBot.Language;
using System.Collections.Generic;
using System.IO;

namespace SlateBot.DAL
{
  /// <summary>
  /// The Data Abstraction Layer handles saving and loading of bot and user data.
  /// </summary>
  internal class SlateBotDAL
  {
    internal readonly ErrorLogger errorLogger;
    private readonly LanguagesFileDAL languagesFileDAL;
    private readonly string saveDataFolder;

    public SlateBotDAL()
    {
      string currentFolder = Directory.GetCurrentDirectory();
      currentFolder = currentFolder.Substring(0, currentFolder.LastIndexOf(nameof(SlateBot)) + nameof(SlateBot).Length);
      this.saveDataFolder = Path.Combine(currentFolder, "SaveData");

      this.errorLogger = new ErrorLogger(Path.Combine(saveDataFolder, "Logs"));
      this.languagesFileDAL = new LanguagesFileDAL(errorLogger, Path.Combine(saveDataFolder, "Languages"));
    }

    internal void Initialise()
    {
      errorLogger.Initialise();
    }

    internal Dictionary<Languages, LanguageDefinitions> ReadLanguagesFiles()
    {
      return languagesFileDAL.LoadFiles();
    }
  }
}