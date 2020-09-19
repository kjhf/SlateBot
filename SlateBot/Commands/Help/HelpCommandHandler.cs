using SlateBot.DAL.CommandFile;
using System;

namespace SlateBot.Commands.Help
{
  internal class HelpCommandHandler : ICommandHandler
  {
    public CommandHandlerType CommandHandlerType => CommandHandlerType.Help;

    public Command CreateCommand(SlateBotController controller, CommandFile file)
    {
      bool moduleParsed = Enum.TryParse(file.Module, out ModuleType module);
      if (!moduleParsed)
      {
        module = ModuleType.General;
      }

      return (new HelpCommand(controller.commandHandlerController, controller.languageHandler, file.Aliases, file.Examples, file.Help, module));
    }
  }
}