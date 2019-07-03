using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SlateBot.DAL.CommandFile;
using SlateBot.Imaging;
using SlateBot.Utility;

namespace SlateBot.Commands.YouTube
{
  class YouTubeCommandHandler : ICommandHandler
  {
    public CommandHandlerType CommandHandlerType => CommandHandlerType.YouTube;
    
    public Command CreateCommand(SlateBotController controller, CommandFile file)
    {
      bool moduleParsed = Enum.TryParse(file.Module, out ModuleType module);
      if (!moduleParsed)
      {
        module = ModuleType.General;
      }
      
      var dictionary = file.ExtraData;
      YouTubeCommandType youTubeCommandType = YouTubeCommandType.YouTube;

      bool valid = dictionary.Any();
      if (valid)
      {
        bool hasMirrorType = file.ExtraData.TryGetValue("YouTubeType", out IEnumerable<string> youTubeCommandTypeStr) > 0;
        if (hasMirrorType)
        {
          valid = Enum.TryParse(youTubeCommandTypeStr.First(), out youTubeCommandType);
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

      return valid ? (new YouTubeCommand(controller.ErrorLogger, controller.waitHandler, controller, controller.languageHandler, file.Aliases, file.Examples, file.Help, module, youTubeCommandType)) : null;
    }
  }
}
