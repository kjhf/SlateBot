using SlateBot.DAL.CommandFile;
using System;

namespace SlateBot.Commands.Avatar
{
  internal class AvatarCommandHandler : ICommandHandler
  {
    public CommandHandlerType CommandHandlerType => CommandHandlerType.Avatar;

    public Command CreateCommand(SlateBotController controller, CommandFile file)
    {
      bool moduleParsed = Enum.TryParse(file.Module, out ModuleType module);
      if (!moduleParsed)
      {
        module = ModuleType.General;
      }

      return new AvatarCommand(controller.client, controller.languageHandler, controller, file.Aliases, file.Examples, file.Help, module);
    }
  }
}