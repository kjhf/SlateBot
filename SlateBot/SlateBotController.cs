using Discord;
using Discord.WebSocket;
using SlateBot.Commands;
using SlateBot.DAL;
using SlateBot.Errors;
using SlateBot.Events;
using SlateBot.Language;
using SlateBot.Lifecycle;
using SlateBot.SavedSettings;
using SlateBot.Scheduler;
using SlateBot.Utility;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

      if (isFromConsole || response.ResponseType == ResponseType.LogOnly)
      {
        // Log the result.
        string str = response.Message;
        if (response.FilePath != null)
        {
          str += "\n" + response.FilePath;
        }
        ErrorLogger.LogError(new Error(ErrorCode.ConsoleMessage, ErrorSeverity.Information, str));  
      }
      else if (response.ResponseType == ResponseType.None)
      {
        // Nothing to do.
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

          SendMessage(response, responseChannel);
        }
        else
        {
          ErrorLogger.LogError(new Error(ErrorCode.NotImplemented, ErrorSeverity.Error, $"Cannot reply to channel {message.ChannelName} ({message.ChannelId}) by user {message.Username} ({message.UserId}) as the message is not from the socket."));
        }
      }
    }

    public async Task SendResponseAsync(ulong channelId, Response response)
    {
      if (channelId == Constants.ConsoleId)
      {
        ErrorLogger.LogError(new Error(ErrorCode.ConsoleMessage, ErrorSeverity.Information, response.Message));
      }
      else
      {
        var channel = await client.GetMessageChannelAsync(channelId);
        SendMessage(response, channel);
      }
    }

    /// <summary>
    /// Handle a command received from chat.
    /// This is after filtering to make sure a message is a command.
    /// </summary>
    internal void HandleCommandReceived(SenderSettings senderSettings, IMessageDetail message)
    {
      var responses = commandHandlerController.ExecuteCommand(senderSettings, message);

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
          sum.AddReactionAsync(new Emoji(Emojis.CheckUnicode));
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

      // If the message has a file and was sent to us in private, save that file.
      if (socketMessage.IsPrivate)
      {
        if (socketMessage.socketMessage.Attachments.Any() || socketMessage.socketMessage.Embeds.Any())
        {
          Task.Run(async () =>
          {
            foreach (var attachment in socketMessage.socketMessage.Attachments)
            {
              var result = await HTTPHelper.DownloadFile(attachment.Url);
              if (result != null && result.Item2 != null)
              {
                await File.WriteAllBytesAsync(Path.Combine(dal.receivedFilesFolder, attachment.Filename), result.Item2);
              }
            }
            foreach (var embed in socketMessage.socketMessage.Embeds)
            {
              if (embed.Image.HasValue)
              {
                var image = (EmbedImage)embed.Image;
                var result = await HTTPHelper.DownloadFile(image.Url);
                if (result != null && result.Item2 != null)
                {
                  await File.WriteAllBytesAsync(Path.Combine(dal.receivedFilesFolder, DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss-fff")), result.Item2);
                }
              }
            }
          });
        }
      }
    }

    internal void SendMessage(Response message, ulong channelId)
    {
      lifecycle.OnMessageReadyToSend(message, (IMessageChannel)client.GetChannel(channelId));
    }

    internal void SendMessage(Response message, IMessageChannel channel)
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
      await SendResponseAsync(args.message, args.response);
    }
  }
}