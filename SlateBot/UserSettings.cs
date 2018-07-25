﻿using SlateBot.Commands;
using SlateBot.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlateBot
{
  public class UserSettings
  {
    /// <summary>
    /// The user id this file belongs to
    /// </summary>
    public ulong UserId { get; } = Constants.ErrorId;

    /// <summary>
    /// The stats about the user, such as achievements and commands issued.
    /// </summary>
    public UserStats UserStats { get; }

    // TODO - bot permissions

    /// <summary>
    /// Construct an empty user settings.
    /// </summary>
    /// <param name="userId"></param>
    public UserSettings(ulong userId)
       : this(userId, new UserStats())
    {
    }

    /// <summary>
    /// Construct a user settings with pre-established values.
    /// </summary>
    /// <param name="userId"></param>
    public UserSettings(ulong userId, UserStats userStats)
    {
      this.UserId = userId;
      this.UserStats = userStats;
    }
  }
}
