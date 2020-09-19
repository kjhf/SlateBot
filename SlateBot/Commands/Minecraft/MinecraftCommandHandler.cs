using SlateBot.DAL.CommandFile;
using System;

namespace SlateBot.Commands.Minecraft
{
  internal class MinecraftCommandHandler : ICommandHandler
  {
    public CommandHandlerType CommandHandlerType => CommandHandlerType.Minecraft;

    public Command CreateCommand(SlateBotController controller, CommandFile file)
    {
      bool moduleParsed = Enum.TryParse(file.Module, out ModuleType module);
      if (!moduleParsed)
      {
        module = ModuleType.Lookup;
      }

      return (new MinecraftCommand(controller.ErrorLogger, controller.languageHandler, controller, file.Aliases, file.Examples, file.Help, module));
    }
  }
}