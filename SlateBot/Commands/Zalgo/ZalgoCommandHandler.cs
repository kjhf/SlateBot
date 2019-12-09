using SlateBot.DAL.CommandFile;
using System;

namespace SlateBot.Commands.Zalgo
{
  internal class ZalgoCommandHandler : ICommandHandler
  {
    public CommandHandlerType CommandHandlerType => CommandHandlerType.Zalgo;

    public Command CreateCommand(SlateBotController controller, CommandFile file)
    {
      bool moduleParsed = Enum.TryParse(file.Module, out ModuleType module);
      if (!moduleParsed)
      {
        module = ModuleType.General;
      }

      return (new ZalgoCommand(file.Aliases, file.Examples, file.Help, module));
    }
  }
}