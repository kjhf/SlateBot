using Discord.WebSocket;
using SlateBot.Language;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SlateBot.Commands.SetName
{
  public class SetNameCommand : Command
  {
    private readonly DiscordSocketClient client;
    private readonly LanguageHandler languageHandler;

    internal SetNameCommand(DiscordSocketClient client, LanguageHandler languageHandler, string[] aliases, string examples, string help, ModuleType module)
      : base(CommandHandlerType.SetName, aliases, examples, help, module)
    {
      this.client = client;
      this.languageHandler = languageHandler;
    }

    public override IList<Response> Execute(SenderSettings senderDetail, IMessageDetail args)
    {
      ServerSettings serverSettings = senderDetail.ServerSettings;
      CommandMessageHelper command = new CommandMessageHelper(serverSettings.CommandSymbol, args.Message);

      if (Constants.IsBotOwner(args.UserId))
      {
        Task.Run(async () =>
        {
          string name = command.CommandDetail;
          if (args.ChannelId != Constants.ConsoleId)
          {
            var guild = await client.GetGuildAsync(args.GuildId).ConfigureAwait(false);
            var user = guild.GetUser(client.CurrentUser.Id);
            await user.ModifyAsync(x => x.Nickname = name).ConfigureAwait(false);
          }
          else
          {
            Console.Title = name;
          }
        });
        return new Response[] { Response.CreateFromReact(Emojis.ThumbsUpUnicode) };
      }
      else
      {
        return Response.CreateArrayFromString($"{Emojis.CrossSymbol} {languageHandler.GetPhrase(serverSettings.Language, "Error_BotOwnerOnly")}.");
      }
    }
  }
}