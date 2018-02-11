using SlateBot.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlateBot.Lifecycle
{
  class SlateBotControllerDisconnectedState : ISlateBotControllerLifecycleState
  {
    private readonly SlateBotControllerLifecycle lifecycle;

    public SlateBotControllerLifecycleStates StateId => SlateBotControllerLifecycleStates.Disconnected;

    public SlateBotControllerDisconnectedState(SlateBotControllerLifecycle lifecycle)
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
      return SlateBotControllerLifecycleStates.Connecting;
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

    public SlateBotControllerLifecycleStates OnMessageReadyToSend(object sender, IMessageDetail message)
    {
      lifecycle.ErrorLogger.LogError(new Error(ErrorCode.UnexpectedEvent, ErrorSeverity.Information, $"{nameof(SlateBotControllerDisconnectedState)} {nameof(OnMessageReadyToSend)}: Storing message."));
      return StateId;
    }
  }
}
