using SlateBot.Events;
using System.Threading.Tasks;

namespace SlateBot
{
  public interface IAsyncResponder
  {
    Task SendResponseAsync(CommandReceivedEventArgs args);
  }
}