using SlateBot.DAL.CommandFile;
using System;

namespace SlateBot.Commands.Achievement
{
  internal class AchievementCommandHandler : ICommandHandler
  {
    public CommandHandlerType CommandHandlerType => CommandHandlerType.Achievements;

    public Command CreateCommand(SlateBotController controller, CommandFile file)
    {
      bool moduleParsed = Enum.TryParse(file.Module, out ModuleType module);
      if (!moduleParsed)
      {
        module = ModuleType.General;
      }

      return (new AchievementCommand(controller.languageHandler, file.Aliases, file.Examples, file.Help, module));
    }
  }
}