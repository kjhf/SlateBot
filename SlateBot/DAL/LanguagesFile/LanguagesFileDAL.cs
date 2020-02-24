using SlateBot.Errors;
using SlateBot.Language;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlateBot.DAL.LanguagesFile
{
  /// <summary>
  /// Languages File Data Abstraction Layer handles the loading of the language XML files.
  /// </summary>
  internal class LanguagesFileDAL
  {
    private readonly LanguagesFile file;
    private readonly IErrorLogger errorLogger;
    private readonly string saveDirectory;

    /// <summary>
    /// The prefix of the project that handles the translatable phrases.
    /// https://www.poeditor.com/projects/view?id=88659
    /// </summary>
    private const string LanguageProjectPrefix = "kjbot";

    public LanguagesFileDAL(IErrorLogger errorLogger, string saveDirectory)
    {
      this.errorLogger = errorLogger ?? throw new ArgumentNullException(nameof(errorLogger));
      this.saveDirectory = saveDirectory ?? throw new ArgumentNullException(nameof(saveDirectory));

      Directory.CreateDirectory(saveDirectory);
      file = new LanguagesFile(errorLogger);
    }

    public Dictionary<Languages, LanguageDefinitions> LoadFiles()
    {
      Dictionary<Languages, LanguageDefinitions> result = new Dictionary<Languages, LanguageDefinitions>();

      foreach (Languages l in Enum.GetValues(typeof(Languages)))
      {
        string filename = GetFilename(l);
        string path = Path.Combine(saveDirectory, filename);
        if (File.Exists(path))
        {
          string contents = File.ReadAllText(path);
          bool loaded = file.FromXML(contents);
          if (loaded)
          {
            result[l] = new LanguageDefinitions(l, file.Data);
          }
        }
        else
        {
          errorLogger.LogError(new Error(ErrorCode.MissingLanguageFile, ErrorSeverity.Warning, path));
        }
      }
      
      return result;
    }

    private string GetFilename(Languages l) => $"{LanguageProjectPrefix}_{l}.xml";
  }
}
