using SlateBot.Errors;

namespace SlateBot.Lifecycle
{
  internal class SlateBotControllerConnectedTransceivingState : ISlateBotControllerLifecycleState
  {
    private readonly SlateBotControllerLifecycle lifecycle;

    public SlateBotControllerConnectedTransceivingState(SlateBotControllerLifecycle lifecycle)
    {
      this.lifecycle = lifecycle;
    }

    public SlateBotControllerLifecycleStates StateId => SlateBotControllerLifecycleStates.ConnectedTransceiving;

    public SlateBotControllerLifecycleStates AttemptConnection()
    {
      lifecycle.ErrorLogger.LogError(new Error(ErrorCode.AlreadyConnected, ErrorSeverity.Error, $"{nameof(SlateBotControllerConnectedTransceivingState)} {nameof(AttemptConnection)}"));
      return StateId;
    }

    public SlateBotControllerLifecycleStates Disconnect()
    {
      lifecycle.Client.LogoutAsync();
      return StateId;
    }

    public SlateBotControllerLifecycleStates OnConnection()
    {
      lifecycle.ErrorLogger.LogError(new Error(ErrorCode.UnexpectedEvent, ErrorSeverity.Error, $"{nameof(SlateBotControllerConnectedTransceivingState)} {nameof(OnConnection)}"));
      return StateId;
    }

    public SlateBotControllerLifecycleStates OnDisconnection()
    {
      return SlateBotControllerLifecycleStates.Disconnected;
    }

    public void OnEntry()
    {
    }

    public void OnExit()
    {
    }

    public SlateBotControllerLifecycleStates OnMessageReadyToSend(Commands.Response message, Discord.IMessageChannel destination)
    {
      if (message.embed == null)
      {
        destination.SendMessageAsync(message.message);
      }
      else
      {
        destination.SendMessageAsync("", false, message.embed);
      }
      return StateId;
    }
  }
}