using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SlateBot.Scheduler
{
  internal class ScheduleHandler
  {
    private readonly Dictionary<string, ScheduledTask> scheduled = new Dictionary<string, ScheduledTask>();
    private readonly Dictionary<string, Task> timers = new Dictionary<string, Task>();

    public ScheduleHandler()
    {
    }

    public void LoadScheduledTasks(IEnumerable<ServerSettings> settings, SavedSettings.ServerSettingsHandler serverSettingsHandler, IAsyncResponder asyncResponder)
    {
      foreach (var s in settings)
      {
        foreach (Commands.Schedule.ScheduledMessageData m in s.ScheduledMessages)
        {
          if (m.enabled)
          {
            StartJob(m, () =>
            {
              asyncResponder.SendResponseAsync(m.channelId, new Commands.Response { Embed = null, Message = m.message, ResponseType = Commands.ResponseType.Default });

              // Save enabled/disabled and due time.
              serverSettingsHandler.WriteServerSettings(s);
            });
          }
        }
      }
    }
    
    public bool StartJob(Commands.Schedule.ScheduledMessageData m, Action callback)
    {
      var job = new ScheduledTask(m, callback);
      return Schedule(m.Name, job);
    }

    public void StopJob(Commands.Schedule.ScheduledMessageData m)
    {
      string name = m.Name;
      if (scheduled.TryGetValue(name, out ScheduledTask s))
      {
        s.Disable();
        scheduled.Remove(name);
        timers.Remove(name);
      }
    }

    private bool Schedule(string name, ScheduledTask job)
    {
      bool result = true;
      var m = job.scheduledMessageData;
      int delayStartSeconds = 0;

      if (m.nextDue == default(DateTime))
      {
        // Next due not set. Fail.
        result = false;
      }
      else
      {
        // If we've missed the next time that the job is scheduled for:
        if (m.nextDue < DateTime.UtcNow)
        {
          // Add on the repetition period until we're back on track.
          delayStartSeconds = 0;
          while (m.nextDue < DateTime.UtcNow)
          {
            m.nextDue += m.repetitionTimeSpan;
          }
        }
        else
        {
          // Otherwise just delay until ready.
          delayStartSeconds = (int)((m.nextDue - DateTime.UtcNow).TotalSeconds);
        }
        scheduled.Add(name, job);
        RunJobAfterDelay(name, job, delayStartSeconds);
      }
      return result;
    }

    private void RunJobAfterDelay(string jobName, ScheduledTask job, int delaySeconds)
    {
      job.Enable();
      timers.Add(jobName, Task.Run(() =>
      {
        Task.Delay(TimeSpan.FromSeconds(delaySeconds));
        while (job.IsEnabled)
        {
          Task.Delay(job.scheduledMessageData.repetitionTimeSpan);
          job.Execute();
        }
      }));
    }
  }
}