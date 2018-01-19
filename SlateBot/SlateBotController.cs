using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SlateBot.DAL;
using SlateBot.Errors;
using SlateBot.Language;
using SlateBot.Lifecycle;

namespace SlateBot
{
  /// <summary>
  /// The <see cref="SlateBotController"/> handles the main part of the program.
  /// </summary>
  class SlateBotController
  {
    internal readonly SlateBotDAL dal;
    internal ErrorLogger ErrorLogger => dal.errorLogger;
    private readonly LanguageHandler languageHandler;
    private readonly SlateBotControllerLifecycle lifecycle;

    public SlateBotController()
    {
      this.dal = new SlateBotDAL();
      this.languageHandler = new LanguageHandler(dal);
      this.lifecycle = new SlateBotControllerLifecycle(this);
    }

    internal void Initialise()
    {
      this.dal.Initialise();
      this.languageHandler.Initialise();

      ErrorLogger.LogError(new Error(ErrorCode.Success, ErrorSeverity.Debug, "Program initialised successfully."));
    }

    internal void HandleConsoleCommand(string line)
    {
      lifecycle.OnMessageReceived(this, new ConsoleMessageDetail(line));
    }
  }
}
