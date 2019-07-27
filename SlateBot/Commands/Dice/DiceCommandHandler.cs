using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SlateBot.DAL.CommandFile;

namespace SlateBot.Commands.Dice
{
  class DiceCommandHandler : ICommandHandler
  {
    public CommandHandlerType CommandHandlerType => CommandHandlerType.Dice;
    
    public Command CreateCommand(SlateBotController controller, CommandFile file)
    {
      bool moduleParsed = Enum.TryParse(file.Module, out ModuleType module);
      if (!moduleParsed)
      {
        module = ModuleType.General;
      }

      return (new DiceCommand(controller.languageHandler, file.Aliases, file.Examples, file.Help, module));
    }
  }
}
