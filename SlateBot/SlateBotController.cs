using Discord;
using Discord.WebSocket;
using SlateBot.Commands;
using SlateBot.DAL;
using SlateBot.Errors;
using SlateBot.Events;
using SlateBot.Language;
using SlateBot.Lifecycle;
using SlateBot.SavedSettings;
using SlateBot.Utility;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SlateBot
{
  /// <summary>
  /// The <see cref="SlateBotController"/> handles the main part of the program.
  /// </summary>
  internal class SlateBotController : IController
  {
    internal readonly CommandController commandHandlerController;
    internal readonly SlateBotDAL dal;
    internal readonly LanguageHandler languageHandler;
    internal readonly ServerSettingsHandler serverSettingsHandler;
    internal readonly DiscordSocketClient client;
    private readonly SlateBotControllerLifecycle lifecycle;
    private readonly UserSettingsHandler userSettingsHandler;

    public SlateBotController()
    {
      this.dal = new SlateBotDAL();
      this.languageHandler = new LanguageHandler(dal);
      this.lifecycle = new SlateBotControllerLifecycle(this);
      this.commandHandlerController = new CommandController(ErrorLogger, dal, languageHandler);
      this.serverSettingsHandler = new ServerSettingsHandler(ErrorLogger, dal);
      this.userSettingsHandler = new UserSettingsHandler(this);

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
      this.userSettingsHandler.Initialise();

      ErrorLogger.LogDebug("Program initialised successfully.");
    }

    /// <summary>
    /// Handle a command received from chat.
    /// This is after filtering to make sure a message is a command.
    /// </summary>
    internal void HandleCommandReceived(SenderSettings senderSettings, IMessageDetail message)
    {
      var responses = commandHandlerController.ExecuteCommand(senderSettings, message);

      if (responses.Count > 0)
      {
        foreach (var response in responses)
        {
          OnCommandReceived?.Invoke(this, new CommandReceivedEventArgs(senderSettings, message, response));
          /// Handled by <see cref="SlateBotController_OnCommandReceived"/>
          /// and the <see cref="UserSettingsHandler"/>
        }

        if ((message is SocketMessageWrapper smw) && (smw.socketMessage is SocketUserMessage sum))
        {
          // Help out if we're debugging.
          if (Debugger.IsAttached)
          {
            var sb = MiscUtility.DumpObject(sum);
            ErrorLogger.LogDebug("Handled " + sum.GetType() + ": \r\n" + sb);
          }

          // React to the message handled.
          sum.AddReactionAsync(new Emoji("✅"));
        }
      }
      else if (message is ConsoleMessageDetail) // If from the console
      {
        ErrorLogger.LogDebug("I got nothin'. (" + message.Message + ")", true);
      }
    }

    internal void HandleConsoleCommand(string line)
    {
      UserSettings consoleUserSettings = userSettingsHandler.GetOrCreateUserSettings(Constants.ConsoleId);
      HandleCommandReceived(new SenderSettings(serverSettingsHandler.consoleServerSettings, consoleUserSettings), new ConsoleMessageDetail(line));
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

    private Task Client_MessageUpdated(Discord.Cacheable<Discord.IMessage, ulong> arg1, SocketMessage arg2, ISocketMessageChannel arg3)
    {
      if (arg2 != null)
      {
        HandleMessageReceived(new SocketMessageWrapper(arg2));
      }
      return Task.CompletedTask;
    }

    private async void SlateBotController_OnCommandReceived(object sender, CommandReceivedEventArgs args)
    {
      bool isFromConsole = args.message is ConsoleMessageDetail;
      bool isFromSocket = args.message is SocketMessageWrapper;

      if (isFromConsole || args.response.responseType == ResponseType.LogOnly)
      {
        // Log the result.
        ErrorLogger.LogError(new Error(ErrorCode.ConsoleMessage, ErrorSeverity.Information, args.response.message));
      }
      else
      {
        if (isFromSocket)
        {
          SocketMessageWrapper socketMessageWrapper = (SocketMessageWrapper)args.message;

          // If private response, get the DM channel to the user, otherwise use the current channel.
          IMessageChannel responseChannel =
            args.response.responseType == ResponseType.Private ?
              (await socketMessageWrapper.User.GetOrCreateDMChannelAsync()) : 
              (IMessageChannel)socketMessageWrapper.Channel;

          SendMessage(args.response, responseChannel);
        }
        else
        {
          ErrorLogger.LogError(new Error(ErrorCode.NotImplemented, ErrorSeverity.Error, $"Cannot reply to channel {args.message.ChannelName} ({args.message.ChannelId}) by user {args.message.Username} ({args.message.UserId}) as the message is not from the socket."));
        }
      }
    }
  }
}