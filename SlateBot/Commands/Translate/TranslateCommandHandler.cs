using CsHelper;
using SlateBot.DAL.CommandFile;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SlateBot.Commands.Translate
{
  internal class TranslateCommandHandler : ICommandHandler
  {
    public CommandHandlerType CommandHandlerType => CommandHandlerType.Translate;

    public Command CreateCommand(SlateBotController controller, CommandFile file)
    {
      bool moduleParsed = Enum.TryParse(file.Module, out ModuleType module);
      if (!moduleParsed)
      {
        module = ModuleType.Translation;
      }

      return (new TranslateCommand(controller, controller.ErrorLogger, controller.languageHandler, file.Aliases, file.Examples, file.Help, module));
    }
  }
}