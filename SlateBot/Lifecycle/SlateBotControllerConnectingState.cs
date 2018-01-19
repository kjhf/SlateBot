using SlateBot.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlateBot.Lifecycle
{
  class SlateBotControllerConnectingState : ISlateBotControllerLifecycleState
  {
    private readonly SlateBotControllerLifecycle lifecycle;

    public SlateBotControllerLifecycleStates StateId => SlateBotControllerLifecycleStates.Connecting;

    public SlateBotControllerConnectingState(SlateBotControllerLifecycle lifecycle)
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
      lifecycle.ErrorLogger.LogError(new Error(ErrorCode.UnexpectedEvent, ErrorSeverity.Error, $"{nameof(SlateBotControllerConnectingState)} {nameof(AttemptConnection)}"));
      return StateId;
    }

    public SlateBotControllerLifecycleStates OnConnection()
    {
      return SlateBotControllerLifecycleStates.ConnectedTransceiving;
    }

    public SlateBotControllerLifecycleStates OnDisconnection()
    {
      return SlateBotControllerLifecycleStates.Disconnected;
    }

    public SlateBotControllerLifecycleStates OnMessageReadyToSend(object sender, IMessageDetail message)
    {
      lifecycle.ErrorLogger.LogError(new Error(ErrorCode.UnexpectedEvent, ErrorSeverity.Information, $"{nameof(SlateBotControllerConnectingState)} {nameof(OnMessageReadyToSend)}: Storing message."));
      return StateId;
    }

    public SlateBotControllerLifecycleStates OnMessageReceived(object sender, IMessageDetail message)
    {
      lifecycle.ErrorLogger.LogError(new Error(ErrorCode.UnexpectedEvent, ErrorSeverity.Warning, $"{nameof(SlateBotControllerConnectingState)} {nameof(OnMessageReceived)}"));
      return StateId;
    }
  }
}
