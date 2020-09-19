using SlateBot.Commands.Schedule;
using System;

namespace SlateBot.Scheduler
{
  internal class ScheduledTask
  {
    /// <summary>
    /// The function to execute when the job timer elapses
    /// </summary>
    public readonly Action executeAction;

    /// <summary>
    /// The job's schedule data
    /// </summary>
    public readonly ScheduledMessageData scheduledMessageData;

    /// <summary>
    /// Construct a <see cref="ScheduledTask"/> with its data and elapsed action.
    /// </summary>
    /// <param name="scheduledMessageData"></param>
    /// <param name="executeAction"></param>
    public ScheduledTask(ScheduledMessageData scheduledMessageData, Action executeAction)
    {
      this.scheduledMessageData = scheduledMessageData ?? throw new ArgumentNullException(nameof(scheduledMessageData));
      this.executeAction = executeAction ?? throw new ArgumentNullException(nameof(executeAction));
    }

    /// <summary>
    /// Get if this job is enabled.
    /// </summary>
    internal bool IsEnabled => scheduledMessageData.enabled;

    /// <summary>
    /// Execute this job.
    /// </summary>
    internal void Execute()
    {
      // Fire action if (still) enabled
      if (scheduledMessageData.enabled)
      {
        executeAction?.Invoke();

        // Repeating?
        if (scheduledMessageData.Repeating)
        {
          // Set next due time
          scheduledMessageData.nextDue += scheduledMessageData.repetitionTimeSpan;
        }
        else
        {
          // If not repeating, now disable.
          scheduledMessageData.enabled = false;
        }
      }
    }

    /// <summary>
    /// Disable this job.
    /// </summary>
    internal void Disable()
    {
      scheduledMessageData.enabled = false;
    }

    /// <summary>
    /// Enable this job.
    /// </summary>
    internal void Enable()
    {
      scheduledMessageData.enabled = true;
    }
  }
}