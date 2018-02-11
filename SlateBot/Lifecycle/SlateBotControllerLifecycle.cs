﻿using SlateBot.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SlateBot.Lifecycle
{
  class SlateBotControllerLifecycle
  {
    internal IErrorLogger ErrorLogger => controller.ErrorLogger;

    /// <summary>
    /// The states of the lifecycle.
    /// </summary>
    /// <remarks>
    /// The states should be in order of the <see cref="SlateBotControllerLifecycleStates"/> enumeration. </remarks>
    private readonly ISlateBotControllerLifecycleState[] states;

    private readonly object lifecycleLock;
    private ISlateBotControllerLifecycleState CurrentState => states[(int)CurrentStateId];
    private readonly SlateBotController controller;
    private SlateBotControllerLifecycleStates currentStateId;
    private SlateBotControllerLifecycleStates CurrentStateId
    {
      get => currentStateId;
      set
      {
        if (value != currentStateId)
        {
          CurrentState.OnExit();
          ErrorLogger.LogError(new Error(ErrorCode.Success, ErrorSeverity.Debug, $"{nameof(SlateBotControllerLifecycle)}: Transitioning from {CurrentStateId} to {value}."));
          currentStateId = value;
          CurrentState.OnEntry();
        }
      }
    }

    /// <summary>
    /// Constructor for the <see cref="SlateBotControllerLifecycle"/>.
    /// </summary>
    /// <param name="controller"></param>
    public SlateBotControllerLifecycle(SlateBotController controller)
    {
      this.lifecycleLock = new object();
      this.controller = controller;
      this.states = new ISlateBotControllerLifecycleState[(int)SlateBotControllerLifecycleStates.NumberOfStates]
      {
        new SlateBotControllerDisconnectedState(this),
        new SlateBotControllerConnectingState(this),
        new SlateBotControllerConnectedTransceivingState(this),
        new SlateBotControllerConnectedReceivingOnlyState(this)
      };
      currentStateId = SlateBotControllerLifecycleStates.Disconnected;
      CurrentState.OnEntry();
    }

    /// <summary>
    /// Event raised when the controller requests connection.
    /// </summary>
    /// <remarks>(Lifecycle)</remarks>
    /// <returns>The desired new state</returns>
    internal void AttemptConnection()
    {
      lock (lifecycleLock)
      {
        CurrentStateId = CurrentState.AttemptConnection();
      }
    }

    /// <summary>
    /// Event raised when the controller has connected.
    /// </summary>
    /// <remarks>(Lifecycle)</remarks>
    /// <returns>The desired new state</returns>
    internal void OnConnection()
    {
      lock (lifecycleLock)
      {
        CurrentStateId = CurrentState.OnConnection();
      }
    }

    /// <summary>
    /// Event raised when the controller has disconnected.
    /// </summary>
    /// <remarks>(Lifecycle)</remarks>
    /// <returns>The desired new state</returns>
    internal void OnDisconnection()
    {
      lock (lifecycleLock)
      {
        CurrentStateId = CurrentState.OnDisconnection();
      }
    }

    /// <summary>
    /// Event raised when the controller is ready to send a message.
    /// </summary>
    /// <remarks>(Lifecycle)</remarks>
    /// <param name="sender">The sender object</param>
    /// <param name="message">The message received detail</param>
    /// <returns>The desired new state</returns>
    internal void OnMessageReadyToSend(object sender, IMessageDetail message)
    {
      lock (lifecycleLock)
      {
        CurrentStateId = CurrentState.OnMessageReadyToSend(sender, message);
      }
    }

    internal void HandleMessageReceived(SocketMessageWrapper socketMessage)
    {
      controller.HandleMessageReceived(socketMessage);
    }
  }
}
