using SlateBot.DAL.CommandFile;
using System;

namespace SlateBot.Commands.PokemonCommand
{
  internal class PokemonCommandHandler : ICommandHandler
  {
    public CommandHandlerType CommandHandlerType => CommandHandlerType.Pokemon;

    public Command CreateCommand(SlateBotController controller, CommandFile file)
    {
      bool moduleParsed = Enum.TryParse(file.Module, out ModuleType module);
      if (!moduleParsed)
      {
        module = ModuleType.General;
      }

      return (new PokemonCommand(controller, file.Aliases, file.Examples, file.Help, module));
    }
  }
}