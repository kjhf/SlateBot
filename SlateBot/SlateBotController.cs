using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using SlateBot.Commands;
using SlateBot.DAL;
using SlateBot.Errors;
using SlateBot.Events;
using SlateBot.Language;
using SlateBot.Lifecycle;
using SlateBot.SavedSettings;

namespace SlateBot
{
  /// <summary>
  /// The <see cref="SlateBotController"/> handles the main part of the program.
  /// </summary>
  class SlateBotController : IController
  {
    internal readonly CommandController commandHandlerController;
    internal readonly SlateBotDAL dal;
    internal ErrorLogger ErrorLogger => dal.errorLogger;
    private readonly LanguageHandler languageHandler;
    private readonly SlateBotControllerLifecycle lifecycle;
    private readonly ServerSettingsHandler serverSettingsHandler;
    private readonly UserSettingsHandler userSettingsHandler;
    internal DiscordSocketClient client;

    internal event CommandReceived OnCommandReceived;

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

    private Task Client_MessageUpdated(Discord.Cacheable<Discord.IMessage, ulong> arg1, SocketMessage arg2, ISocketMessageChannel arg3)
    {
      HandleMessageReceived(new SocketMessageWrapper(arg2));
      return Task.CompletedTask;
    }

    private Task Client_MessageReceived(SocketMessage arg)
    {
      HandleMessageReceived(new SocketMessageWrapper(arg));
      return Task.CompletedTask;
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

    internal void HandleConsoleCommand(string line)
    {
      UserSettings consoleUserSettings = userSettingsHandler.GetOrCreateUserSettings(Constants.ErrorId);
      HandleCommandReceived(new SenderDetail(serverSettingsHandler.consoleServerSettings, consoleUserSettings), new ConsoleMessageDetail(line));
    }

    /// <summary>
    /// Handle a command received from chat.
    /// This is after filtering to make sure a message is a command.
    /// </summary>
    internal void HandleCommandReceived(SenderDetail senderDetail, IMessageDetail message)
    {
      var result = commandHandlerController.ExecuteCommand(senderDetail, message);

      if (result.Item2 != null)
      {
        Command command = result.Item1;
        string response = result.Item2;
        OnCommandReceived?.Invoke(this, new CommandReceivedEventArgs(senderDetail, message, command, response));
      }
      else if (message is ConsoleMessageDetail) // If from the console
      {
        ErrorLogger.LogDebug("I got nothin'. (" + message.Message + ")", true);
      }
    }
    
    private async void SlateBotController_OnCommandReceived(object sender, CommandReceivedEventArgs args)
    {
      bool isFromConsole = args.message is ConsoleMessageDetail;
      bool isFromSocket = args.message is SocketMessageWrapper;
      ResponseType responseType = args.command.ResponseType;
      if (isFromConsole || responseType == ResponseType.LogOnly)
      {
        // Log the result.
        ErrorLogger.LogError(new Error(ErrorCode.ConsoleMessage, ErrorSeverity.Information, args.response));
      }
      else
      {
        if (isFromSocket)
        {
          SocketMessageWrapper socketMessageWrapper = (SocketMessageWrapper)args.message;

          // If private response, get the DM channel to the user, otherwise use the current channel.
          ISocketMessageChannel responseChannel =
            responseType == ResponseType.Private ?
            (ISocketMessageChannel)(await socketMessageWrapper.User.GetOrCreateDMChannelAsync())
            : (ISocketMessageChannel)socketMessageWrapper.Channel;

          lifecycle.OnMessageReadyToSend(args.response, responseChannel);
        }
        else
        {
          ErrorLogger.LogError(new Error(ErrorCode.NotImplemented, ErrorSeverity.Error, $"Cannot reply to channel {args.message.ChannelName} ({args.message.ChannelId}) by user {args.message.Username} ({args.message.UserId}) as the message is not from the socket."));
        }
      }
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
      HandleCommandReceived(new SenderDetail(serverSettings, userSettings), socketMessage);
    }
  }
}
