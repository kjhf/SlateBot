using SlateBot.Language;
using SlateBot.SavedSettings;
using SlateBot.Utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SlateBot.Commands.Schedule
{
  public class ScheduleCommand : Command
  {
    private readonly LanguageHandler languageHandler;
    private readonly ServerSettingsHandler serverSettingsHandler;
    private readonly Scheduler.ScheduleHandler scheduleHandler;
    private readonly IAsyncResponder asyncResponder;

    internal ScheduleCommand(LanguageHandler languageHandler, ServerSettingsHandler serverSettingsHandler, Scheduler.ScheduleHandler scheduleHandler, IAsyncResponder asyncResponder, string[] aliases, string examples, string help, ModuleType module)
      : base(CommandHandlerType.Schedule, aliases, examples, help, module)
    {
      this.languageHandler = languageHandler ?? throw new ArgumentNullException(nameof(languageHandler));
      this.serverSettingsHandler = serverSettingsHandler ?? throw new ArgumentNullException(nameof(serverSettingsHandler));
      this.scheduleHandler = scheduleHandler ?? throw new ArgumentNullException(nameof(scheduleHandler));
      this.asyncResponder = asyncResponder ?? throw new ArgumentNullException(nameof(asyncResponder));
    }

    public override IList<Response> Execute(SenderSettings senderDetail, IMessageDetail args)
    {
      StringBuilder output = new StringBuilder();

      if ((Constants.IsBotOwner(args.UserId)) || (args.GuildOwnerId == args.UserId))
      {
        ServerSettings serverSettings = senderDetail.ServerSettings;
        IEnumerable<ScheduledMessageData> scheduledData = serverSettings.ScheduledMessages.Where(d => d.channelId == args.ChannelId);
        int scheduledDataLength = scheduledData.Count();
        CommandMessageHelper command = new CommandMessageHelper(serverSettings.CommandSymbol, args.Message);
        string[] commandParams = command.CommandParams;

        switch (commandParams.Length)
        {
          case 0: 
            // Should not occur.
            output.AppendLine($"{Emojis.CrossSymbol} {languageHandler.GetPhrase(senderDetail.ServerSettings.Language, "Error_Oops")}");
            break;

          case 1:
            // Show help.
            output.AppendLine(Help);
            output.AppendLine(Examples);
            output.AppendLine(BuildScheduledMessageInformation(scheduledData, languageHandler.GetCultureInfo(serverSettings.Language), serverSettings.Language));
            break;

          case 2:
          {
            // Remove without an id?
            if (commandParams[1].Equals("remove", StringComparison.OrdinalIgnoreCase))
            {
              switch (scheduledDataLength)
              {
                case 0:
                {
                  // No messages set up
                  output.AppendLine($"{Emojis.CrossSymbol} {languageHandler.GetPhrase(senderDetail.ServerSettings.Language, "Error_NoResults")}");
                  break;
                }

                case 1:
                {
                  // Only one, remove that
                  ScheduledMessageData matchedData = serverSettings.ScheduledMessages.Single(d => d.channelId == args.ChannelId);
                  AttemptDisable(serverSettings, matchedData, output, senderDetail.ServerSettings.Language);

                  // And remove
                  serverSettings.ScheduledMessages.Remove(matchedData);
                  serverSettingsHandler.WriteServerSettings(serverSettings);
                  break;
                }

                default:
                {
                  // More than one, require an id.
                  output.AppendLine($"{Emojis.CrossSymbol} {languageHandler.GetPhrase(senderDetail.ServerSettings.Language, "Error_IncorrectParameter")}: {Constants.BotMention} {command.CommandLower} remove _id_");
                  output.AppendLine(BuildScheduledMessageInformation(scheduledData, languageHandler.GetCultureInfo(serverSettings.Language), serverSettings.Language));
                  break;
                }
              }
            }
            else if (commandParams[1].Equals("enable", StringComparison.OrdinalIgnoreCase))
            {
              switch (scheduledDataLength)
              {
                case 0:
                {
                  // No messages set up
                  output.AppendLine($"{Emojis.CrossSymbol} {languageHandler.GetPhrase(senderDetail.ServerSettings.Language, "Error_NoResults")}");
                  break;
                }

                case 1:
                {
                  // Only one, try to enable that
                  ScheduledMessageData matchedData = serverSettings.ScheduledMessages.Single(d => d.channelId == args.ChannelId);
                  AttemptEnable(serverSettings, matchedData, output, serverSettings.Language);
                  break;
                }

                default:
                {
                  // More than one, require an id.
                  output.AppendLine($"{Emojis.CrossSymbol} {languageHandler.GetPhrase(senderDetail.ServerSettings.Language, "Error_IncorrectParameter")}: {Constants.BotMention} {command.CommandLower} enable _id_");
                  output.AppendLine(BuildScheduledMessageInformation(scheduledData, languageHandler.GetCultureInfo(serverSettings.Language), serverSettings.Language));
                  break;
                }
              }
            }
            else if (commandParams[1].Equals("disable", StringComparison.OrdinalIgnoreCase))
            {
              switch (scheduledDataLength)
              {
                case 0:
                {
                  // No messages set up
                  output.AppendLine($"{Emojis.CrossSymbol} {languageHandler.GetPhrase(senderDetail.ServerSettings.Language, "Error_NoResults")}");
                  break;
                }

                case 1:
                {
                  // Only one, disable that
                  ScheduledMessageData matchedData = serverSettings.ScheduledMessages.Single(d => d.channelId == args.ChannelId);
                  AttemptDisable(serverSettings, matchedData, output, senderDetail.ServerSettings.Language);
                  break;
                }

                default:
                {
                  // More than one, require an id.
                  output.AppendLine($"{Emojis.CrossSymbol} {languageHandler.GetPhrase(senderDetail.ServerSettings.Language, "Error_IncorrectParameter")}: {Constants.BotMention} {command.CommandLower} enable _id_");
                  output.AppendLine(BuildScheduledMessageInformation(scheduledData, languageHandler.GetCultureInfo(serverSettings.Language), serverSettings.Language));
                  break;
                }
              }
            }
            else if (commandParams[1].Equals("list", StringComparison.OrdinalIgnoreCase))
            {
              string list = BuildScheduledMessageInformation(scheduledData, languageHandler.GetCultureInfo(serverSettings.Language), serverSettings.Language);
              if (list == string.Empty)
              {
                output.AppendLine("<none>");
              }
              else
              {
                output.AppendLine(list);
              }
            }
            else if (commandParams[1].Equals("set", StringComparison.OrdinalIgnoreCase))
            {
              output.AppendLine($"{Constants.BotMention} {command.CommandLower} set _id_ <Due|Repeating|Message|Enabled> <_value_>");
              output.AppendLine(BuildScheduledMessageInformation(scheduledData, languageHandler.GetCultureInfo(serverSettings.Language), serverSettings.Language));
            }
            else if (commandParams[1].Equals("new", StringComparison.OrdinalIgnoreCase))
            {
              ushort id = 1;
              var takenIds = serverSettings.ScheduledMessages.Select(d => d.id).ToArray();
              while (takenIds.Contains(id))
              {
                ++id;
              }
              serverSettings.ScheduledMessages.Insert(id - 1, new ScheduledMessageData(args.ChannelId, id));
              output.AppendLine($"{Constants.BotMention} {command.CommandLower} set **{id}** <Due|Repeating|Message|Enabled> <_value_>");
              output.AppendLine(BuildScheduledMessageInformation(scheduledData, languageHandler.GetCultureInfo(serverSettings.Language), serverSettings.Language));
            }


            break;
          }

          case 3:
          {
            bool idParsed = ushort.TryParse(commandParams[2], out ushort id);

            if (!idParsed)
            {
              // Id not in range
              output.AppendLine($"{Emojis.CrossSymbol} {languageHandler.GetPhrase(senderDetail.ServerSettings.Language, "Error_IncorrectParameter")}: {id}");
              output.AppendLine(BuildScheduledMessageInformation(scheduledData, languageHandler.GetCultureInfo(serverSettings.Language), serverSettings.Language));
            }
            else
            {
              var matchedData = serverSettings.ScheduledMessages.Find(m => m.id == id);
              if (matchedData != default(ScheduledMessageData))
              {
                if (commandParams[1].Equals("set", StringComparison.OrdinalIgnoreCase))
                {
                  output.AppendLine($"{Constants.BotMention} {command.CommandLower} set _id_ <Due|Repeating|Message|Enabled> <value>");
                  output.AppendLine(BuildScheduledMessageInformation(scheduledData, languageHandler.GetCultureInfo(serverSettings.Language), serverSettings.Language));
                }
                else if (commandParams[1].Equals("enable", StringComparison.OrdinalIgnoreCase))
                {
                  AttemptEnable(serverSettings, matchedData, output, serverSettings.Language);
                }
                else if (commandParams[1].Equals("disable", StringComparison.OrdinalIgnoreCase))
                {
                  AttemptDisable(serverSettings, matchedData, output, serverSettings.Language);
                }
                else
                {
                  // Remove with an id?
                  if (commandParams[1].Equals("remove", StringComparison.OrdinalIgnoreCase))
                  {
                    if (!scheduledData.Any())
                    {
                      output.AppendLine($"{Emojis.CrossSymbol} {languageHandler.GetPhrase(senderDetail.ServerSettings.Language, "Error_NoResults")}");
                    }
                    else
                    {
                      if (id > 0 && id <= scheduledDataLength)
                      {
                        AttemptDisable(serverSettings, matchedData, output, senderDetail.ServerSettings.Language);

                        // And remove
                        serverSettings.ScheduledMessages.Remove(matchedData);
                        serverSettingsHandler.WriteServerSettings(serverSettings);
                        output.AppendLine(BuildScheduledMessageInformation(scheduledData, languageHandler.GetCultureInfo(serverSettings.Language), serverSettings.Language));
                      }
                    }
                  }
                }
              }
              else
              {
                // Id not found
                output.AppendLine($"{Emojis.CrossSymbol} {languageHandler.GetPhrase(senderDetail.ServerSettings.Language, "Error_IncorrectParameter")}: {id}");
                output.AppendLine(BuildScheduledMessageInformation(scheduledData, languageHandler.GetCultureInfo(serverSettings.Language), serverSettings.Language));
              }
            }
            break;
          }

          // 4 or more
          default:
          {
            bool idParsed = uint.TryParse(commandParams[2], out uint id);

            if (!idParsed)
            {
              // Id not in range
              output.AppendLine($"{Emojis.CrossSymbol} {languageHandler.GetPhrase(senderDetail.ServerSettings.Language, "Error_IncorrectParameter")}: {id}");
              output.AppendLine(BuildScheduledMessageInformation(scheduledData, languageHandler.GetCultureInfo(serverSettings.Language), serverSettings.Language));
            }
            else
            {
              var matchedData = serverSettings.ScheduledMessages.Find(m => m.id == id);
              if (matchedData != default(ScheduledMessageData))
              {
                if (commandParams[1].Equals("set", StringComparison.OrdinalIgnoreCase))
                {
                  // Due|Repeating|Message|Enabled
                  if (commandParams[3].Equals("Due", StringComparison.OrdinalIgnoreCase))
                  {
                    string dueStr = string.Join(" ", commandParams.Skip(4));
                    if (string.IsNullOrWhiteSpace(dueStr))
                    {
                      output.AppendLine($"{Emojis.InfoSymbol} Due is the next time to send the message e.g. 5:30pm, 00:00");
                    }
                    else
                    {
                      bool success = DateTime.TryParse(dueStr, languageHandler.GetCultureInfo(serverSettings.Language), DateTimeStyles.AssumeUniversal, out matchedData.nextDue);
                      if (success)
                      {
                        matchedData.nextDue = matchedData.nextDue.ToUniversalTime();
                        while (matchedData.nextDue < DateTime.UtcNow)
                        {
                          matchedData.nextDue = matchedData.nextDue.AddDays(1);
                        }
                        string localisedToGo = languageHandler.GetLocalisedTimeSpan(serverSettings.Language, matchedData.nextDue - DateTime.UtcNow);
                        output.AppendLine($"{Emojis.TimerSymbol} {matchedData.nextDue.ToString("t")} UTC ({localisedToGo})");
                        serverSettingsHandler.WriteServerSettings(serverSettings);
                      }
                      else
                      {
                        output.AppendLine($"{Emojis.CrossSymbol} {languageHandler.GetPhrase(senderDetail.ServerSettings.Language, "Error_IncorrectParameter")}: {dueStr}");
                      }
                    }
                  }
                  else if (commandParams[3].Equals("Repeating", StringComparison.OrdinalIgnoreCase))
                  {
                    string repeatingStr = string.Join(" ", commandParams.Skip(4));
                    if (string.IsNullOrWhiteSpace(repeatingStr))
                    {
                      output.AppendLine($"{Emojis.InfoSymbol} Repeating is the period to wait between each message, or 0 to not repeat. In form _days_ or _days_._hours_:_minutes_:_seconds_ or _hours_:_minutes_:_seconds_");
                    }
                    else
                    {
                      bool success = TimeSpan.TryParse(repeatingStr, languageHandler.GetCultureInfo(senderDetail.ServerSettings.Language), out matchedData.repetitionTimeSpan);
                      if (success)
                      {
                        TimeSpan ts = matchedData.repetitionTimeSpan;
                        string localisedTs = languageHandler.GetLocalisedTimeSpan(serverSettings.Language, ts);
                        output.AppendLine($"{Emojis.RepeatSymbol} {localisedTs}");
                        serverSettingsHandler.WriteServerSettings(serverSettings);
                      }
                      else
                      {
                        output.AppendLine($"{Emojis.CrossSymbol} {languageHandler.GetPhrase(senderDetail.ServerSettings.Language, "Error_IncorrectParameter")}: {repeatingStr}");
                      }
                    }
                  }
                  else if (commandParams[3].Equals("Message", StringComparison.OrdinalIgnoreCase))
                  {
                    string messageStr = string.Join(" ", commandParams.Skip(4));
                    if (string.IsNullOrWhiteSpace(messageStr))
                    {
                      output.AppendLine($"{Emojis.InfoSymbol} Message is the message to write.");
                    }
                    else
                    {
                      matchedData.message = messageStr;
                      output.AppendLine($"{Emojis.MessageUnicode} {messageStr}");
                      serverSettingsHandler.WriteServerSettings(serverSettings);
                    }
                  }
                  else if (commandParams[3].Equals("Enabled", StringComparison.OrdinalIgnoreCase))
                  {
                    string enabledStr = string.Join(" ", commandParams.Skip(4));
                    if (string.IsNullOrWhiteSpace(enabledStr))
                    {
                      output.AppendLine($"{Emojis.InfoSymbol} Enabled is if the message should be enabled (true/false). Long hand for the enable/disable command.");
                    }
                    else
                    {
                      if (enabledStr.Equals("true", StringComparison.OrdinalIgnoreCase) || enabledStr.Equals("yes", StringComparison.OrdinalIgnoreCase) || enabledStr.Equals("y", StringComparison.OrdinalIgnoreCase))
                      {
                        AttemptEnable(serverSettings, matchedData, output, senderDetail.ServerSettings.Language);
                        serverSettingsHandler.WriteServerSettings(serverSettings);
                      }
                      else if (enabledStr.Equals("false", StringComparison.OrdinalIgnoreCase) || enabledStr.Equals("no", StringComparison.OrdinalIgnoreCase) || enabledStr.Equals("n", StringComparison.OrdinalIgnoreCase))
                      {
                        AttemptDisable(serverSettings, matchedData, output, senderDetail.ServerSettings.Language);
                        serverSettingsHandler.WriteServerSettings(serverSettings);
                      }
                      else
                      {
                        output.AppendLine($"{Emojis.CrossSymbol} {languageHandler.GetPhrase(senderDetail.ServerSettings.Language, "Error_IncorrectParameter")}: {enabledStr}");
                      }
                    }
                  }
                  else
                  {
                    output.AppendLine($"{Emojis.CrossSymbol} {languageHandler.GetPhrase(senderDetail.ServerSettings.Language, "Error_IncorrectParameter")}: {commandParams[3]}");
                    output.AppendLine($"{Constants.BotMention} {command.CommandLower} set _id_ <Due|Repeating|Message|Enabled> [value]");
                  }
                }
              }
              else
              {
                // Id not found
                output.AppendLine($"{Emojis.CrossSymbol} {languageHandler.GetPhrase(senderDetail.ServerSettings.Language, "Error_IncorrectParameter")}: {id}");
                output.AppendLine(BuildScheduledMessageInformation(scheduledData, languageHandler.GetCultureInfo(serverSettings.Language), serverSettings.Language));
              }
            }

            break;
          }
        }
      }
      else
      {
        output.AppendLine($"{Emojis.CrossSymbol} {languageHandler.GetPhrase(senderDetail.ServerSettings.Language, "Error_ServerOwnerOnly")}");
      }
      
      return new[] { new Response
      {
        Message = output.ToString(),
        ResponseType = ResponseType.Default
      }};
    }
    
    private void AttemptEnable(ServerSettings s, ScheduledMessageData m, StringBuilder output, Languages language)
    {
      if (string.IsNullOrWhiteSpace(m.message))
      {
        output.AppendLine($"{Emojis.CrossSymbol} {languageHandler.GetPhrase(language, "Error_IncorrectParameter")}: Message?");
      }
      else if (m.nextDue == default(DateTime))
      {
        output.AppendLine($"{Emojis.CrossSymbol} {languageHandler.GetPhrase(language, "Error_IncorrectParameter")}: Start time?");
      }
      else if (m.enabled)
      {
        output.AppendLine($"{Emojis.TickSymbol} {languageHandler.GetPhrase(language, "Error_IncorrectParameter")}: Already enabled!");
      }
      else
      {
        bool valid = scheduleHandler.StartJob(m, () =>
        {
          asyncResponder.SendResponseAsync(m.channelId, new Response { Embed = null, Message = m.message, ResponseType = ResponseType.Default });

          // Save enabled/disabled and due time.
          serverSettingsHandler.WriteServerSettings(s);
        });
        if (valid)
        {
          m.enabled = true;
          output.AppendLine($"{Emojis.TickSymbol}");
        }
        else
        {
          output.AppendLine($"{Emojis.CrossSymbol} {languageHandler.GetPhrase(language, "Error_IncorrectParameter")}: error in timer data.");
        }
      }
    }

    private void AttemptDisable(ServerSettings s, ScheduledMessageData matchedData, StringBuilder output, Languages language)
    {
      if (!matchedData.enabled)
      {
        // Already disabled
      }
      else
      {
        matchedData.enabled = false;
        scheduleHandler.StopJob(matchedData);

        // Save enabled/disabled and due time.
        serverSettingsHandler.WriteServerSettings(s);
      }
      output.AppendLine($"{Emojis.TickSymbol}");
    }

    /// <summary>
    /// Builds the list of scheduled message data as a localised string.
    /// Returns empty if none.
    /// </summary>
    /// <param name="scheduledMessageData"></param>
    /// <param name="culture"></param>
    /// <param name="language"></param>
    /// <returns></returns>
    private string BuildScheduledMessageInformation(IEnumerable<ScheduledMessageData> scheduledMessageData, CultureInfo culture, Languages language)
    {
      StringBuilder output = new StringBuilder();
      foreach (var data in scheduledMessageData)
      {
        if (data.nextDue != default(DateTime))
        {
          if (data.Repeating)
          {
            string localisedRep = languageHandler.GetLocalisedTimeSpan(language, data.repetitionTimeSpan);
            string localisedToGo = languageHandler.GetLocalisedTimeSpan(language, data.nextDue - DateTime.UtcNow);
            output.Append($"[{data.id}] {Emojis.RepeatSymbol} {localisedRep}, {Emojis.TimerSymbol} {data.nextDue.ToString("U", culture.DateTimeFormat)} UTC ({localisedToGo})");
          }
          else
          {
            string localisedToGo = languageHandler.GetLocalisedTimeSpan(language, data.nextDue - DateTime.UtcNow);
            output.Append($"[{data.id}] {Emojis.TimerSymbol} {data.nextDue.ToString("U", culture.DateTimeFormat)} UTC ({localisedToGo})");
          }
        }
        else
        {
          output.Append($"[{data.id}] {Emojis.TimerSymbol} <not set>");
        }

        if (!string.IsNullOrWhiteSpace(data.message))
        {
          output.Append(", ");

          bool truncated = data.message.Length > 40;
          if (truncated)
          {
            output.Append(data.message.Truncate(20, "..."));
            output.Append(data.message.Substring(data.message.Length - 15));
          }
          else
          {
            output.Append(data.message);
          }
        }
        output.AppendLine();
      }
      return output.ToString();
    }
  }
}