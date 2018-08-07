using SlateBot.Commands;
using System;

namespace SlateBot.Events
{
  public delegate void CommandReceived(object sender, CommandReceivedEventArgs args);

  public class CommandReceivedEventArgs : EventArgs
  {
    public readonly IMessageDetail message;
    public readonly Response response;
    public readonly SenderSettings senderSettings;

    public CommandReceivedEventArgs(SenderSettings senderDetail, IMessageDetail message, Response response)
    {
      this.senderSettings = senderDetail ?? throw new ArgumentNullException(nameof(senderDetail));
      this.message = message ?? throw new ArgumentNullException(nameof(message));
      this.response = response ?? throw new ArgumentNullException(nameof(response));
    }
  }
}