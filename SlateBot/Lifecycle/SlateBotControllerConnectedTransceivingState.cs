using Discord;
using Discord.WebSocket;
using SlateBot.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlateBot.Lifecycle
{
  class SlateBotControllerConnectedTransceivingState : ISlateBotControllerLifecycleState
  {
    private readonly SlateBotControllerLifecycle lifecycle;

    public SlateBotControllerLifecycleStates StateId => SlateBotControllerLifecycleStates.ConnectedTransceiving;

    public SlateBotControllerConnectedTransceivingState(SlateBotControllerLifecycle lifecycle)
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

    public SlateBotControllerLifecycleStates OnMessageReadyToSend(string message, ISocketMessageChannel destination)
    {
      destination.SendMessageAsync(message);
      return StateId;
    }
  }
}
