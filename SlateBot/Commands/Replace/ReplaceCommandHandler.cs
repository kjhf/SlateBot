using CsHelper;
using SlateBot.DAL.CommandFile;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SlateBot.Commands.Replace
{
  internal class ReplaceCommandHandler : ICommandHandler
  {
    public CommandHandlerType CommandHandlerType => CommandHandlerType.Replace;

    public Command CreateCommand(SlateBotController controller, CommandFile file)
    {
      bool moduleParsed = Enum.TryParse(file.Module, out ModuleType module);
      if (!moduleParsed)
      {
        module = ModuleType.General;
      }

      var dictionary = file.ExtraData;
      bool ignoreCase = true;
      bool reverse = false;
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

        bool hasReverse = file.ExtraData.TryGetValue("Reverse", out IEnumerable<string> reverseStr) > 0;
        if (hasReverse)
        {
          reverse = bool.Parse(reverseStr.First());
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

      return (new ReplaceCommand(file.Aliases, file.Examples, file.Help, module, replacements, ignoreCase, reverse));
    }
  }
}