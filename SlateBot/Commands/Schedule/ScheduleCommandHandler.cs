using SlateBot.DAL.CommandFile;
using System;

namespace SlateBot.Commands.Schedule
{
  internal class ScheduleCommandHandler : ICommandHandler
  {
    public CommandHandlerType CommandHandlerType => CommandHandlerType.Schedule;

    public Command CreateCommand(SlateBotController controller, CommandFile file)
    {
      bool moduleParsed = Enum.TryParse(file.Module, out ModuleType module);
      if (!moduleParsed)
      {
        module = ModuleType.General;
      }

      return new ScheduleCommand(controller.languageHandler, controller.serverSettingsHandler, controller.scheduleHandler, controller, file.Aliases, file.Examples, file.Help, module);
    }
  }
}