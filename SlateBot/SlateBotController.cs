using CsHelper;
using Discord;
using Discord.WebSocket;
using SlateBot.Commands;
using SlateBot.DAL;
using SlateBot.Errors;
using SlateBot.Events;
using SlateBot.Imaging;
using SlateBot.Language;
using SlateBot.Lifecycle;
using SlateBot.SavedSettings;
using SlateBot.Scheduler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SlateBot
{
  /// <summary>
  /// The <see cref="SlateBotController"/> handles the main part of the program.
  /// </summary>
  internal class SlateBotController : IController, IAsyncResponder
  {
    internal readonly DiscordSocketClient client;
    internal readonly CommandController commandHandlerController;
    internal readonly SlateBotDAL dal;
    internal readonly LanguageHandler languageHandler;
    internal readonly ScheduleHandler scheduleHandler;
    internal readonly ServerSettingsHandler serverSettingsHandler;
    private readonly UpdateController updateController;
    internal readonly PleaseWaitHandler waitHandler;
    private readonly SlateBotControllerLifecycle lifecycle;
    private readonly UserSettingsHandler userSettingsHandler;
    private bool dumpDebug;

    /// <summary>
    /// Get if the Bot is connected.
    /// </summary>
    public bool Connected => lifecycle.Connected;

    public SlateBotController()
    {
      this.dal = new SlateBotDAL();
      this.languageHandler = new LanguageHandler(dal);
      this.lifecycle = new SlateBotControllerLifecycle(this);
      this.scheduleHandler = new ScheduleHandler();
      this.commandHandlerController = new CommandController(this);
      this.serverSettingsHandler = new ServerSettingsHandler(ErrorLogger, dal);
      this.updateController = new UpdateController(this);
      this.userSettingsHandler = new UserSettingsHandler(this);
      this.waitHandler = new PleaseWaitHandler(serverSettingsHandler, languageHandler);

      this.client = new DiscordSocketClient();
      client.LoggedIn += Client_LoggedIn;
      client.LoggedOut += Client_LoggedOut;
      client.MessageReceived += Client_MessageReceived;
      client.MessageUpdated += Client_MessageUpdated;

      OnCommandReceived += SlateBotController_OnCommandReceived;
    }

    internal event CommandReceived OnCommandReceived;

    internal ErrorLogger ErrorLogger => dal.errorLogger;

    public void Connect()
    {
      lifecycle.AttemptConnection();
    }

    public void Disconnect()
    {
      lifecycle.Disconnect();
    }

    public void Initialise()
    {
      this.dal.Initialise();
      this.languageHandler.Initialise();
      this.commandHandlerController.Initialise();
      this.serverSettingsHandler.Initialise();
      this.updateController.Initialise();
      this.userSettingsHandler.Initialise();
      this.scheduleHandler.LoadScheduledTasks(serverSettingsHandler.ServerSettings, serverSettingsHandler, this);
      this.waitHandler.Initialise();

      ErrorLogger.LogDebug("Program initialised successfully.");
    }

    public void ToggleDumpDebug()
    {
      dumpDebug = !dumpDebug;
    }

    public async Task SendResponseAsync(IMessageDetail message, Response response)
    {
      bool isFromConsole = message is ConsoleMessageDetail;
      bool isFromSocket = message is SocketMessageWrapper;

      if (response.ResponseType == ResponseType.None)
      {
        // Nothing to do.
      }
      else if (isFromConsole || response.ResponseType == ResponseType.LogOnly)
      {
        // Log the result.
        if (response.ResponseType == ResponseType.Default_TTS)
        {
          Debug.Assert(isFromConsole);
          response.Message = "[TTS] " + response.Message;
        }
        if (response.FilePath != null)
        {
          response.Message += "\n" + response.FilePath;
        }
        SendMessageToConsole(response);
      }
      else
      {
        if (isFromSocket)
        {
          SocketMessageWrapper socketMessageWrapper = (SocketMessageWrapper)message;

          // If private response, get the DM channel to the user, otherwise use the current channel.
          IMessageChannel responseChannel =
            response.ResponseType == ResponseType.Private ?
              (await socketMessageWrapper.User.GetOrCreateDMChannelAsync()) :
              (IMessageChannel)socketMessageWrapper.Channel;

          if (response.ResponseType != ResponseType.Default_React)
          {
            SendMessage(response, responseChannel);
          }
          else
          {
            await socketMessageWrapper.socketMessage.AddReactionAsync(new Emoji(response.Message)).ConfigureAwait(false);
          }
        }
        else
        {
          ErrorLogger.LogError(new Error(ErrorCode.NotImplemented, ErrorSeverity.Error, $"Cannot reply to channel {message.ChannelName} ({message.ChannelId}) by user {message.Username} ({message.UserId}) as the message is not from the socket."));
        }
      }
    }

    public bool SendResponse(ulong channelId, Response response)
    {
      if (channelId == Constants.ConsoleId)
      {
        SendMessageToConsole(response);
      }
      else
      {
        var channel = client.GetMessageChannel(channelId);
        if (channel == null)
        {
          return false;
        }
        SendMessage(response, channel);
      }
      return true;
    }

    public async Task SendResponseAsync(ulong channelId, Response response)
    {
      if (channelId == Constants.ConsoleId)
      {
        SendMessageToConsole(response);
      }
      else
      {
        var channel = await client.GetMessageChannelAsync(channelId).ConfigureAwait(false);
        SendMessage(response, channel);
      }
    }

    private void SendMessageToConsole(Response response)
    {
      if (response.Embed?.Color != null)
      {
        ConsoleColor cc = ImageManipulator.ToConsoleColor(System.Drawing.Color.FromArgb(response.Embed.Color.Value.R, response.Embed.Color.Value.G, response.Embed.Color.Value.B));
        ErrorLogger.LogError(new Error(ErrorCode.ConsoleMessage, ErrorSeverity.Information, new Tuple<string, ConsoleColor>(response.Message, cc)));
      }
      else
      {
        ErrorLogger.LogError(new Error(ErrorCode.ConsoleMessage, ErrorSeverity.Information, response.Message));
      }
    }

    /// <summary>
    /// Handle a command received from chat.
    /// This is after filtering to make sure a message is a command.
    /// </summary>
    internal void HandleCommandReceived(SenderSettings senderSettings, IMessageDetail message)
    {
      bool success;
      IList<Response> responses;
      try
      {
        responses = commandHandlerController.ExecuteCommand(senderSettings, message);
        success = true;
      }
      catch (Exception ex)
      {
        responses = new List<Response>
        {
          new Response { Message = $"{Emojis.ExclamationSymbol} {languageHandler.GetPhrase(senderSettings.ServerSettings.Language, "Error_Oops")}: {ex.Message}" }
        };
        success = false;
      }

      if (dumpDebug)
      {
        var sb = MiscUtility.DumpObject(message);
        ErrorLogger.LogDebug("Handled " + message.GetType() + ": \r\n" + sb, true);
      }

      if (responses.Count > 0)
      {
        foreach (Response response in responses)
        {
          OnCommandReceived?.Invoke(this, new CommandReceivedEventArgs(senderSettings, message, response));
          /// Handled by <see cref="SlateBotController_OnCommandReceived"/>
          /// and the <see cref="UserSettingsHandler"/>
          if (dumpDebug)
          {
            var sb = MiscUtility.DumpObject(response);
            ErrorLogger.LogDebug("Response " + response.GetType() + ": \r\n" + sb, true);
          }
        }

        if ((message is SocketMessageWrapper smw) && (smw.socketMessage is SocketUserMessage sum))
        {
          // Help out if we're debugging.
          if (Debugger.IsAttached)
          {
            var sb = MiscUtility.DumpObject(sum);
            ErrorLogger.LogDebug("Handled " + sum.GetType() + ": \r\n" + sb, dumpDebug);
          }

          // React to the message handled.
          sum.AddReactionAsync(new Emoji(success ? Emojis.CheckUnicode : Emojis.CrossSymbol)).ConfigureAwait(false);
        }
      }
      else if (message is ConsoleMessageDetail) // If from the console
      {
        ErrorLogger.LogDebug("I got nothin'. (" + message.Message + ")", true);
      }
    }

    internal void HandleConsoleCommand(string line)
    {
      ServerSettings consoleServerSettings = serverSettingsHandler.GetOrCreateServerSettings(Constants.ConsoleId);
      UserSettings consoleUserSettings = userSettingsHandler.GetOrCreateUserSettings(Constants.ConsoleId);
      HandleCommandReceived(new SenderSettings(consoleServerSettings, consoleUserSettings), new ConsoleMessageDetail(line, SlateBotProgram.ConsoleIsPrivate));
    }

    internal void HandleMessageReceived(SocketMessageWrapper socketMessage)
    {
      // Don't respond to bots (includes us!)
      if (socketMessage.User.IsBot)
      {
        return;
      }

      // First, get the server settings for the guild this message is from.
      ServerSettings serverSettings = serverSettingsHandler.GetOrCreateServerSettings(socketMessage.GuildId);

      // And the user settings for the user this message is from.
      UserSettings userSettings = userSettingsHandler.GetOrCreateUserSettings(socketMessage.UserId);

      // Handle the command
      HandleCommandReceived(new SenderSettings(serverSettings, userSettings), socketMessage);
    }

    private void SendMessage(Response message, IMessageChannel channel)
    {
      lifecycle.OnMessageReadyToSend(message, channel);
    }

    private Task Client_LoggedIn()
    {
      lifecycle.OnConnection();

      if (Debugger.IsAttached)
      {
        client.SetGameAsync("~ Debugger Connected ~", null, ActivityType.Streaming);
      }

      return Task.CompletedTask;
    }

    private Task Client_LoggedOut()
    {
      lifecycle.OnDisconnection();
      return Task.CompletedTask;
    }

    private Task Client_MessageReceived(SocketMessage arg)
    {
      HandleMessageReceived(new SocketMessageWrapper(arg));
      return Task.CompletedTask;
    }

    private Task Client_MessageUpdated(Cacheable<IMessage, ulong> arg1, SocketMessage arg2, ISocketMessageChannel arg3)
    {
      if (arg2 != null)
      {
        HandleMessageReceived(new SocketMessageWrapper(arg2));
      }
      return Task.CompletedTask;
    }

    private async void SlateBotController_OnCommandReceived(object sender, CommandReceivedEventArgs args)
    {
      await SendResponseAsync(args.message, args.response).ConfigureAwait(false);
    }
  }
}