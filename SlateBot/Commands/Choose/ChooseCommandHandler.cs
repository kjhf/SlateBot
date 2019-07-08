using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SlateBot.DAL.CommandFile;
using SlateBot.Utility;

namespace SlateBot.Commands.Choose
{
  class ChooseCommandHandler : ICommandHandler
  {
    public CommandHandlerType CommandHandlerType => CommandHandlerType.Choose;
    
    public Command CreateCommand(SlateBotController controller, CommandFile file)
    {
      bool moduleParsed = Enum.TryParse(file.Module, out ModuleType module);
      if (!moduleParsed)
      {
        module = ModuleType.General;
      }
      
      var dictionary = file.ExtraData;
      string delimiter = "";

      bool valid = dictionary.Any();
      if (valid)
      {
        bool has = file.ExtraData.TryGetValue("Delimiter", out IEnumerable<string> delimiterStr) > 0;
        if (has)
        {
          delimiter = delimiterStr.First();
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

      return valid ? (new ChooseCommand(controller.languageHandler, file.Aliases, file.Examples, file.Help, module, delimiter)) : null;
    }
  }
}
