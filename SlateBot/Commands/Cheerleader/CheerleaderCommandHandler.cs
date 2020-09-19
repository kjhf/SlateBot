using SlateBot.DAL.CommandFile;
using System;

namespace SlateBot.Commands.Cheerleader
{
  internal class CheerleaderCommandHandler : ICommandHandler
  {
    public CommandHandlerType CommandHandlerType => CommandHandlerType.Cheerleader;

    public Command CreateCommand(SlateBotController controller, CommandFile file)
    {
      bool moduleParsed = Enum.TryParse(file.Module, out ModuleType module);
      if (!moduleParsed)
      {
        module = ModuleType.Memes;
      }

      return (new CheerleaderCommand(controller.languageHandler, file.Aliases, file.Examples, file.Help, module));
    }
  }
}