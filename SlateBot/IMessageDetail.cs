namespace SlateBot
{
  internal interface IMessageDetail
  {
    ulong ChannelId { get; }
    string ChannelName { get; }
    string Message { get; }
    ulong MessageId { get; }
    bool HasGuild { get; }
    ulong GuildId { get; }
    string GuildName { get; }
    ulong GuildOwnerId { get; }
    string GuildOwnerName { get; }
    ulong UserId { get; }
    string Username { get; }
    string MentionUsername { get; }
  }
}