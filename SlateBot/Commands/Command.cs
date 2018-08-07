using System;
using System.Collections.Generic;

namespace SlateBot.Commands
{
  public abstract class Command
  {
    /// <summary> Random generator </summary>
    protected static readonly Random rand = new Random();

    /// <summary> Array of aliases this command accepts. </summary>
    public abstract string[] Aliases { get; }

    /// <summary> The command's handling controller type. </summary>
    public abstract CommandHandlerType CommandHandlerType { get; }

    /// <summary> Examples for this command </summary>
    public abstract string Examples { get; }

    /// <summary> Extra Data as a string dictionary for this command </summary>
    public abstract List<KeyValuePair<string, string>> ExtraData { get; }

    /// <summary> What to display if the sender requests help for this command </summary>
    public abstract string Help { get; }

    /// <summary> The command's module. </summary>
    public abstract ModuleType Module { get; }

    /// <summary> How the handler should use this command's response. </summary>
    public abstract ResponseType ResponseType { get; }

    /// <summary>
    /// Handle an incoming command and return a string as a result.
    /// If the string is null or empty, no message will be sent.
    /// </summary>
    /// <param name="args">Context</param>
    /// <returns>The message to reply with.</returns>
    public abstract string Execute(SenderSettings sender, IMessageDetail args);

    /// <summary>
    /// Overridden ToString. Returns the <see cref="Module"/> and its aliases.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      return Module + ": { " + string.Join(", ", Aliases) + " }";
    }
  }
}