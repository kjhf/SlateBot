using CsHelper;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SlateBot
{
  internal class SocketMessageWrapper : IMessageDetail
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

    public bool IsPrivate => Channel.IsPrivate();

    public string Message => socketMessage.Content;

    public ulong MessageId => socketMessage.Id;

    public bool HasGuild => Guild != null;

    public ulong GuildId => Guild?.Id ?? UserId;

    public string GuildName => Guild?.Name ?? Username;

    public ulong GuildOwnerId => Guild?.OwnerId ?? Constants.ErrorId;

    public string GuildOwnerName => Guild?.Owner?.Username;

    public string[] URLs { get; }

    public ulong UserId => User?.Id ?? Constants.ErrorId;

    public string Username => User?.Username;

    public string MentionUsername => "<@!" + UserId + ">";

    /// <summary>
    /// Construct a <see cref="SocketMessageWrapper"/> from a <see cref="SocketMessage"/>.
    /// </summary>
    /// <param name="e"></param>
    public SocketMessageWrapper(SocketMessage msg)
    {
      this.socketMessage = msg;

      List<string> urls = new List<string>();
      if (msg?.Application?.IconUrl != null)
      {
        urls.Add(msg.Application.IconUrl);
      }

      if (msg.Attachments != null && msg.Attachments.Count > 0)
      {
        foreach (var a in msg.Attachments)
        {
          if (!string.IsNullOrEmpty(a.Url))
          {
            urls.Add(a.Url);
          }
        }
      }

      if (msg.Embeds != null && msg.Embeds.Count > 0)
      {
        foreach (var e in msg.Embeds)
        {
          if (!string.IsNullOrEmpty(e.Url))
          {
            urls.Add(e.Url);
          }
        }
      }

      var urlMatches = WebHelper.URL_REGEX.Matches(msg.Content);
      foreach (Match m in urlMatches)
      {
        if (m.Success)
        {
          urls.Add(m.Value);
        }
      }

      URLs = urls.ToArray();
    }
  }
}