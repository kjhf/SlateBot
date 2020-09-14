using SlateBot.Language;
using System.Collections.Generic;

namespace SlateBot.Commands.Say
{
  public class SayCommand : Command
  {
    private readonly IAsyncResponder asyncResponder;
    private readonly LanguageHandler languageHandler;

    internal SayCommand(IAsyncResponder asyncResponder, LanguageHandler languageHandler, string[] aliases, string examples, string help, ModuleType module)
      : base(CommandHandlerType.Say, aliases, examples, help, module)
    {
      this.asyncResponder = asyncResponder;
      this.languageHandler = languageHandler;
    }

    public override IList<Response> Execute(SenderSettings senderDetail, IMessageDetail args)
    {
      ServerSettings serverSettings = senderDetail.ServerSettings;
      CommandMessageHelper command = new CommandMessageHelper(serverSettings.CommandSymbol, args.Message);
      string responseMessage;

      if (Constants.IsBotOwner(args.UserId))
      {
        string channelIdStr = command.CommandParams[1];
        if (!string.IsNullOrWhiteSpace(channelIdStr))
        {
          bool parsed = ulong.TryParse(channelIdStr, out ulong targetChannelId);
          if (parsed)
          {
            string messageToSend = string.Join(" ", command.CommandParams, 2, command.CommandParams.Length - 2);
            if (string.IsNullOrEmpty(messageToSend))
            {
              responseMessage = ($"{Emojis.CrossSymbol} {languageHandler.GetPhrase(senderDetail.ServerSettings.Language, "Error_IncorrectParameter")}: say <channel id> <message> -- message");
            }
            else
            {
              if (asyncResponder.SendResponse(targetChannelId, Response.CreateFromString(messageToSend)))
              {
                responseMessage = ($"{Emojis.TickSymbol} {languageHandler.GetPhrase(senderDetail.ServerSettings.Language, "YourRequestHasBeenSent")}: {messageToSend} --> {channelIdStr}");
              }
              else
              {
                responseMessage = ($"{Emojis.CrossSymbol} {languageHandler.GetPhrase(senderDetail.ServerSettings.Language, "Error_NoResults")}: {channelIdStr}");
              }
            }
          }
          else
          {
            responseMessage = ($"{Emojis.CrossSymbol} {languageHandler.GetPhrase(senderDetail.ServerSettings.Language, "Error_IncorrectParameter")}: say <channel id> <message> -- {channelIdStr}");
          }
        }
        else
        {
          responseMessage = $"{Emojis.CrossSymbol} say <channel id> <message>.";
        }
      }
      else
      {
        responseMessage = $"{Emojis.CrossSymbol} {languageHandler.GetPhrase(serverSettings.Language, "Error_BotOwnerOnly")}.";
      }

      return Response.CreateArrayFromString(responseMessage);
    }
  }
}