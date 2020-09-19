using SlateBot.DAL.CommandFile;
using System;

namespace SlateBot.Commands.SplatBuilds
{
  internal class SplatBuildsCommandHandler : ICommandHandler
  {
    public CommandHandlerType CommandHandlerType => CommandHandlerType.SplatBuilds;

    public Command CreateCommand(SlateBotController controller, CommandFile file)
    {
      bool moduleParsed = Enum.TryParse(file.Module, out ModuleType module);
      if (!moduleParsed)
      {
        module = ModuleType.Lookup;
      }

      return (new SplatBuildsCommand(controller.languageHandler, controller, file.Aliases, file.Examples, file.Help, module));
    }
  }
}