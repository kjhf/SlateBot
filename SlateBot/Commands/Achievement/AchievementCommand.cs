using SlateBot.Language;
using System.Collections.Generic;
using System.Text;

namespace SlateBot.Commands.Achievement
{
  public class AchievementCommand : Command
  {
    private readonly LanguageHandler languageHandler;

    internal AchievementCommand(LanguageHandler languageHandler, string[] aliases, string examples, string help, ModuleType module)
      : base(CommandHandlerType.Achievements, aliases, examples, help, module)
    {
      this.languageHandler = languageHandler;
    }

    public override IList<Response> Execute(SenderSettings senderDetail, IMessageDetail args)
    {
      ServerSettings serverSettings = senderDetail.ServerSettings;
      UserSettings userSettings = senderDetail.UserSettings;
      CommandMessageHelper command = new CommandMessageHelper(serverSettings.CommandSymbol, args.Message);
      string[] commandParams = command.CommandParams;

      var allAchievements = SaveData.Achievements.Achievement.AllAchievements;
      var unlockedAchievements = userSettings.UserStats.GetAchievements();

      // First, reply to the room that the command was written in.
      // This has minimal information so as to not spoil achievements
      // for other users.
      string outputToChannel = ($"{languageHandler.GetPhrase(senderDetail.ServerSettings.Language, "Achievements_AchievementsUnlocked")}: {unlockedAchievements.Count}/{allAchievements.Count} {Emojis.Trophy}");
      Response channelResponse = new Response
      {
        command = this,
        embed = Utility.EmbedUtility.StringToEmbed(outputToChannel, 200, 200, 50),
        responseType = ResponseType.Default
      };

      // Next, PM the user their achievements and any still to unlock.
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

      string outputToPrivate = sb.ToString();
      Response privateResponse = new Response
      {
        command = this,
        embed = Utility.EmbedUtility.StringToEmbed(outputToPrivate, 200, 200, 50),
        responseType = ResponseType.Private
      };

      return new[] { channelResponse, privateResponse };
    }
  }
}