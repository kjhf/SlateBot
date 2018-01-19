using SlateBot.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlateBot.Lifecycle
{
  class SlateBotControllerConnectedReceivingOnlyState : ISlateBotControllerLifecycleState
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
      lifecycle.ErrorLogger.LogError(new Error(ErrorCode.UnexpectedEvent, ErrorSeverity.Error, $"{nameof(SlateBotControllerConnectedReceivingOnlyState)} {nameof(AttemptConnection)}"));
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

    public SlateBotControllerLifecycleStates OnMessageReadyToSend(object sender, IMessageDetail message)
    {
      lifecycle.ErrorLogger.LogError(new Error(ErrorCode.NotSendingMessage, ErrorSeverity.Debug, $"{nameof(SlateBotControllerConnectedReceivingOnlyState)} {nameof(OnMessageReadyToSend)}"));
      return StateId;
    }

    public SlateBotControllerLifecycleStates OnMessageReceived(object sender, IMessageDetail message)
    {
      lifecycle.ErrorLogger.LogError(new Error(ErrorCode.UnexpectedEvent, ErrorSeverity.Information, $"{nameof(SlateBotControllerConnectedReceivingOnlyState)} {nameof(OnMessageReceived)}"));
      return StateId;
    }
  }
}
