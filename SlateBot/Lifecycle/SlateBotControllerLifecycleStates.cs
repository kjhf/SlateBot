namespace SlateBot.Lifecycle
{
  /// <summary>
  /// States for the lifecycle.
  /// </summary>
  internal enum SlateBotControllerLifecycleStates
  {
    /// <summary>
    /// The controller is disconnected
    /// </summary>
    Disconnected,

    /// <summary>
    /// The controller is attempting connection
    /// </summary>
    Connecting,

    /// <summary>
    /// The controller is connected and is sending and receiving messages
    /// </summary>
    ConnectedTransceiving,

    /// <summary>
    /// The controller is connected but is only receiving messages
    /// </summary>
    ConnectedReceivingOnly,

    //

    /// <summary>
    /// The number of states defined.
    /// </summary>
    NumberOfStates
  }
}