using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlateBot
{
  public class ConsoleMessageDetail : IMessageDetail
  {
    public const string ConsoleConstant = "CONSOLE";

    public ulong ChannelId => Constants.ConsoleId;

    public string ChannelName => ConsoleConstant;

    public string Message { get; private set; }

    public ulong MessageId => Constants.ConsoleId;

    public bool HasGuild => false;

    public ulong GuildId => Constants.ConsoleId;

    public string GuildName => "";

    public ulong GuildOwnerId => Constants.ConsoleId;

    public string GuildOwnerName => "";

    public ulong UserId => Constants.ConsoleId;

    public string Username => ConsoleConstant;

    public string MentionUsername => ConsoleConstant;

    public bool IsPrivate => true;

    /// <summary>
    /// Construct a <see cref="ConsoleMessageDetail"/> with the message.
    /// </summary>
    /// <param name="message"></param>
    public ConsoleMessageDetail(string message)
    {
      this.Message = message;
    }
  }
}
