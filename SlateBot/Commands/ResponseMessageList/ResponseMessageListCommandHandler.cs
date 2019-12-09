using CsHelper;
using SlateBot.DAL.CommandFile;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SlateBot.Commands.ResponseMessageList
{
  internal class ResponseMessageListCommandHandler : ICommandHandler
  {
    public CommandHandlerType CommandHandlerType => CommandHandlerType.ResponseMessageList;

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
      string choiceFormat = "";
      string[][] choices;
      if (dictionary.Any())
      {
        bool hasResponseType = file.ExtraData.TryGetValue("ResponseType", out IEnumerable<string> responseTypeStr) > 0;
        if (hasResponseType)
        {
          Enum.TryParse(responseTypeStr.First(), out responseType);
          // If not parsed then "Default" will be used.
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

      return (new ResponseMessageListCommand(file.Aliases, choices, choiceFormat, file.Examples, file.Help, module, responseType, requiresSymbol));
    }
  }
}