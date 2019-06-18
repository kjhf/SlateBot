using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SlateBot.DAL.CommandFile;
using SlateBot.Utility;

namespace SlateBot.Commands.ResponseMessage
{
  class ResponseMessageCommandHandler : ICommandHandler
  {
    public CommandHandlerType CommandHandlerType => CommandHandlerType.ResponseMessage;
    
    public Command CreateCommand(SlateBotController controller, CommandFile file)
    {
      bool moduleParsed = Enum.TryParse(file.Module, out ModuleType module);
      if (!moduleParsed)
      {
        module = ModuleType.General;
      }
      
      var dictionary = file.ExtraData;
      ResponseType responseType = ResponseType.Default;
      bool requiresSymbol = true;
      IEnumerable<string> choices;
      if (dictionary.Any())
      {
        bool hasResponseType = file.ExtraData.TryGetValue("ResponseType", out IEnumerable<string> responseTypeStr) > 0;
        if (hasResponseType)
        {
          Enum.TryParse(responseTypeStr.First(), out responseType);
          // If not parsed then "Default" will be used.
        }

        bool hasRequiresSymbol = file.ExtraData.TryGetValue("RequiresSymbol", out IEnumerable<string> requiresSymbolStr) > 0;
        if (hasRequiresSymbol)
        {
          requiresSymbol = bool.Parse(requiresSymbolStr.First());
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
