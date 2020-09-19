using Discord;
using Discord.WebSocket;
using SlateBot.Language;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SlateBot.Commands.Avatar
{
  public class AvatarCommand : Command
  {
    private readonly DiscordSocketClient client;
    private readonly LanguageHandler languageHandler;
    private readonly IAsyncResponder asyncResponder;

    internal AvatarCommand(DiscordSocketClient client, LanguageHandler languageHandler, IAsyncResponder asyncResponder, string[] aliases, string examples, string help, ModuleType module)
      : base(CommandHandlerType.Avatar, aliases, examples, help, module)
    {
      this.client = client;
      this.languageHandler = languageHandler;
      this.asyncResponder = asyncResponder;
    }

    public override IList<Response> Execute(SenderSettings senderDetail, IMessageDetail args)
    {
      ServerSettings serverSettings = senderDetail.ServerSettings;
      CommandMessageHelper command = new CommandMessageHelper(serverSettings.CommandSymbol, args.Message);
      string requestStr = command.CommandDetail;
      ulong requestedId;
      if (string.IsNullOrWhiteSpace(requestStr))
      {
        // Default to the user asking.
        requestedId = args.UserId;
      }
      else
      {
        // Parse a mention?
        if (requestStr.StartsWith("<@") && requestStr.EndsWith(">"))
        {
          requestStr = requestStr.Substring("<@".Length, requestStr.Length - 3); // Minus 3 for <@nnnnn>
        }

        // Id?
        if (!ulong.TryParse(requestStr, out requestedId))
        {
          // No, fail.
          return Response.CreateArrayFromString($"{Emojis.CrossSymbol} {languageHandler.GetPhrase(senderDetail.ServerSettings.Language, "Error_NoResults")}: {requestStr}");
        }
      }

      if (client != null)
      {
        Task.Run(async () =>
        {
          // From the id, determine if it's a user or server.
          // Is it a server?
          var candidateServer = await client.GetGuildAsync(requestedId).ConfigureAwait(false);
          if (candidateServer != null)
          {
            await asyncResponder.SendResponseAsync(args, Response.CreateFromString($"{candidateServer.IconUrl}")).ConfigureAwait(false);
          }

          // Is it a channel?
          IChannel candidateChannel = await client.GetDMChannelAsync(requestedId).ConfigureAwait(false);
          if (candidateChannel != null)
          {
            await asyncResponder.SendResponseAsync(args, Response.CreateFromString($"{candidateChannel.GetGuild().IconUrl}")).ConfigureAwait(false);
          }

          // Is it a user?
          IUser candidateUser = client.GetUser(requestedId);
          if (candidateUser != null)
          {
            await asyncResponder.SendResponseAsync(args, Response.CreateFromString($"{candidateUser.GetAvatarUrl(ImageFormat.Auto, 2048)}")).ConfigureAwait(false);
          }
        });
      }
      return new Response[] { Response.WaitForAsync };
    }
  }
}