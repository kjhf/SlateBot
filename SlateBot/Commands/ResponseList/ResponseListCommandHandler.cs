using SlateBot.DAL.CommandFile;
using SlateBot.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SlateBot.Commands.ResponseList
{
  internal class ResponseListCommandHandler : ICommandHandler
  {
    public CommandHandlerType CommandHandlerType => CommandHandlerType.ResponseList;

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
      string choiceFormat = "";
      string[][] choices;
      if (dictionary.Any())
      {
        bool hasRequiresSymbol = file.ExtraData.TryGetValue("RequiresSymbol", out IEnumerable<string> requiresSymbolStr) > 0;
        if (hasRequiresSymbol)
        {
          requiresSymbol = bool.Parse(requiresSymbolStr.First());
        }

        bool hasChoiceFormat = file.ExtraData.TryGetValue("ChoiceFormat", out IEnumerable<string> choiceFormats) > 0;
        if (hasChoiceFormat)
        {
          choiceFormat = choiceFormats.First();
        }

        List<List<string>> temp = new List<List<string>>();
        foreach (var pair in file.ExtraData)
        {
          if (pair.Key.Length == 1)
          {
            int section = pair.Key[0] - 'A';

            if (temp.Count <= section)
            {
              temp.Add(new List<string>());
            }
            temp[section].Add(pair.Value);
          }
        }
        choices = temp.Select(l => l.ToArray()).ToArray();
      }
      else
      {
        choices = new string[0][];
      }

      return (new ResponseListCommand(languageHandler, file.Aliases, choices, choiceFormat, file.Examples, file.Help, module, responseType, requiresSymbol));
    }
  }
}