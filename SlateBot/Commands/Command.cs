using System;
using System.Collections.Generic;

namespace SlateBot.Commands
{
  public abstract class Command
  {
    /// <summary> Random generator </summary>
    protected static readonly Random rand = new Random();

    /// <summary>
    /// Base constructor - construct the common features across all commands.
    /// </summary>
    /// <param name="commandHandlerType"></param>
    /// <param name="module"></param>
    /// <param name="aliases"></param>
    /// <param name="examples"></param>
    /// <param name="help"></param>
    protected Command(CommandHandlerType commandHandlerType, string[] aliases, string examples, string help, ModuleType module)
    {
      this.CommandHandlerType = commandHandlerType;
      this.Aliases = aliases;
      this.Examples = examples;
      this.Help = help;
      this.Module = module;
    }

    /// <summary> Array of aliases this command accepts. </summary>
    public string[] Aliases { get; }

    /// <summary> The command's handling controller type. </summary>
    public CommandHandlerType CommandHandlerType { get; }

    /// <summary> Examples for this command </summary>
    public string Examples { get; }

    /// <summary> Extra Data as a string dictionary for this command </summary>
    public List<KeyValuePair<string, string>> ExtraData => ConstructExtraData();

    /// <summary> What to display if the sender requests help for this command </summary>
    public string Help { get; }

    /// <summary> The command's module. </summary>
    public ModuleType Module { get; }

    /// <summary>
    /// Handle an incoming command and return response object(s) as a result.
    /// The result may be null or empty to remain unhandled.
    /// </summary>
    /// <param name="sender">Settings context</param>
    /// <param name="args">Message context</param>
    /// <returns>The response objects to reply with.</returns>
    public abstract IList<Response> Execute(SenderSettings sender, IMessageDetail args);

    /// <summary>
    /// Overridden ToString. Returns the <see cref="Module"/> and its aliases.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      return Module + ": { " + string.Join(", ", Aliases) + " }";
    }

    /// <summary>
    /// Construct the extra data that this command uses. Return null for none.
    /// </summary>
    /// <returns></returns>
    protected virtual List<KeyValuePair<string, string>> ConstructExtraData()
    {
      // No extra data for the command by default.
      return null;
    }
  }
}