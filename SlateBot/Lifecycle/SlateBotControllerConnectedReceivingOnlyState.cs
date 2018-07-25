using Discord.WebSocket;
using SlateBot.Errors;

namespace SlateBot.Lifecycle
{
  internal class SlateBotControllerConnectedReceivingOnlyState : ISlateBotControllerLifecycleState
  {
    private readonly SlateBotControllerLifecycle lifecycle;

    public SlateBotControllerLifecycleStates StateId => SlateBotControllerLifecycleStates.ConnectedReceivingOnly;

    public SlateBotControllerConnectedReceivingOnlyState(SlateBotControllerLifecycle lifecycle)
    {
      this.lifecycle = lifecycle;
    }

    public void OnEntry()
    {
    }

    public void OnExit()
    {
    }

    public SlateBotControllerLifecycleStates AttemptConnection()
    {
      lifecycle.ErrorLogger.LogError(new Error(ErrorCode.AlreadyConnected, ErrorSeverity.Error, $"{nameof(SlateBotControllerConnectedReceivingOnlyState)} {nameof(AttemptConnection)}"));
      return StateId;
    }

    public SlateBotControllerLifecycleStates Disconnect()
    {
      lifecycle.Client.LogoutAsync();
      return StateId;
    }

    public SlateBotControllerLifecycleStates OnConnection()
    {
      lifecycle.ErrorLogger.LogError(new Error(ErrorCode.UnexpectedEvent, ErrorSeverity.Error, $"{nameof(SlateBotControllerConnectedReceivingOnlyState)} {nameof(OnConnection)}"));
      return StateId;
    }

    public SlateBotControllerLifecycleStates OnDisconnection()
    {
      return SlateBotControllerLifecycleStates.Disconnected;
    }

    public SlateBotControllerLifecycleStates OnMessageReadyToSend(string message, ISocketMessageChannel destination)
    {
      lifecycle.ErrorLogger.LogError(new Error(ErrorCode.NotSendingMessage, ErrorSeverity.Debug, $"{nameof(SlateBotControllerConnectedReceivingOnlyState)} {nameof(OnMessageReadyToSend)} dest {destination.Id} message {message}"));
      return StateId;
    }
  }
}