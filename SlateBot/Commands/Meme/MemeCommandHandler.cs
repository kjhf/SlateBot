using CsHelper;
using SlateBot.DAL.CommandFile;
using SlateBot.Imaging;
using System;
using System.Collections.Generic;
using System.DrawingCore;
using System.IO;
using System.Linq;

namespace SlateBot.Commands.Meme
{
  internal class MemeCommandHandler : ICommandHandler
  {
    public CommandHandlerType CommandHandlerType => CommandHandlerType.Meme;

    public Command CreateCommand(SlateBotController controller, CommandFile file)
    {
      bool moduleParsed = Enum.TryParse(file.Module, out ModuleType module);
      if (!moduleParsed)
      {
        module = ModuleType.Image;
      }

      var dictionary = file.ExtraData;
      string template = null;
      Point topLeft = new Point(), topRight = new Point(), bottomLeft = new Point();

      bool valid = dictionary.Any();
      bool autoPoints = true;
      if (valid)
      {
        bool hasTemplate = file.ExtraData.TryGetValue("Template", out IEnumerable<string> templateStr) > 0;
        if (hasTemplate)
        {
          template = templateStr.First();
        }
        else
        {
          valid = false;
        }

        bool hasTopLeft = file.ExtraData.TryGetValue("TopLeft", out IEnumerable<string> topLeftStr) > 0;
        if (hasTopLeft)
        {
          var topLeftPointArr = topLeftStr.First().Split(',');
          topLeft = new Point(int.Parse(topLeftPointArr[0]), int.Parse(topLeftPointArr[1]));

          bool hasTopRight = file.ExtraData.TryGetValue("TopRight", out IEnumerable<string> topRightStr) > 0;
          if (hasTopRight)
          {
            var topRightPointArr = topRightStr.First().Split(',');
            topRight = new Point(int.Parse(topRightPointArr[0]), int.Parse(topRightPointArr[1]));

            bool hasBottomLeft = file.ExtraData.TryGetValue("BottomLeft", out IEnumerable<string> bottomLeftStr) > 0;
            if (hasBottomLeft)
            {
              var bottomLeftPointArr = bottomLeftStr.First().Split(',');
              bottomLeft = new Point(int.Parse(bottomLeftPointArr[0]), int.Parse(bottomLeftPointArr[1]));
              autoPoints = false;
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
        }
      }
      else
      {
        valid = false;
      }

      if (valid)
      {
        string templatePath = template;
        if (!Path.IsPathRooted(template))
        {
          templatePath = Path.Combine(controller.dal.memeFolder, templatePath);
        }

        if (autoPoints)
        {
          return new MemeCommand(controller.ErrorLogger, controller.waitHandler, controller, controller.languageHandler, file.Aliases, file.Examples, file.Help, module, templatePath);
        }
        else
        {
          return new MemeCommand(controller.ErrorLogger, controller.waitHandler, controller, controller.languageHandler, file.Aliases, file.Examples, file.Help, module, templatePath, topLeft, topRight, bottomLeft);
        }
      }
      else
      {
        return null;
      }
    }
  }
}