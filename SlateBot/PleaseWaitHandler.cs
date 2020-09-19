using Discord;
using SlateBot.Commands;
using SlateBot.Language;
using SlateBot.SavedSettings;
using SlateBot.Utility;
using System;
using System.Collections.Generic;

namespace SlateBot
{
  public class PleaseWaitHandler
  {
    private readonly Stack<IUserMessage> pleaseWaitMessageStack = new Stack<IUserMessage>();
    private readonly Random rand;
    private readonly ServerSettingsHandler serverSettingsHandler;
    private readonly LanguageHandler languageHandler;
    private int deleteMessagesPending = 0;

    public PleaseWaitHandler(ServerSettingsHandler serverSettingsHandler, LanguageHandler languageHandler)
    {
      rand = new Random();
      this.serverSettingsHandler = serverSettingsHandler;
      this.languageHandler = languageHandler;
    }

    internal void DeleteStack()
    {
      foreach (var m in pleaseWaitMessageStack)
      {
        IGuild s = m.Channel.GetGuild();
        if (s != null)
        {
          var serverSettings = serverSettingsHandler.GetOrCreateServerSettings(s.Id);
          m.EditAsync(languageHandler.GetPhrase(serverSettings.Language, "Error_Oops")).ConfigureAwait(false);
        }
      }
    }

    internal void PopPleaseWaitMessage()
    {
      IUserMessage m = null;
      lock (pleaseWaitMessageStack)
      {
        if (pleaseWaitMessageStack.Count > 0)
        {
          m = pleaseWaitMessageStack.Pop();
        }
        else
        {
          deleteMessagesPending++;
        }
      }

      if (m != null)
      {
        m.DeleteAsync(new RequestOptions { RetryMode = RetryMode.RetryTimeouts });
      }
    }

    internal Response CreatePleaseWaitResponse(Languages language)
    {
      int r = rand.Next(0, 3);
      string output = Emojis.TimerSymbol + " " + languageHandler.GetPhrase(language, "PleaseWait_" + r);

      Response pleaseWaitResponse = new Response
      {
        Embed = EmbedUtility.ToEmbed(output, Discord.Color.Gold),
        Message = output,
        ResponseType = ResponseType.PleaseWaitMessage
      };

      return pleaseWaitResponse;
    }

    internal void PushToStack(IUserMessage pleaseWaitMessage)
    {
      bool delete = false;

      lock (pleaseWaitMessageStack)
      {
        delete = deleteMessagesPending > 0;

        if (delete)
        {
          --deleteMessagesPending;
        }
        else
        {
          pleaseWaitMessageStack.Push(pleaseWaitMessage);
        }
      }

      if (delete)
      {
        pleaseWaitMessage.DeleteAsync(new RequestOptions { RetryMode = RetryMode.RetryTimeouts });
      }
    }

    /// <summary>
    /// Initialise the please wait handler.
    /// </summary>
    public void Initialise()
    {
      pleaseWaitMessageStack.Clear();
    }
  }
}