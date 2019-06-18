using SlateBot.Commands;
using SlateBot.Commands.Schedule;
using SlateBot.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlateBot
{
  public class ServerSettings
  {
    /// <summary>
    /// The Blocked Modules that this server disallows.
    /// </summary>
    public HashSet<ModuleType> BlockedModules { get; internal set; } = new HashSet<ModuleType>();

    /// <summary>
    /// The Command Symbol for this server
    /// </summary>
    public string CommandSymbol { get; internal set; } = "!";

    /// <summary>
    /// The Join/Quit message channel id in this server
    /// </summary>
    public ulong JoinQuitChannelId { get; internal set; } = Constants.ErrorId;

    /// <summary>
    /// Join messages
    /// </summary>
    public List<string> JoinServerMessages { get; internal set; } = new List<string>();

    /// <summary>
    /// The server's language
    /// </summary>
    public Languages Language { get; internal set; } = Languages.English;

    /// <summary>
    /// Quit messages
    /// </summary>
    public List<string> QuitServerMessages { get; internal set; } = new List<string>();

    /// <summary>
    /// Channels that the bot will add ratings to
    /// </summary>
    public HashSet<ulong> RateChannels { get; internal set; } = new HashSet<ulong>();

    /// <summary>
    /// Messages that are scheduled on this server
    /// </summary>
    public List<ScheduledMessageData> ScheduledMessages { get; internal set; } = new List<ScheduledMessageData>();

    /// <summary>
    /// Channels that announce Splatoon 2 rotations
    /// </summary>
    public HashSet<ulong> Splatoon2RotationChannels { get; internal set; } = new HashSet<ulong>();

    //public IList<RSSChannelData> RSSFeedChannels { get; internal set; } = new List<RSSChannelData>();

    /// <summary>
    /// The server id this file belongs to
    /// </summary>
    public ulong ServerId { get; internal set; } = Constants.ErrorId;

    /// <summary>
    /// Should the bot track deleted messages?
    /// </summary>
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
