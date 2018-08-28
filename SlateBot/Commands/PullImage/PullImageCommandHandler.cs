using SlateBot.DAL.CommandFile;
using SlateBot.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SlateBot.Commands.PullImage
{
  internal class PullImageCommandHandler : ICommandHandler
  {
    public CommandHandlerType CommandHandlerType => CommandHandlerType.PullImage;

    public Command CreateCommand(Language.LanguageHandler languageHandler, CommandFile file)
    {
      bool moduleParsed = Enum.TryParse(file.Module, out ModuleType module);
      if (!moduleParsed)
      {
        module = ModuleType.General;
      }

      var dictionary = file.ExtraData;
      string url, formattedResponseURL, jsonProperty;

      if (dictionary.Any())
      {
        bool hasURL = file.ExtraData.TryGetValue("URL", out IEnumerable<string> urlStr) > 0;
        if (hasURL)
        {
          url = urlStr.First();
        }
        else
        {
          url = null;
        }

        bool hasFormattedResponseURL = file.ExtraData.TryGetValue("FormattedResponseURL", out IEnumerable<string> formattedResponseURLStr) > 0;
        if (hasFormattedResponseURL)
        {
          formattedResponseURL = formattedResponseURLStr.First();
        }
        else
        {
          formattedResponseURL = null;
        }

        bool hasJSONProperty = file.ExtraData.TryGetValue("JSONProperty", out IEnumerable<string> jsonPropertyStr) > 0;
        if (hasJSONProperty)
        {
          jsonProperty = jsonPropertyStr.First();
        }
        else
        {
          jsonProperty = null;
        }

      }
      else
      {
        throw new InvalidOperationException("XML for PullImageCommand is insufficient.");
      }

      return (new PullImageCommand(languageHandler, SlateBotProgram.GetAsyncResponder(), file.Aliases, file.Examples, file.Help, module, url, formattedResponseURL, jsonProperty));
    }
  }
}