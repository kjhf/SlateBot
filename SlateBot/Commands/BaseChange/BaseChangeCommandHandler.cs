using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SlateBot.DAL.CommandFile;
using SlateBot.Utility;

namespace SlateBot.Commands.BaseChange
{
  class BaseChangeCommandHandler : ICommandHandler
  {
    public CommandHandlerType CommandHandlerType => CommandHandlerType.BaseChange;
    
    public Command CreateCommand(SlateBotController controller, CommandFile file)
    {
      bool moduleParsed = Enum.TryParse(file.Module, out ModuleType module);
      if (!moduleParsed)
      {
        module = ModuleType.General;
      }
      
      var dictionary = file.ExtraData;
      int fromBase = 0;
      int toBase = 0;

      bool valid = dictionary.Any();
      if (valid)
      {
        bool hasFromBase = file.ExtraData.TryGetValue("FromBase", out IEnumerable<string> fromBaseStr) > 0;
        if (hasFromBase)
        {
          fromBase = int.Parse(fromBaseStr.First());
        }
        else
        {
          valid = false;
        }

        bool hasToBase = file.ExtraData.TryGetValue("ToBase", out IEnumerable<string> toBaseStr) > 0;
        if (hasToBase)
        {
          toBase = int.Parse(toBaseStr.First());
        }
        else
        {
          valid = false;
        }
      }
      else
      {
        valid = false;
      }

      return valid ? (new BaseChangeCommand(controller.languageHandler, file.Aliases, file.Examples, file.Help, module, fromBase, toBase)) : null;
    }
  }
}
