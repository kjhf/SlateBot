using Discord.WebSocket;
using SlateBot.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlateBot.Commands.Achievement
{
  public class AchievementCommand : Command
  {
    private readonly SlateBotController controller;
    private readonly Language.LanguageHandler languageHandler;
    private string[] aliases = new[] { "Achievement", "Achievements" };
    private string examples = Constants.BotMention + " Achievement";
    private string help = "Displays information about user's achievements";
    private ModuleType module = ModuleType.General;
    private ResponseType responseType = ResponseType.Private;
    
    internal AchievementCommand(Language.LanguageHandler languageHandler, string[] aliases, string examples, string help, ModuleType module, ResponseType responseType)
    {
      this.controller = SlateBotProgram.GetController();
      this.languageHandler = languageHandler;
      this.aliases = aliases;
      this.examples = examples;
      this.help = help;
      this.module = module;
      this.responseType = responseType;
    }

    public override string[] Aliases => aliases;
    public override CommandHandlerType CommandHandlerType => CommandHandlerType.Achievements;
    public override string Examples => examples;
    public override List<KeyValuePair<string, string>> ExtraData => ConstructExtraData();
    public override string Help => help;
    public override ModuleType Module => module;
    public override ResponseType ResponseType => responseType;

    public override string Execute(SenderSettings senderDetail, IMessageDetail args)
    {
      StringBuilder output = new StringBuilder();
      ServerSettings serverSettings = senderDetail.ServerSettings;
      UserSettings userSettings = senderDetail.UserSettings;
      CommandMessageHelper command = new CommandMessageHelper(serverSettings.CommandSymbol, args.Message);
      string[] commandParams = command.CommandParams;

      var allAchievements = SaveData.Achievements.Achievement.AllAchievements;
      var unlockedAchievements = userSettings.UserStats.GetAchievements();

      // First, reply to the room that the command was written in.
      // This has minimal information so as to not spoil achievements
      // for other users.
      output.AppendLine($"{languageHandler.GetPhrase(senderDetail.ServerSettings.Language, "Achievements_AchievementsUnlocked")}: {unlockedAchievements.Count} {Emojis.Trophy}");

      // Next, PM the user their achievements and any still to unlock.
      // TODO -- redesign this, we should return response objects instead.
      Task.Run(async () =>
      {
        StringBuilder sb = new StringBuilder();
        foreach (var achievement in allAchievements)
        {
          if (achievement.HasAchieved(userSettings.UserStats))
          {
            sb.AppendLine($"{Emojis.Trophy} {achievement.GetDescription(languageHandler, senderDetail.ServerSettings.Language)}");
          }
          else
          {
            sb.AppendLine($"???");
          }
        }

        if (args is ConsoleMessageDetail consoleMessageDetail)
        {
          controller.ErrorLogger.LogError(new Error(ErrorCode.ConsoleMessage, ErrorSeverity.Information, sb.ToString()));
        }
        else if (args is SocketMessageWrapper socketMessageWrapper)
        {
          ISocketMessageChannel responseChannel = (ISocketMessageChannel)(await socketMessageWrapper.User.GetOrCreateDMChannelAsync());

          controller.SendMessage(sb.ToString(), responseChannel);
        }
      });

      return output.ToString();
    }

    private List<KeyValuePair<string, string>> ConstructExtraData()
    {
      // No extra data for the command.
      return null;
    }
  }
}