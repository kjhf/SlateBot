using SlateBot.DAL.CommandFile;
using System;

namespace SlateBot.Commands.TF2Craft
{
  internal class TF2CraftCommandHandler : ICommandHandler
  {
    public CommandHandlerType CommandHandlerType => CommandHandlerType.TF2Craft;

    public Command CreateCommand(SlateBotController controller, CommandFile file)
    {
      bool moduleParsed = Enum.TryParse(file.Module, out ModuleType module);
      if (!moduleParsed)
      {
        module = ModuleType.General;
      }

      return (new TF2CraftCommand(controller.languageHandler, file.Aliases, file.Examples, file.Help, module));
    }
  }
}