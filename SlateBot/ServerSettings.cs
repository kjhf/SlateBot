using Newtonsoft.Json;
using SlateBot.Commands;
using SlateBot.Commands.Schedule;
using SlateBot.Language;
using System;
using System.Collections.Generic;

namespace SlateBot
{
  [Serializable]
  [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
  public class ServerSettings
  {
    /// <summary>
    /// The Blocked Modules that this server disallows.
    /// </summary>
    [JsonProperty(Required = Required.Default)]
    public HashSet<ModuleType> BlockedModules { get; internal set; } = new HashSet<ModuleType>();

    /// <summary>
    /// The Command Symbol for this server
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    public string CommandSymbol { get; internal set; } = "!";

    /// <summary>
    /// The Join/Quit message channel id in this server
    /// </summary>
    [JsonProperty(Required = Required.Default)]
    public ulong? JoinQuitChannelId { get; internal set; } = null;

    /// <summary>
    /// Join messages
    /// </summary>
    [JsonProperty(Required = Required.Default)]
    public List<string> JoinServerMessages { get; internal set; } = new List<string>();

    /// <summary>
    /// The server's language
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    public Languages Language { get; internal set; } = Languages.English;

    /// <summary>
    /// Quit messages
    /// </summary>
    [JsonProperty(Required = Required.Default)]
    public List<string> QuitServerMessages { get; internal set; } = new List<string>();

    /// <summary>
    /// Channels that the bot will add ratings to
    /// </summary>
    [JsonProperty(Required = Required.Default)]
    public HashSet<ulong> RateChannels { get; internal set; } = new HashSet<ulong>();

    /// <summary>
    /// Messages that are scheduled on this server
    /// </summary>
    [JsonProperty(Required = Required.Default)]
    public List<ScheduledMessageData> ScheduledMessages { get; internal set; } = new List<ScheduledMessageData>();

    /// <summary>
    /// Channels that announce Splatoon 2 rotations
    /// </summary>
    [JsonProperty(Required = Required.Default)]
    public HashSet<ulong> Splatoon2RotationChannels { get; internal set; } = new HashSet<ulong>();

    //public IList<RSSChannelData> RSSFeedChannels { get; internal set; } = new List<RSSChannelData>();

    /// <summary>
    /// The server id this file belongs to
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    public ulong ServerId { get; } = Constants.ErrorId;

    /// <summary>
    /// Should the bot track deleted messages?
    /// </summary>
    [JsonProperty(Required = Required.Default)]
    public bool TrackDeletedMessages { get; internal set; } = false;

    /// <summary>
    /// Construct an empty server settings.
    /// </summary>
    /// <param name="serverId"></param>
    public ServerSettings(ulong serverId)
    {
      this.ServerId = serverId;
    }
  }
}