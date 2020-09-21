using SlateBot.Language;
using System.Collections.Generic;

namespace SlateBot.Commands.Reload
{
  public class ReloadCommand : Command
  {
    private readonly SlateBotController controller;
    private readonly LanguageHandler languageHandler;

    internal ReloadCommand(SlateBotController controller, LanguageHandler languageHandler, string[] aliases, string examples, string help, ModuleType module)
      : base(CommandHandlerType.Reload, aliases, examples, help, module)
    {
      this.controller = controller ?? throw new System.ArgumentNullException(nameof(controller));
      this.languageHandler = languageHandler ?? throw new System.ArgumentNullException(nameof(languageHandler));
    }

    public override IList<Response> Execute(SenderSettings senderDetail, IMessageDetail args)
    {
      if (Constants.IsBotOwner(senderDetail.UserSettings.UserId))
      {
        controller.Initialise();
        return new Response[] { Response.CreateFromReact(Emojis.SoonUnicode) };
      }
      else
      {
        return Response.CreateArrayFromString($"{Emojis.CrossSymbol} {languageHandler.GetPhrase(senderDetail.ServerSettings.Language, "Error_BotOwnerOnly")}.");
      }
    }
  }
}