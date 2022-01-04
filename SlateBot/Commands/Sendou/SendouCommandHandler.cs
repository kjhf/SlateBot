using SlateBot.DAL.CommandFile;
using System;

namespace SlateBot.Commands.Sendou
{
  internal class SendouCommandHandler : ICommandHandler
  {
    public CommandHandlerType CommandHandlerType => CommandHandlerType.Sendou;

    public Command CreateCommand(SlateBotController controller, CommandFile file)
    {
      bool moduleParsed = Enum.TryParse(file.Module, out ModuleType module);
      if (!moduleParsed)
      {
        module = ModuleType.Lookup;
      }

      return (new SendouCommand(controller.languageHandler, controller, file.Aliases, file.Examples, file.Help, module));
    }
  }
}