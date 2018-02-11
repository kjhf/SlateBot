using SlateBot.Errors;
using SlateBot.Language;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlateBot.DAL.CommandFile
{
  /// <summary>
  /// Command File Abstraction Layer handles the loading of the command XML files.
  /// </summary>
  class CommandFileDAL : IFileHandler
  {
    private readonly IErrorLogger errorLogger;
    private readonly string saveDirectory;

    public CommandFileDAL(IErrorLogger errorLogger, string saveDirectory)
    {
      this.errorLogger = errorLogger ?? throw new ArgumentNullException(nameof(errorLogger));
      this.saveDirectory = saveDirectory ?? throw new ArgumentNullException(nameof(saveDirectory));
      Directory.CreateDirectory(saveDirectory);
    }

    /// <summary>
    /// Loads command XML files present in the saveDirectory into <see cref="CommandFile"/>s
    /// </summary>
    /// <returns></returns>
    public List<CommandFile> LoadFiles()
    {
      List<CommandFile> result = new List<CommandFile>();

      foreach (var path in Directory.EnumerateFiles(saveDirectory))
      {
        string contents = File.ReadAllText(path);
        CommandFile file = new CommandFile(errorLogger);
        bool loaded = file.FromXML(contents);
        if (loaded)
        {
          result.Add(file);
        }
      }
      
      return result;
    }

    public void SaveFile(Commands.Command commandToSave, string filename)
    {
      try
      {
        CommandFile file = new CommandFile(errorLogger);
        file.Initialise(commandToSave);
        string contents = file.ToXML();
        if (contents != null)
        {
          if (!filename.EndsWith(".xml"))
          {
            filename += ".xml";
          }
          File.WriteAllText(Path.Combine(saveDirectory, filename), contents);
        }
      }
      catch (Exception ex)
      {
        errorLogger.LogException(ex, ErrorSeverity.Error);
      }
    }
  }
}
