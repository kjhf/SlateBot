using Newtonsoft.Json;
using System;

namespace SlateBot.Commands.Schedule
{
  [Serializable]
  [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
  public class ScheduledMessageData
  {
    /// <summary>
    /// The channel id.
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    public readonly ulong channelId;

    /// <summary>
    /// The id of the message (to refer to deletion/modification).
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    public readonly ushort id;

    /// <summary>
    /// If the message is enabled.
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    public bool enabled;

    /// <summary>
    /// The message text.
    /// </summary>
    [JsonProperty(Required = Required.Default)]
    public string message;

    /// <summary>
    /// Repeating every <see cref="TimeSpan"/>, or Zero for non-repeating.
    /// </summary>
    [JsonProperty(Required = Required.Default)]
    public TimeSpan repetitionTimeSpan = TimeSpan.Zero;

    /// <summary>
    /// The time this message is next due to fire.
    /// </summary>
    [JsonProperty(Required = Required.Default)]
    public DateTime nextDue;

    /// <summary>
    /// Get the name of the message data.
    /// </summary>
    public string Name => channelId + "_" + id;

    /// <summary>
    /// Construct a <see cref="ScheduledMessageData"/> with its channel and id.
    /// </summary>
    /// <param name="channelId"></param>
    /// <param name="id"></param>
    public ScheduledMessageData(ulong channelId, ushort id)
    {
      this.channelId = channelId;
      this.id = id;
    }

    /// <summary>
    /// If the message is repeating.
    /// </summary>
    public bool Repeating => repetitionTimeSpan != TimeSpan.Zero;

    /// <summary>
    /// Overridden ToString - returns the name of the message data.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      return Name;
    }
  }
}