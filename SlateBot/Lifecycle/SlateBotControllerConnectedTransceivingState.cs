using Discord;
using SlateBot.Commands;
using SlateBot.Errors;
using System;
using System.Threading.Tasks;

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
      // Attempt to send all pending messages.
      Tuple<Response, IMessageChannel>[] pendingMessages = lifecycle.GetPendingMessages();
      foreach (var tuple in pendingMessages)
      {
        lifecycle.OnMessageReadyToSend(tuple.Item1, tuple.Item2);
      }
    }

    public void OnExit()
    {
    }

    public SlateBotControllerLifecycleStates OnMessageReadyToSend(Commands.Response message, Discord.IMessageChannel destination)
    {
      bool tts = message.ResponseType == ResponseType.Default_TTS;
      if (message.ResponseType == Commands.ResponseType.PleaseWaitMessage)
      {
        // We need to run this asynchronously.
        Task.Run(async () =>
        {
          try
          {
            Discord.IUserMessage m;
            if (message.Embed == null)
            {
              if (message.FilePath == null)
              {
                m = await destination.SendMessageAsync(message.Message);
              }
              else
              {
                m = await destination.SendFileAsync(message.FilePath, message.Message);
              }
            }
            else
            {
              if (message.FilePath == null)
              {
                m = await destination.SendMessageAsync("", false, message.Embed.Build());
              }
              else
              {
                m = await destination.SendFileAsync(message.FilePath, "", false, message.Embed.Build());
              }
            }
            lifecycle.PleaseWaitHandler.PushToStack(m);
          }
          catch (Exception ex)
          {
            lifecycle.ErrorLogger.LogException(ex, ErrorSeverity.Error);
          }
        });
      }
      else
      {
        // This can be fired and forgotten.
        if (message.Embed == null)
        {
          if (message.FilePath == null)
          {
            destination.SendMessageAsync(message.Message, tts);
          }
          else
          {
            destination.SendFileAsync(message.FilePath, message.Message, tts);
          }
        }
        else
        {
          if (message.FilePath == null)
          {
            destination.SendMessageAsync("", tts, message.Embed.Build());
          }
          else
          {
            destination.SendFileAsync(message.FilePath, "", tts, message.Embed.Build());
          }
        }
      }
      
      return StateId;
    }
  }
}