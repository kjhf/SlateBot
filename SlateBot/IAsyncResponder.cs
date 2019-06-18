using SlateBot.Events;
using System.Threading.Tasks;

namespace SlateBot
{
  public interface IAsyncResponder
  {
    Task SendResponseAsync(IMessageDetail message, Commands.Response response);
    Task SendResponseAsync(ulong channelId, Commands.Response response);
  }
}