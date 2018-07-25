using SlateBot.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlateBot.Events
{
  public delegate void CommandReceived(object sender, CommandReceivedEventArgs args);

  public class CommandReceivedEventArgs : EventArgs
  {
    public readonly SenderDetail senderDetail;
    public readonly IMessageDetail message;
    public readonly Command command;
    public readonly string response;

    public CommandReceivedEventArgs(SenderDetail senderDetail, IMessageDetail message, Command command, string response)
    {
      this.senderDetail = senderDetail ?? throw new ArgumentNullException(nameof(senderDetail));
      this.message = message ?? throw new ArgumentNullException(nameof(message));
      this.command = command ?? throw new ArgumentNullException(nameof(command));
      this.response = response ?? throw new ArgumentNullException(nameof(response));
    }
  }
}
