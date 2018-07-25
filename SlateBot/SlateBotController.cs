using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using SlateBot.Commands;
using SlateBot.DAL;
using SlateBot.Errors;
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
    private readonly SeverSettingsHandler severSettingsHandler;
    internal DiscordSocketClient client;

    public SlateBotController()
    {
      this.dal = new SlateBotDAL();
      this.languageHandler = new LanguageHandler(dal);
      this.lifecycle = new SlateBotControllerLifecycle(this);
      this.commandHandlerController = new CommandController(ErrorLogger, dal, languageHandler);
      this.severSettingsHandler = new SeverSettingsHandler(ErrorLogger, dal);

      this.client = new DiscordSocketClient();
      client.LoggedIn += Client_LoggedIn;
      client.LoggedOut += Client_LoggedOut;
      client.MessageReceived += Client_MessageReceived;
      client.MessageUpdated += Client_MessageUpdated;
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
      this.severSettingsHandler.Initialise();

      ErrorLogger.LogDebug("Program initialised successfully.");
    }

    internal void HandleConsoleCommand(string line)
    {
      HandleCommandReceived(new SenderDetail(severSettingsHandler.consoleServerSettings), new ConsoleMessageDetail(line));
    }

    /// <summary>
    /// Handle a command received from chat.
    /// This is after filtering to make sure a message is a command.
    /// </summary>
    internal async void HandleCommandReceived(SenderDetail senderDetail, IMessageDetail message)
    {
      var result = commandHandlerController.ExecuteCommand(senderDetail, message);
      bool isFromConsole = message is ConsoleMessageDetail;
      bool isFromSocket = message is SocketMessageWrapper;

      if (result.Item2 != null)
      {
        Command command = result.Item1;
        string response = result.Item2;

        ResponseType responseType = command.ResponseType;
        if (isFromConsole)
        {
          // Send response to console.
          responseType = ResponseType.LogOnly;
        }

        switch (responseType)
        {
          case ResponseType.LogOnly:
          {
            // Log the result.
            ErrorLogger.LogError(new Error(ErrorCode.ConsoleMessage, ErrorSeverity.Information, response));
            break;
          }

          case ResponseType.Default:
          {
            // Post to the channel.
            if (isFromSocket)
            {
              SocketMessageWrapper socketMessageWrapper = (SocketMessageWrapper)message;
              lifecycle.OnMessageReadyToSend(response, (ISocketMessageChannel)socketMessageWrapper.Channel);
            }
            else
            {
              ErrorLogger.LogError(new Error(ErrorCode.NotImplemented, ErrorSeverity.Error, $"Cannot reply to channel {message.ChannelName} ({message.ChannelId}) by user {message.Username} ({message.UserId}) as the message is not from the socket."));
            }
            break;
          }

          case ResponseType.Private:
          {
            // Send the result to the user privately.
            if (isFromSocket)
            {
              SocketMessageWrapper socketMessageWrapper = (SocketMessageWrapper)message;
              lifecycle.OnMessageReadyToSend(response, (ISocketMessageChannel)(await socketMessageWrapper.User.GetOrCreateDMChannelAsync()));
            }
            else
            {
              ErrorLogger.LogError(new Error(ErrorCode.NotImplemented, ErrorSeverity.Error, $"Cannot DM reply to user {message.Username} ({message.UserId}) as the message is not from the socket."));
            }
            break;
          }
        }
      }
      else if (isFromConsole)
      {
        ErrorLogger.LogError(new Error(ErrorCode.ConsoleMessage, ErrorSeverity.Information, "I got nothin'."));
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
      ServerSettings serverSettings = severSettingsHandler.GetOrCreateServerSettings(socketMessage.GuildId);
      
      // Handle the command
      HandleCommandReceived(new SenderDetail(serverSettings), socketMessage);
    }
  }
}
