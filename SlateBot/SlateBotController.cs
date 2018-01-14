using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SlateBot.DAL;
using SlateBot.Errors;
using SlateBot.Language;

namespace SlateBot
{
  /// <summary>
  /// The <see cref="SlateBotController"/> handles the main part of the program.
  /// </summary>
  class SlateBotController
  {
    internal readonly SlateBotDAL dal;
    internal readonly LanguageHandler languageHandler;
    internal ErrorLogger ErrorLogger => dal.errorLogger;

    public SlateBotController()
    {
      this.dal = new SlateBotDAL();
      this.languageHandler = new LanguageHandler(dal);
    }

    internal void Initialise()
    {
      this.dal.Initialise();
      this.languageHandler.Initialise();

      ErrorLogger.LogError(new Error(ErrorCode.ProgramInitialised, ErrorSeverity.Debug));
    }

    internal void HandleConsoleCommand(string line)
    {
    }
  }
}
