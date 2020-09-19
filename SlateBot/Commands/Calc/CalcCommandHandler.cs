using SlateBot.DAL.CommandFile;
using System;

namespace SlateBot.Commands.Calc
{
  internal class CalcCommandHandler : ICommandHandler
  {
    public CommandHandlerType CommandHandlerType => CommandHandlerType.Calc;

    public Command CreateCommand(SlateBotController controller, CommandFile file)
    {
      bool moduleParsed = Enum.TryParse(file.Module, out ModuleType module);
      if (!moduleParsed)
      {
        module = ModuleType.General;
      }

      return (new CalcCommand(controller.languageHandler, file.Aliases, file.Examples, file.Help, module));
    }
  }
}