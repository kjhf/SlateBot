﻿using SlateBot.DAL.CommandFile;
using System;

namespace SlateBot.Commands.ASCII
{
  internal class ASCIICommandHandler : ICommandHandler
  {
    public CommandHandlerType CommandHandlerType => CommandHandlerType.ASCII;

    public Command CreateCommand(SlateBotController controller, CommandFile file)
    {
      bool moduleParsed = Enum.TryParse(file.Module, out ModuleType module);
      if (!moduleParsed)
      {
        module = ModuleType.Image;
      }

      return (new ASCIICommand(controller.ErrorLogger, controller.waitHandler, controller, controller.languageHandler, file.Aliases, file.Examples, file.Help, module));
    }
  }
}