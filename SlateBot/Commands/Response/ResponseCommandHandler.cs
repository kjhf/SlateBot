using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SlateBot.DAL.CommandFile;
using SlateBot.Utility;

namespace SlateBot.Commands.Response
{
  class ResponseCommandHandler : ICommandHandler
  {
    public CommandHandlerType CommandHandlerType => CommandHandlerType.Response;
    
    public Command CreateCommand(Language.LanguageHandler languageHandler, CommandFile file)
    {
      bool moduleParsed = Enum.TryParse(file.Module, out ModuleType module);
      if (!moduleParsed)
      {
        module = ModuleType.General;
      }

      bool responseTypeParsed = Enum.TryParse(file.ResponseType, out ResponseType responseType);
      if (!responseTypeParsed)
      {
        responseType = ResponseType.Default;
      }

      var dictionary = file.ExtraData;
      bool requiresSymbol = true;
      IEnumerable<string> choices;
      if (dictionary.Any())
      {
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
      
      return (new ResponseCommand(languageHandler, file.Aliases, choices, file.Examples, file.Help, module, responseType, requiresSymbol));
    }
  }
}
