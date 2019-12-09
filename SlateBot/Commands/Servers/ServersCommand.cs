using Discord.WebSocket;
using SlateBot.Language;
using SlateBot.SavedSettings;
using System.Collections.Generic;
using System.Linq;

namespace SlateBot.Commands.Servers
{
  public class ServersCommand : Command
  {
    private readonly DiscordSocketClient client;
    private readonly ServerSettingsHandler serverSettingsHandler;
    private readonly LanguageHandler languageHandler;

    internal ServersCommand(DiscordSocketClient client, ServerSettingsHandler serverSettingsHandler, LanguageHandler languageHandler, string[] aliases, string examples, string help, ModuleType module)
      : base(CommandHandlerType.Servers, aliases, examples, help, module)
    {
      this.client = client;
      this.serverSettingsHandler = serverSettingsHandler;
      this.languageHandler = languageHandler;
    }

    public override IList<Response> Execute(SenderSettings senderDetail, IMessageDetail args)
    {
      ServerSettings serverSettings = senderDetail.ServerSettings;
      CommandMessageHelper command = new CommandMessageHelper(serverSettings.CommandSymbol, args.Message);
      string[] commandParams = command.CommandParams;

      if (Constants.IsBotOwner(args.UserId))
      {
        var servers = (client.Guilds).ToArray();
        string serverOutputString = string.Join("\n ",
          servers.Select((server) =>
          {
            var ss = serverSettingsHandler.GetOrCreateServerSettings(server.Id);
            var owner = server.Owner;
            return $"`{server.Name}` ({ss.Language}): `{owner?.Username}`";
          }));

        return Response.CreateArrayFromString(servers.Length + " servers: \n" + serverOutputString);
      }

      return Response.CreateArrayFromString($"{Emojis.CrossSymbol} {languageHandler.GetPhrase(serverSettings.Language, "Error_BotOwnerOnly")}.");
    }
  }
}