using CsHelper;
using SlateBot.DAL.CommandFile;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SlateBot.Commands.ResponseMessage
{
  internal class ResponseMessageCommandHandler : ICommandHandler
  {
    public CommandHandlerType CommandHandlerType => CommandHandlerType.ResponseMessage;

    public Command CreateCommand(SlateBotController controller, CommandFile file)
    {
      bool moduleParsed = Enum.TryParse(file.Module, out ModuleType module);
      if (!moduleParsed)
      {
        module = ModuleType.General;
      }

      bool requiresSymbolParsed = bool.TryParse(file.RequiresSymbol, out bool requiresSymbol);
      if (!requiresSymbolParsed)
      {
        requiresSymbol = true;
      }

      var dictionary = file.ExtraData;
      ResponseType responseType = ResponseType.Default;
      IEnumerable<string> choices;
      if (dictionary.Any())
      {
        bool hasResponseType = file.ExtraData.TryGetValue("ResponseType", out IEnumerable<string> responseTypeStr) > 0;
        if (hasResponseType)
        {
          Enum.TryParse(responseTypeStr.First(), out responseType);
          // If not parsed then "Default" will be used.
        }

        choices = file.ExtraData.Where(pair => pair.Key.StartsWith("C")).Select(pair => pair.Value);
      }
      else
      {
        choices = new string[0];
      }

      return (new ResponseMessageCommand(file.Aliases, choices, file.Examples, file.Help, module, responseType, requiresSymbol));
    }
  }
}