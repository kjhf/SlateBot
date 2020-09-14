using SlateBot.DAL.CommandFile;
using System;

namespace SlateBot.Commands.Say
{
  internal class SayCommandHandler : ICommandHandler
  {
    public CommandHandlerType CommandHandlerType => CommandHandlerType.Say;

    public Command CreateCommand(SlateBotController controller, CommandFile file)
    {
      bool moduleParsed = Enum.TryParse(file.Module, out ModuleType module);
      if (!moduleParsed)
      {
        module = ModuleType.BotAdmin;
      }

      return new SayCommand(controller, controller.languageHandler, file.Aliases, file.Examples, file.Help, module);
    }
  }
}