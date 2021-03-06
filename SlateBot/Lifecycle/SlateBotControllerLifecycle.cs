﻿using Discord;
using Discord.WebSocket;
using SlateBot.Commands;
using SlateBot.Errors;
using System;
using System.Collections.Generic;

namespace SlateBot.Lifecycle
{
  internal class SlateBotControllerLifecycle
  {
    internal PleaseWaitHandler PleaseWaitHandler => controller.waitHandler;
    internal IErrorLogger ErrorLogger => controller.ErrorLogger;
    internal DiscordSocketClient Client => controller.client;

    /// <summary>
    /// Get if the lifecycle is in one of its connected states.
    /// </summary>
    internal bool Connected => CurrentStateId == SlateBotControllerLifecycleStates.ConnectedReceivingOnly || CurrentStateId == SlateBotControllerLifecycleStates.ConnectedTransceiving;

    /// <summary>
    /// The states of the lifecycle.
    /// </summary>
    /// <remarks>
    /// The states should be in order of the <see cref="SlateBotControllerLifecycleStates"/> enumeration. </remarks>
    private readonly ISlateBotControllerLifecycleState[] states;

    private readonly object lifecycleLock;
    private ISlateBotControllerLifecycleState CurrentState => states[(int)CurrentStateId];
    private readonly SlateBotController controller;
    private readonly List<Tuple<Response, IMessageChannel>> pendingMessages = new List<Tuple<Response, IMessageChannel>>();
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

    internal void StorePendingMessage(Response message, IMessageChannel destination)
    {
      pendingMessages.Add(new Tuple<Response, IMessageChannel>(message, destination));
    }

    internal Tuple<Response, IMessageChannel>[] GetPendingMessages()
    {
      lock (pendingMessages)
      {
        var arr = pendingMessages.ToArray();
        pendingMessages.Clear();
        return arr;
      }
    }

    /// <summary>
    /// Event raised when the controller requests connection.
    /// </summary>
    /// <remarks>(Lifecycle)</remarks>
    internal void AttemptConnection()
    {
      lock (lifecycleLock)
      {
        CurrentStateId = CurrentState.AttemptConnection();
      }
    }

    /// <summary>
    /// Event raised when the controller requests disconnection.
    /// </summary>
    /// <remarks>(Lifecycle)</remarks>
    internal void Disconnect()
    {
      lock (lifecycleLock)
      {
        CurrentStateId = CurrentState.Disconnect();
      }
    }

    /// <summary>
    /// Event raised when the controller has connected.
    /// </summary>
    /// <remarks>(Lifecycle)</remarks>
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
    /// <param name="message">The message to send</param>
    /// <param name="destination">The destination</param>
    internal void OnMessageReadyToSend(Commands.Response message, Discord.IMessageChannel destination)
    {
      lock (lifecycleLock)
      {
        CurrentStateId = CurrentState.OnMessageReadyToSend(message, destination);
      }
    }

    internal void HandleMessageReceived(SocketMessageWrapper socketMessage)
    {
      controller.HandleMessageReceived(socketMessage);
    }
  }
}