using System;
using SlateBot.DAL.CommandFile;

namespace SlateBot.Commands.Slapp
{
  class SlappCommandHandler : ICommandHandler
  {
    public CommandHandlerType CommandHandlerType => CommandHandlerType.Slapp;
    
    public Command CreateCommand(SlateBotController controller, CommandFile file)
    {
      bool moduleParsed = Enum.TryParse(file.Module, out ModuleType module);
      if (!moduleParsed)
      {
        module = ModuleType.Lookup;
      }

      return (new SlappCommand(controller.languageHandler, controller, file.Aliases, file.Examples, file.Help, module));
    }
  }
}
