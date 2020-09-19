using System;
using System.Collections.Generic;
using System.Linq;

namespace SlateBot.Commands
{
  internal struct CommandMessageHelper
  {
    /// <summary> The message this helper represents. </summary>
    private readonly string message;

    /// <summary> The full command, excluding the command symbol or bot mention if specified. </summary>
    public readonly string FullCommandExcludingCommandPrefix;

    /// <summary> The command symbol(s) prefixing commands. </summary>
    public readonly string CommandSymbol;

    /// <summary>
    /// Get if this message is actually a command.
    /// </summary>
    public bool IsCommand => message.StartsWith(Constants.BotMention_Nick) || message.StartsWith(Constants.BotMention_NoNick) || message.StartsWith(Constants.AtUsername) || message.StartsWith(CommandSymbol);

    /// <summary> The full command as parameters delimited by space. The command is on [0]. </summary>
    public string[] CommandParams => FullCommandExcludingCommandPrefix.Split(' ');

    /// <summary> The command arguments only as a string enumerable. </summary>
    public IEnumerable<string> CommandArgs => CommandParams.Skip(1);

    /// <summary> The command only as a lower-case string. </summary>
    public string CommandLower => CommandParams[0].ToLower();

    /// <summary> The command arguments only as a string. </summary>
    public string CommandDetail => FullCommandExcludingCommandPrefix.Substring(CommandLower.Length).Trim();

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

      // Remove the BotMention
      this.FullCommandExcludingCommandPrefix = message;

      if (FullCommandExcludingCommandPrefix.StartsWith(Constants.BotMention_NoNick))
      {
        FullCommandExcludingCommandPrefix = FullCommandExcludingCommandPrefix.Substring(Constants.BotMention_NoNick.Length).Trim();
      }
      if (FullCommandExcludingCommandPrefix.StartsWith(Constants.BotMention_Nick))
      {
        FullCommandExcludingCommandPrefix = FullCommandExcludingCommandPrefix.Substring(Constants.BotMention_Nick.Length).Trim();
      }

      // Remove the BotMention
      if (FullCommandExcludingCommandPrefix.StartsWith(Constants.AtUsername))
      {
        FullCommandExcludingCommandPrefix = FullCommandExcludingCommandPrefix.Substring(Constants.AtUsername.Length).Trim();
      }

      // Remove the command symbol
      if (FullCommandExcludingCommandPrefix.StartsWith(CommandSymbol))
      {
        FullCommandExcludingCommandPrefix = FullCommandExcludingCommandPrefix.Substring(CommandSymbol.Length).Trim();
      }
    }
  }
}