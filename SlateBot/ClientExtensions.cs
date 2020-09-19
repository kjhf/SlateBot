using Discord;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SlateBot
{
  public static class Extensions
  {
    public static async Task<IMessageChannel> GetMessageChannelAsync(this IDiscordClient client, ulong id) => client == null ? null : (await client.GetChannelAsync(id).ConfigureAwait(false) as IMessageChannel);

    public static async Task<IVoiceChannel> GetVoiceChannelAsync(this IDiscordClient client, ulong id) => client == null ? null : (await client.GetChannelAsync(id).ConfigureAwait(false) as IVoiceChannel);

    public static ISocketMessageChannel GetMessageChannel(this DiscordSocketClient client, ulong id) => client?.GetChannel(id) as ISocketMessageChannel;

    public static ISocketPrivateChannel GetPrivateChannel(this DiscordSocketClient client, ulong id) => client?.GetChannel(id) as ISocketPrivateChannel;

    public static ISocketAudioChannel GetVoiceChannel(this DiscordSocketClient client, ulong id) => client?.GetChannel(id) as ISocketAudioChannel;

    public static Task<ISocketMessageChannel> GetMessageChannelAsync(this DiscordSocketClient client, ulong id) => Task.FromResult(client?.GetMessageChannel(id));

    public static Task<ISocketPrivateChannel> GetPrivateChannelAsync(this DiscordSocketClient client, ulong id) => Task.FromResult(client?.GetPrivateChannel(id));

    public static Task<ISocketAudioChannel> GetVoiceChannelAsync(this DiscordSocketClient client, ulong id) => Task.FromResult(client?.GetVoiceChannel(id));

    public static Task<SocketGuild> GetGuildAsync(this DiscordSocketClient client, ulong id) => Task.FromResult(client?.GetGuild(id));

    public static bool IsMentioningMe(this IMessage message) => message == null ? false : message.MentionedUserIds.Contains(Constants.BotId);

    public static async Task EditAsync(this IUserMessage message, string newMessage, Embed newEmbed = null)
    {
      await message?.ModifyAsync((e) => { e.Content = newMessage; e.Embed = newEmbed; });
    }

    public static async Task<IUserMessage> SendPrivateMessageAsync(this DiscordSocketClient client, IUser recipient, string text, bool isTTS = false, Embed embed = null, RequestOptions options = null)
    {
      if (string.IsNullOrWhiteSpace(text)) { return (null); }

      return await recipient?.SendMessageAsync(text, isTTS, embed, options);
    }

    public static async Task<IUserMessage> SendPrivateMessageAsync(this DiscordSocketClient client, ulong recipientId, string text, bool isTTS = false, Embed embed = null, RequestOptions options = null)
    {
      if (string.IsNullOrWhiteSpace(text)) { return (null); }

      IUser recipient = client?.GetUser(recipientId);
      return await recipient?.SendMessageAsync(text, isTTS, embed, options);
    }

    public static async Task<IUserMessage> SendMessageAsync(this IUser u, string text, bool isTTS = false, Embed embed = null, RequestOptions options = null)
    {
      if (string.IsNullOrWhiteSpace(text)) { return (null); }

      var channel = await u.GetOrCreateDMChannelAsync();
      return await channel.SendMessageAsync(text, isTTS, embed, options);
    }

    public static bool IsPrivate(this IChannel c)
    {
      return (c is IPrivateChannel);
    }

    /// <summary>
    /// Gets the Guild of the channel, or null if the channel does not support guilds.
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    public static IGuild GetGuild(this IChannel c)
    {
      try
      {
        dynamic d = (dynamic)c;
        return d.Guild;
      }
      catch (Exception)
      {
        return null;
      }
    }
  }
}