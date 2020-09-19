using SlateBot.DAL.CommandFile;
using System;

namespace SlateBot.Commands.Colour
{
  internal class ColourCommandHandler : ICommandHandler
  {
    public CommandHandlerType CommandHandlerType => CommandHandlerType.Colour;

    public Command CreateCommand(SlateBotController controller, CommandFile file)
    {
      bool moduleParsed = Enum.TryParse(file.Module, out ModuleType module);
      if (!moduleParsed)
      {
        module = ModuleType.General;
      }

      return (new ColourCommand(file.Aliases, file.Examples, file.Help, module));
    }
  }
}