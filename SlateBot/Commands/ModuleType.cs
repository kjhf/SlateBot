using System;

namespace SlateBot.Commands
{
  [Serializable]
  public enum ModuleType
  {
    /// <summary>
    /// The catch-all or default group for commands that don't fit elsewhere.
    /// </summary>
    General,

    /// <summary>
    /// Magic 8-ball commands
    /// </summary>
    Magic8Balls,

    /// <summary>
    /// Translation and language commands
    /// </summary>
    Translation,

    /// <summary>
    /// Image manipulation commands
    /// </summary>
    Image,

    /// <summary>
    /// Number and arithmatic commands
    /// </summary>
    Number,

    /// <summary>
    /// Lookup and wiki commands
    /// </summary>
    Lookup,

    /// <summary>
    /// Memes and spam commands
    /// </summary>
    Memes,

    /// <summary>
    /// Adult and NSFW commands
    /// </summary>
    Adult,

    /// <summary>
    /// Commands restricted to the server admin
    /// </summary>
    ServerAdmin,

    /// <summary>
    /// Commands restricted to the bot admin
    /// </summary>
    BotAdmin
  }
}