using SlateBot.DAL.CommandFile;
using System;

namespace SlateBot.Commands.SetName
{
  internal class SetNameCommandHandler : ICommandHandler
  {
    public CommandHandlerType CommandHandlerType => CommandHandlerType.SetName;

    public Command CreateCommand(SlateBotController controller, CommandFile file)
    {
      bool moduleParsed = Enum.TryParse(file.Module, out ModuleType module);
      if (!moduleParsed)
      {
        module = ModuleType.BotAdmin;
      }

      return new SetNameCommand(controller.client, controller.languageHandler, file.Aliases, file.Examples, file.Help, module);
    }
  }
}