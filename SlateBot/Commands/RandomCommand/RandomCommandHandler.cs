using SlateBot.DAL.CommandFile;
using System;

namespace SlateBot.Commands.RandomCommand
{
  internal class RandomCommandHandler : ICommandHandler
  {
    public CommandHandlerType CommandHandlerType => CommandHandlerType.Random;

    public Command CreateCommand(SlateBotController controller, CommandFile file)
    {
      bool moduleParsed = Enum.TryParse(file.Module, out ModuleType module);
      if (!moduleParsed)
      {
        module = ModuleType.General;
      }

      return (new RandomCommand(controller.languageHandler, file.Aliases, file.Examples, file.Help, module));
    }
  }
}