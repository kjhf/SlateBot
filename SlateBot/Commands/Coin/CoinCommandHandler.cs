using SlateBot.DAL.CommandFile;
using System;

namespace SlateBot.Commands.Coin
{
  internal class CoinCommandHandler : ICommandHandler
  {
    public CommandHandlerType CommandHandlerType => CommandHandlerType.Coin;

    public Command CreateCommand(SlateBotController controller, CommandFile file)
    {
      bool moduleParsed = Enum.TryParse(file.Module, out ModuleType module);
      if (!moduleParsed)
      {
        module = ModuleType.General;
      }

      return (new CoinCommand(controller.languageHandler, file.Aliases, file.Examples, file.Help, module));
    }
  }
}