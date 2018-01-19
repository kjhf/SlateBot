using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlateBot
{
  class SocketMessageWrapper : IMessageDetail
  {
    /// <summary>
    /// The <see cref="SocketMessage"/> used to construct this wrapper.
    /// </summary>
    public readonly SocketMessage socketMessage;
    public SocketUser User => socketMessage.Author;
    public SocketChannel Channel => socketMessage.Channel as SocketChannel;
    public SocketGuild Guild => (Channel as SocketGuildChannel)?.Guild;

    public ulong ChannelId => Channel?.Id ?? UserId;

    public string ChannelName => socketMessage.Channel?.Name ?? Username;

    public string Message => socketMessage.Content;

    public ulong MessageId => socketMessage.Id;

    public bool HasGuild => Guild != null;

    public ulong GuildId => Guild?.Id ?? UserId;

    public string GuildName => Guild?.Name ?? Username;

    public ulong GuildOwnerId => Guild?.OwnerId ?? Constants.ErrorId;

    public string GuildOwnerName => Guild?.Owner?.Username;

    public ulong UserId => User?.Id ?? Constants.ErrorId;

    public string Username => User?.Username;

    public string MentionUsername => "<@" + UserId + ">";

    /// <summary>
    /// Construct a <see cref="SocketMessageWrapper"/> from a <see cref="SocketMessage"/>.
    /// </summary>
    /// <param name="e"></param>
    public SocketMessageWrapper(SocketMessage e)
    {
      this.socketMessage = e;
    }
  }
}
