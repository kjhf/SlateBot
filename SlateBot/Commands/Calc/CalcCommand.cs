using NCalc;
using SlateBot.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlateBot.Commands.Calc
{
  public class CalcCommand : Command
  {
    private readonly LanguageHandler languageHandler;

    internal CalcCommand(LanguageHandler languageHandler, string[] aliases, string examples, string help, ModuleType module)
      : base(CommandHandlerType.Calc, aliases, examples, help, module)
    {
      this.languageHandler = languageHandler;
    }

    public override IList<Response> Execute(SenderSettings senderDetail, IMessageDetail args)
    {
      ServerSettings serverSettings = senderDetail.ServerSettings;
      CommandMessageHelper command = new CommandMessageHelper(serverSettings.CommandSymbol, args.Message);
      string[] commandParams = command.CommandParams;
      Discord.Color responseColor = Discord.Color.Green;

      object ans;
      StringBuilder output = new StringBuilder();
      string commandDetail = command.CommandDetail;

      try
      {
        if (commandDetail.Contains('!'))
        {
          output.AppendLine("*! is ambiguous, use ~ for 'bitwise-not'. Factorial is not yet supported. Assuming conditional-not.*");
        }

        if (commandDetail.Contains('^'))
        {
          output.AppendLine("*Assumed ^ means XOR. For Power, use Pow(x,y).*");
        }

        if (commandDetail.Contains('%'))
        {
          output.AppendLine("*Assumed % means modulo. For percentages, simply divide by 100.*");
        }

        if (commandDetail.Count(c => c == '=') == 1)
        {
          output.AppendLine($"*Assumed = means is equal. For an equation solver, use the `{senderDetail.ServerSettings.CommandSymbol}wolframalpha` (`{senderDetail.ServerSettings.CommandSymbol}wa`) command.*");
        }

        // Tidy up some common operator symbols.
        commandDetail = commandDetail
          .Replace("fix", "floor") // before the x replacement
          .Replace("x", "*")
          .Replace("modulo", "mod")
          .Replace("mod", "%")
          .Replace("¬", "~")
          .Replace("cosine", "cos")
          .Replace("sine", "sin")
          .Replace("tangent", "tan")
          .Replace("sqr ", "sqrt ")
          .Replace("root", "sqrt")
          .Replace("√", "sqrt")
          .Replace("atn", "atan")
          .Replace("π", "[pi]")
          .Replace("∞", "[infinity]")
          .Replace("pi ", "[pi] ")
          .Replace("infinity", "[infinity]")
          .Replace("inf", "[infinity]");

        // Use ; to split statements. : can be used for more powerful functionality
        // such as [Convert]::ToString(1234, 16)
        foreach (string split in commandDetail.Split(';'))
        {
          Expression e = new Expression(split, EvaluateOptions.NoCache | EvaluateOptions.IgnoreCase);
          e.Parameters["pi"] = Math.PI;
          e.Parameters["e"] = Math.E;
          e.Parameters["infinity"] = double.MaxValue;
          ans = e.Evaluate();
          if (ans == null)
          {
            output.AppendLine("I don't understand your input (undefined). Please check constants are enclosed by parentheses.");
          }
          else
          {
            if (e.HasErrors())
            {
              output.AppendLine(e.Error);
            }
            else
            {
              output.AppendLine(split + " == " + ans.ToString());
            }
          }
        }
      }
      catch (EvaluationException eEx)
      {
        responseColor = Discord.Color.Red;
        output.AppendLine("I can't evaluate your input: " + eEx.Message + ".");
      }
      catch (DivideByZeroException)
      {
        output.AppendLine("NaN.");
      }
      catch (OverflowException)
      {
        responseColor = Discord.Color.Red;
        output.AppendLine("Overflow.");
      }

      Response response = new Response
      {
        Embed = Utility.EmbedUtility.ToEmbed(output.ToString(), responseColor),
        Message = output.ToString(),
        ResponseType = ResponseType.Default
      };
      return new[] { response };
    }
  }
}