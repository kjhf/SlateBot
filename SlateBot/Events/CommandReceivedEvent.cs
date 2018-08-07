using SlateBot.Commands;
using System;

namespace SlateBot.Events
{
  public delegate void CommandReceived(object sender, CommandReceivedEventArgs args);

  public class CommandReceivedEventArgs : EventArgs
  {
    public readonly Command command;
    public readonly IMessageDetail message;
    public readonly string response;
    public readonly SenderSettings senderSettings;

    public CommandReceivedEventArgs(SenderSettings senderDetail, IMessageDetail message, Command command, string response)
    {
      this.senderSettings = senderDetail ?? throw new ArgumentNullException(nameof(senderDetail));
      this.message = message ?? throw new ArgumentNullException(nameof(message));
      this.command = command ?? throw new ArgumentNullException(nameof(command));
      this.response = response ?? throw new ArgumentNullException(nameof(response));
    }
  }
}