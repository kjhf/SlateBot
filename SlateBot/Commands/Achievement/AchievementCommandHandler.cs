using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SlateBot.DAL.CommandFile;

namespace SlateBot.Commands.Achievement
{
  class AchievementCommandHandler : ICommandHandler
  {
    public CommandHandlerType CommandHandlerType => CommandHandlerType.Achievements;
    
    public Command CreateCommand(Language.LanguageHandler languageHandler, CommandFile file)
    {
      bool moduleParsed = Enum.TryParse(file.Module, out ModuleType module);
      if (!moduleParsed)
      {
        module = ModuleType.General;
      }

      return (new AchievementCommand(languageHandler, file.Aliases, file.Examples, file.Help, module));
    }
  }
}
