using SlateBot.DAL.CommandFile;
using System;

namespace SlateBot.Commands.PokemonTypeCommand
{
  internal class PokemonTypeCommandHandler : ICommandHandler
  {
    public CommandHandlerType CommandHandlerType => CommandHandlerType.PokemonType;

    public Command CreateCommand(SlateBotController controller, CommandFile file)
    {
      bool moduleParsed = Enum.TryParse(file.Module, out ModuleType module);
      if (!moduleParsed)
      {
        module = ModuleType.General;
      }

      return (new PokemonTypeCommand(controller.languageHandler, file.Aliases, file.Examples, file.Help, module));
    }
  }
}