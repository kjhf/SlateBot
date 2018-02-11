using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SlateBot.DAL.CommandFile;

namespace SlateBot.Commands.Coin
{
  class CoinCommandHandler : ICommandHandler
  {
    public CommandHandlerType CommandHandlerType => CommandHandlerType.Coin;
    
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

      return (new CoinCommand(languageHandler, file.Aliases, file.Examples, file.Help, module, responseType));
    }
  }
}
