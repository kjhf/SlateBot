using SlateBot.DAL.CommandFile;
using System;

namespace SlateBot.Commands.Reload
{
  internal class ReloadCommandHandler : ICommandHandler
  {
    public CommandHandlerType CommandHandlerType => CommandHandlerType.Reload;

    public Command CreateCommand(SlateBotController controller, CommandFile file)
    {
      bool moduleParsed = Enum.TryParse(file.Module, out ModuleType module);
      if (!moduleParsed)
      {
        module = ModuleType.BotAdmin;
      }

      return new ReloadCommand(controller, controller.languageHandler, file.Aliases, file.Examples, file.Help, module);
    }
  }
}