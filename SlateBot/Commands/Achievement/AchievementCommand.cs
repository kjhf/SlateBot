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
    private readonly Language.LanguageHandler languageHandler;
    private string[] aliases = new[] { "Achievement", "Achievements" };
    private string examples = Constants.BotMention + " Achievement";
    private string help = "Displays information about user's achievements";
    private ModuleType module = ModuleType.General;
    
    internal AchievementCommand(Language.LanguageHandler languageHandler, string[] aliases, string examples, string help, ModuleType module)
    {
      this.languageHandler = languageHandler;
      this.aliases = aliases;
      this.examples = examples;
      this.help = help;
      this.module = module;
    }

    public override string[] Aliases => aliases;
    public override CommandHandlerType CommandHandlerType => CommandHandlerType.Achievements;
    public override string Examples => examples;
    public override List<KeyValuePair<string, string>> ExtraData => ConstructExtraData();
    public override string Help => help;
    public override ModuleType Module => module;

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

    private List<KeyValuePair<string, string>> ConstructExtraData()
    {
      // No extra data for the command.
      return null;
    }
  }
}