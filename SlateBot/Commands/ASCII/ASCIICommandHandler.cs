using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SlateBot.DAL.CommandFile;

namespace SlateBot.Commands.ASCII
{
  class ASCIICommandHandler : ICommandHandler
  {
    public CommandHandlerType CommandHandlerType => CommandHandlerType.ASCII;
    
    public Command CreateCommand(SlateBotController controller, CommandFile file)
    {
      bool moduleParsed = Enum.TryParse(file.Module, out ModuleType module);
      if (!moduleParsed)
      {
        module = ModuleType.Image;
      }

      return (new ASCIICommand(controller.ErrorLogger, controller.waitHandler, controller, controller.languageHandler, file.Aliases, file.Examples, file.Help, module));
    }
  }
}
