using SlateBot.Language;
using SlateBot.Utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SlateBot.Commands.Dice
{
  public class DiceCommand : Command
  {
    private readonly LanguageHandler languageHandler;
    private const string D_REGEX = @"((\d)*d[+\-\df\b]+|(\d)+d[+\-\d]*\b)";

    internal DiceCommand(LanguageHandler languageHandler, string[] aliases, string examples, string help, ModuleType module)
      : base(CommandHandlerType.Dice, aliases, examples, help, module, true, true)
    {
      this.languageHandler = languageHandler;
    }

    public override IList<Response> Execute(SenderSettings senderDetail, IMessageDetail args)
    {
      StringBuilder output = new StringBuilder();
      ServerSettings serverSettings = senderDetail.ServerSettings;
      CommandMessageHelper command = new CommandMessageHelper(serverSettings.CommandSymbol, args.Message);

      bool handled = false;
      string toParse = command.CommandLower;
      string commandDetail = command.CommandDetail;

      if (Aliases.Contains(toParse))
      {
        if (Regex.IsMatch(commandDetail, D_REGEX, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled))
        {
          // Ignore the command entered because the full command is in the detail.
          toParse = commandDetail;
        }
        else
        {
          // Convert the command into a d6 command.
          string timesToRollStr = "1";
          string diceTypeStr = "6";
          string adjustmentStr = "+0";

          // If number of times to roll is specified.
          if (!string.IsNullOrEmpty(commandDetail))
          {
            // If adjustments are specified.
            if (commandDetail.Contains("-") || commandDetail.Contains("+"))
            {
              int index = commandDetail.LastIndexOf("+");
              if (index == -1)
              {
                index = commandDetail.LastIndexOf("-");
              }

              if (index > 0)
              {
                adjustmentStr = commandDetail.Substring(index, commandDetail.Length - index).Trim();
                timesToRollStr = commandDetail.Substring(0, index).Trim();
              }
            }
            else
            {
              timesToRollStr = commandDetail;
            }
          }

          toParse = $"{timesToRollStr}d{diceTypeStr}{adjustmentStr}";
        }
      }
      else
      {
        // Parse the whole thing
        toParse += " " + commandDetail;
      }

      if (Regex.IsMatch(toParse, D_REGEX, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled))
      {
        handled = true;

        string[] diceParams = toParse.Split('d', 'D');
        string rollsStr = string.IsNullOrEmpty(diceParams[0]) ? "1" : diceParams[0];
        string diceTypeStr = string.IsNullOrEmpty(diceParams[1]) ? "6" : diceParams[1];
        string adjustmentStr = "0";
        if (diceTypeStr.Contains("-") || diceTypeStr.Contains("+"))
        {
          int index = diceTypeStr.LastIndexOf("+");
          if (index == -1)
          {
            index = diceTypeStr.LastIndexOf("-");
          }

          if (index > 0)
          {
            adjustmentStr = diceTypeStr.Substring(index, diceTypeStr.Length - index).Trim();
            diceTypeStr = diceTypeStr.Substring(0, index).Trim();
          }
        }

        bool validNumberOfRolls = ushort.TryParse(rollsStr, out ushort rolls);
        bool validDiceType = byte.TryParse(diceTypeStr, out byte diceType) || diceTypeStr == "F" || diceTypeStr == "f";
        bool validAdjustmentDice = int.TryParse(adjustmentStr, NumberStyles.Any, CultureInfo.InvariantCulture, out int adjustment);

        if (validNumberOfRolls && validDiceType && validAdjustmentDice)
        {
          if (diceTypeStr == "F" || diceTypeStr == "f")
          {
            RollFudgeDice(rolls, adjustment, languageHandler, serverSettings.Language, ref output);
          }
          else
          {
            RollDice(rolls, diceType, adjustment, languageHandler, serverSettings.Language, ref output);
          }
          if (diceType == 1)
          {
            output.AppendLine($"_{languageHandler.GetPhrase(serverSettings.Language, "WhatDidYouExpect")}_");
          }
        }
        else
        {
          if (!validNumberOfRolls)
          {
            output.AppendLine($"{Emojis.CrossSymbol} {languageHandler.GetPhrase(serverSettings.Language, Errors.ErrorCode.IncorrectParameter)}: {rollsStr} [0-65535]");
          }

          if (!validDiceType)
          {
            output.AppendLine($"{Emojis.CrossSymbol} {languageHandler.GetPhrase(serverSettings.Language, Errors.ErrorCode.IncorrectParameter)}: {diceTypeStr} [0-255]");
          }

          if (!validAdjustmentDice)
          {
            output.AppendLine($"{Emojis.CrossSymbol} {languageHandler.GetPhrase(serverSettings.Language, Errors.ErrorCode.IncorrectParameter)}: {adjustmentStr} [+-F]");
          }
        }
      }

      if (handled)
      {
        string retVal = output.ToString();
        Response response = new Response
        {
          Embed = Utility.EmbedUtility.StringToEmbed(retVal),
          Message = retVal,
          ResponseType = ResponseType.Default
        };
        return new[] { response };
      }
      else
      {
        // Not handled.
        return Response.NoResponse;
      }
    }


    /// <summary>
    /// Roll x dice of y sides. Translatable.
    /// </summary>
    /// <param name="rolls"></param>
    /// <param name="diceType"></param>
    /// <param name="localisation"></param>
    /// <param name="adjustDice">Adjust total by amount (or 0)</param>
    /// <returns></returns>
    private static void RollDice(ushort rolls, byte diceType, int adjustDice, LanguageHandler languageHandler, Languages language, ref StringBuilder output)
    {
      if (diceType == 0)
      {
        output.AppendLine($"{Emojis.CrossSymbol} {languageHandler.GetPhrase(language, Errors.ErrorCode.IncorrectParameter)}: {diceType}");
      }
      else
      {
        output.AppendLine($"{Emojis.Dice} {languageHandler.GetPhrase(language, "Rolled")}: {rolls}d{diceType}:");

        string resultsStr;
        int total;

        if (rolls == 1)
        {
          // Simplify everything for a single roll.
          total = (rand.Next(diceType) + 1);
          resultsStr = total.ToString();
          output.Append(resultsStr);
        }
        else
        {
          int[] results = new int[diceType];

          for (int i = 0; i < rolls; i++)
          {
            int rolled0base = (rand.Next(diceType));
            results[rolled0base]++;
          }

          total = results.Select((rolled, index) => (index + 1) * rolled).Sum();
          double average = (double)total / rolls;
          resultsStr = string.Join("", results.Select((rolled, index) => (rolled > 0 ? ($"`{(index + 1)}: {rolled}x` ") : ("")))).Trim();
          output.AppendLine(resultsStr);
          output.Append($"{languageHandler.GetPhrase(language, "Average")}: {average.ToString("N2", languageHandler.GetCultureInfo(language))}, {languageHandler.GetPhrase(language, "Total")}: {total}");
        }

        if (adjustDice != 0)
        {
          output.Append(adjustDice.ToString("+#;-#;0") + " = " + (total + adjustDice));
        }
        output.AppendLine();
      }
    }

    /// <summary>
    /// Roll x dice of y sides. Translatable.
    /// </summary>
    /// <param name="rolls"></param>
    /// <param name="diceType"></param>
    /// <param name="localisation"></param>
    /// <param name="adjustDice">Adjust total by amount (or 0)</param>
    /// <returns></returns>
    private static void RollFudgeDice(ushort rolls, int adjustDice, LanguageHandler languageHandler, Languages language, ref StringBuilder output)
    {
      const byte diceType = 3;
      int[] results = new int[diceType];

      for (int i = 0; i < rolls; i++)
      {
        int rolled0base = (rand.Next(diceType));
        results[rolled0base]++;
      }

      int total = results[2] - results[0]; // Positive minus negative.

      StringBuilder resultsStr = new StringBuilder();
      for (int index = 0; index < diceType; index++)
      {
        int rolled = results[index];
        if (rolled > 0)
        {
          switch (index)
          {
            case 0:
              // Negative
              resultsStr.Append($"`-: {rolled}x` ");
              break;

            default:
            case 1:
              // Zero
              resultsStr.Append($"`0: {rolled}x` ");
              break;

            case 2:
              // Positive
              resultsStr.Append($"`+: {rolled}x` ");
              break;
          }
        }
      }

      output.AppendLine($"{Emojis.Dice} {languageHandler.GetPhrase(language, "Rolled")}: {rolls}dF:");

      if (rolls == 1)
      {
        // Simple case.
        output.Append(resultsStr);
      }
      else
      {
        output.Append($"{resultsStr} {languageHandler.GetPhrase(language, "Total")}: {total}");
      }

      if (adjustDice != 0)
      {
        output.Append(adjustDice.ToString("+#;-#;0") + " = " + (total + adjustDice));
      }
      output.AppendLine();
    }
  }
}
