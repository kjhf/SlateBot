using SlateBot.DAL.CommandFile;
using System;

namespace SlateBot.Commands.Servers
{
  internal class ServersCommandHandler : ICommandHandler
  {
    public CommandHandlerType CommandHandlerType => CommandHandlerType.Servers;

    public Command CreateCommand(SlateBotController controller, CommandFile file)
    {
      bool moduleParsed = Enum.TryParse(file.Module, out ModuleType module);
      if (!moduleParsed)
      {
        module = ModuleType.BotAdmin;
      }

      return (new ServersCommand(controller.client, controller.serverSettingsHandler, controller.languageHandler, file.Aliases, file.Examples, file.Help, module));
    }
  }
}