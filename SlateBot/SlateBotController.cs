using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

    public SlateBotController()
    {
      this.dal = new SlateBotDAL();
      this.languageHandler = new LanguageHandler(dal);
      this.lifecycle = new SlateBotControllerLifecycle(this);
      this.commandHandlerController = new CommandController(ErrorLogger, dal, languageHandler);
      this.severSettingsHandler = new SeverSettingsHandler(ErrorLogger, dal);
    }

    public void Initialise()
    {
      this.dal.Initialise();
      this.languageHandler.Initialise();
      this.commandHandlerController.Initialise();
      this.severSettingsHandler.Initialise();

      ErrorLogger.LogError(new Error(ErrorCode.Success, ErrorSeverity.Debug, "Program initialised successfully."));
    }

    internal void HandleConsoleCommand(string line)
    {
      HandleCommandReceived(new SenderDetail(severSettingsHandler.consoleServerSettings), new ConsoleMessageDetail(line));
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

        ResponseType responseType = command.ResponseType;
        if (senderDetail.ServerSettings == severSettingsHandler.consoleServerSettings)
        {
          // Send response to console.
          responseType = ResponseType.LogOnly;
        }

        switch (responseType)
        {
          case ResponseType.Default:
            {
              // Post to the channel.
              break;
            }

          case ResponseType.LogOnly:
            {
              // Log the result.
              ErrorLogger.LogError(new Error(ErrorCode.ConsoleMessage, ErrorSeverity.Information, response));
              break;
            }

          case ResponseType.Private:
            {
              // Send the result to the user privately.
              break;
            }
        }
      }
    }

    internal void HandleMessageReceived(SocketMessageWrapper socketMessage)
    {
      // First, get the server settings for the guild this message is from.
      ServerSettings serverSettings = severSettingsHandler.GetOrCreateServerSettings(socketMessage.GuildId);
      
      // Handle the command
      HandleCommandReceived(new SenderDetail(serverSettings), socketMessage);
    }
  }
}
