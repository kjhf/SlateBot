using SlateBot.Commands;
using SlateBot.DAL.CommandFile;
using SlateBot.Errors;
using SlateBot.Language;
using SlateBot.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SlateBot
{
  internal class UpdateController : IController
  {
    private bool preparingUpdate;

    /// <summary>
    /// Prepared XML files which have a language and path.
    /// </summary>
    private List<Tuple<Languages, string>> preppedXMLs;

    /// <summary>
    /// Prepared program files which have a path.
    /// </summary>
    private List<string> preppedProgramFiles;

    /// <summary>
    /// Reference to the main controller.
    /// </summary>
    private readonly SlateBotController controller;

    private string updateDir;


    public UpdateController(SlateBotController controller)
    {
      this.controller = controller ?? throw new ArgumentNullException(nameof(controller));
      preppedXMLs = new List<Tuple<Languages, string>>();
      preppedProgramFiles = new List<string>();
    }

    public void Initialise()
    {
      preparingUpdate = false;
      updateDir = Path.Combine(controller.dal.saveDataFolder, "Updates");
    }

    public void BeginPrepareUpdate()
    {
      preppedXMLs.Clear();
      preppedProgramFiles.Clear();
      Directory.CreateDirectory(updateDir);
      preparingUpdate = true;
    }

    public bool StageProgramFile(string path)
    {
      string name = Path.GetFileName(path);
      string destPath = Path.Combine(controller.dal.programFolder, name);
      if (File.Exists(destPath))
      {
        // File already exists.
        bool filesAreEqual = File.ReadAllBytes(destPath).SequenceEqual(File.ReadAllBytes(path));
        if (filesAreEqual)
        {
          return false;
        }
      }

      // Otherwise, prep
      preppedProgramFiles.Add(path);
      return true;
    }

    public bool StageXMLFile(Languages l, string path)
    {
      //string destPath = Path.Combine(controller.dal.commandsParentFolder, l.ToString(), path);

      // Otherwise, prep
      preppedXMLs.Add(new Tuple<Languages, string>(l, path));
      return true;
    }

    public void CommitUpdate()
    {
      // TODO.
      preparingUpdate = false;
    }
  }
}