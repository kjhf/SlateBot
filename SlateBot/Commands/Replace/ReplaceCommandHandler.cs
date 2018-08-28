﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SlateBot.DAL.CommandFile;
using SlateBot.Utility;

namespace SlateBot.Commands.Replace
{
  class ReplaceCommandHandler : ICommandHandler
  {
    public CommandHandlerType CommandHandlerType => CommandHandlerType.Replace;
    
    public Command CreateCommand(Language.LanguageHandler languageHandler, CommandFile file)
    {
      bool moduleParsed = Enum.TryParse(file.Module, out ModuleType module);
      if (!moduleParsed)
      {
        module = ModuleType.General;
      }
      
      var dictionary = file.ExtraData;
      bool ignoreCase = true;
      string[] old;
      string[] @new;
      Dictionary<string, string> replacements = new Dictionary<string, string>();
      if (dictionary.Any())
      {
        bool hasIgnoreCase = file.ExtraData.TryGetValue("IgnoreCase", out IEnumerable<string> ignoreCaseStr) > 0;
        if (hasIgnoreCase)
        {
          ignoreCase = bool.Parse(ignoreCaseStr.First());
        }

        old = file.ExtraData.Where(pair => pair.Key.Equals("Old")).Select(pair => pair.Value).ToArray();
        @new = file.ExtraData.Where(pair => pair.Key.Equals("New")).Select(pair => pair.Value).ToArray();

        if (old.Length == @new.Length)
        {
          for (int i = 0; i < old.Length; i++)
          {
            replacements.Add(old[i], @new[i]);
          }
        }
      }
      
      return (new ReplaceCommand(languageHandler, file.Aliases, file.Examples, file.Help, module, replacements, ignoreCase));
    }
  }
}
