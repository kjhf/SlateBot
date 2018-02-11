namespace SlateBot.Lifecycle
{
  /// <summary>
  /// A lifecycle state is a sub-class of the lifecycle that handles events.
  /// </summary>
  internal interface ISlateBotControllerLifecycleState
  {
    /// <summary>
    /// Get the state id of the state
    /// </summary>
    SlateBotControllerLifecycleStates StateId { get; }

    /// <summary>
    /// Event when the state is entered.
    /// </summary>
    void OnEntry();

    /// <summary>
    /// Event when the state is left.
    /// </summary>
    void OnExit();

    /// <summary>
    /// Event raised when the controller requests connection.
    /// </summary>
    /// <returns>The desired new state</returns>
    SlateBotControllerLifecycleStates AttemptConnection();

    /// <summary>
    /// Event raised when the controller has connected.
    /// </summary>
    /// <returns>The desired new state</returns>
    SlateBotControllerLifecycleStates OnConnection();

    /// <summary>
    /// Event raised when the controller has disconnected.
    /// </summary>
    /// <returns>The desired new state</returns>
    SlateBotControllerLifecycleStates OnDisconnection();
    
    /// <summary>
    /// Event raised when the controller is ready to send a message.
    /// </summary>
    /// <param name="sender">The sender object</param>
    /// <param name="message">The message received detail</param>
    /// <returns>The desired new state</returns>
    SlateBotControllerLifecycleStates OnMessageReadyToSend(object sender, IMessageDetail message);
  }
}