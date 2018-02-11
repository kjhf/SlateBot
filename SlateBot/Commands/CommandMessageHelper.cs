using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlateBot.Commands
{
  struct CommandMessageHelper
  {
    private readonly string message;

    /// <summary> The command symbol(s) prefixing commands. </summary>
    public readonly string CommandSymbol;
    
    /// <summary> The full command, excluding the command symbol if specified. </summary>
    public string FullCommand
    {
      get
      {
        string fullCommand = (message.StartsWith(Constants.BotMention) ? (message.Substring(Constants.BotMention.Length)) : message).Trim();
        fullCommand = (fullCommand.StartsWith(CommandSymbol) ? (fullCommand.Substring(CommandSymbol.Length)) : fullCommand).Trim();
        return fullCommand;
      }
    }

    /// <summary> The full command as parameters delimited by space. The command is on [0]. </summary>
    public string[] CommandParams => FullCommand.Split(' ');

    /// <summary> The command arguments only as a string enumerable. </summary>
    public IEnumerable<string> CommandArgs => CommandParams.Skip(1);

    /// <summary> The command only as a lower-case string. </summary>
    public string CommandLower => CommandParams[0].ToLower();

    /// <summary> The command arguments only as a string. </summary>
    public string CommandDetail => FullCommand.Substring(CommandLower.Length).Trim();

    /// <summary> <see cref="CommandDetail"/> escaped as a URI string. </summary>
    public string EscapedCommandDetail => Uri.EscapeDataString(CommandDetail).Replace("+", "%2B");

    /// <summary>
    /// Construct a <see cref="CommandMessageHelper"/> with a given message.
    /// </summary>
    /// <param name="commandSymbol">The command symbol denoting if the message is a command</param>
    /// <param name="message">The message</param>
    public CommandMessageHelper(string commandSymbol, string message)
    {
      this.message = message;
      this.CommandSymbol = commandSymbol;
    }
  }
}
