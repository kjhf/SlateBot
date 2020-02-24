using CsHelper;
using SlateBot.DAL.CommandFile;
using SlateBot.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SlateBot.Commands.Obabo
{
  internal class ObaboCommandHandler : ICommandHandler
  {
    public CommandHandlerType CommandHandlerType => CommandHandlerType.Obabo;

    public Command CreateCommand(SlateBotController controller, CommandFile file)
    {
      bool moduleParsed = Enum.TryParse(file.Module, out ModuleType module);
      if (!moduleParsed)
      {
        module = ModuleType.Image;
      }

      var dictionary = file.ExtraData;
      MirrorType mirrorType = MirrorType.LeftOntoRight;

      bool valid = dictionary.Any();
      if (valid)
      {
        bool hasMirrorType = file.ExtraData.TryGetValue("MirrorType", out IEnumerable<string> mirrorTypeStr) > 0;
        if (hasMirrorType)
        {
          valid = Enum.TryParse(mirrorTypeStr.First(), out mirrorType);
        }
        else
        {
          valid = false;
        }
      }
      else
      {
        valid = false;
      }

      return valid ? (new ObaboCommand(controller.ErrorLogger, controller.waitHandler, controller, controller.languageHandler, file.Aliases, file.Examples, file.Help, module, mirrorType)) : null;
    }
  }
}