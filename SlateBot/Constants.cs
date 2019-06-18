using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlateBot
{
  public static class Constants
  {
    public const string Owner = "kjhf";
    public const string OwnerUsernameWithDiscriminator = Owner + "#3249";
    public const string Username = @"SlateBot";
    public const string UsernameWithDiscriminator = Username + "#1979";
    public const ulong BotSpamChannelId = 189111240004468736;
    public const ulong DevelopmentServerId = 255312220567764993;
    public const ulong GeneralHelpChannelId = 255312220567764993;
    public const ulong LogChannelId = 255317511212367873;
    public const ulong BotStatusChannelId = 255312337601429507;
    public const ulong BotId = 401799984741089281;
    public const ulong OwnerId = 97288493029416960;
    public const ulong ErrorId = ulong.MaxValue;
    public const ulong ConsoleId = ulong.MaxValue - 1;

    public const int MessageLimit = 1990;
    public const string BotMention = "<@!401799984741089281>"; // bot id
    public const string OwnerMention = "<@!97288493029416960>"; // owner id

    public const string HelpPageTitle = @"Blog:KJ_Bot/Commands";
    public const string HelpPageENURL = @"https://splatoonwiki.org/wiki/" + HelpPageTitle;
    public const string DevelopmentServerLink = @"https://discord.gg/Px5Bhny";

    public static bool IsBotOwner(ulong id)
    {
      return (id == OwnerId) || (id == ConsoleId);
    }
  }
}
