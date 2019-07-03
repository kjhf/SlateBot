using SlateBot.Errors;

namespace SlateBot.Lifecycle
{
  internal class SlateBotControllerDisconnectedState : ISlateBotControllerLifecycleState
  {
    private readonly SlateBotControllerLifecycle lifecycle;

    public SlateBotControllerDisconnectedState(SlateBotControllerLifecycle lifecycle)
    {
      this.lifecycle = lifecycle;
    }

    public SlateBotControllerLifecycleStates StateId => SlateBotControllerLifecycleStates.Disconnected;

    public SlateBotControllerLifecycleStates AttemptConnection()
    {
      lifecycle.Client.LoginAsync(Discord.TokenType.Bot, Tokens.Token, true);
      lifecycle.Client.StartAsync();
      return SlateBotControllerLifecycleStates.Connecting;
    }

    public SlateBotControllerLifecycleStates Disconnect()
    {
      // Should already be logged out here.
      lifecycle.Client.LogoutAsync();
      return StateId;
    }

    public SlateBotControllerLifecycleStates OnConnection()
    {
      lifecycle.ErrorLogger.LogError(new Error(ErrorCode.UnexpectedEvent, ErrorSeverity.Error, $"{nameof(SlateBotControllerDisconnectedState)} {nameof(OnConnection)}"));
      return StateId;
    }

    public SlateBotControllerLifecycleStates OnDisconnection()
    {
      lifecycle.ErrorLogger.LogError(new Error(ErrorCode.UnexpectedEvent, ErrorSeverity.Error, $"{nameof(SlateBotControllerDisconnectedState)} {nameof(OnDisconnection)}"));
      return StateId;
    }

    public void OnEntry()
    {
    }

    public void OnExit()
    {
    }

    public SlateBotControllerLifecycleStates OnMessageReadyToSend(Commands.Response message, Discord.IMessageChannel destination)
    {
      lifecycle.ErrorLogger.LogError(new Error(ErrorCode.UnexpectedEvent, ErrorSeverity.Information, $"{nameof(SlateBotControllerDisconnectedState)} {nameof(OnMessageReadyToSend)}: Storing message."));
      lifecycle.StorePendingMessage(message, destination);
      return StateId;
    }
  }
}