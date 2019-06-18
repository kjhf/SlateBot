using System;

namespace SlateBot.Commands.Schedule
{
  public class ScheduledMessageData
  {
    /// <summary>
    /// The channel id.
    /// </summary>
    public readonly ulong channelId;

    /// <summary>
    /// The id of the message (to refer to deletion/modification).
    /// </summary>
    public readonly ushort id;

    /// <summary>
    /// If the message is enabled.
    /// </summary>
    public bool enabled;

    /// <summary>
    /// The message text.
    /// </summary>
    public string message;

    /// <summary>
    /// Repeating every <see cref="TimeSpan"/>, or Zero for non-repeating.
    /// </summary>
    public TimeSpan repetitionTimeSpan = TimeSpan.Zero;

    /// <summary>
    /// The time this message is next due to fire.
    /// </summary>
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