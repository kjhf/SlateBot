using System.Collections.ObjectModel;

namespace SlateBot
{
  public static class Emojis
  {
    // Emojis
    public const string TickSymbol = ":white_check_mark:";

    public const string CrossSymbol = ":x:";
    public const string ExclamationSymbol = ":exclamation:";
    public const string RedoSymbol = ":arrow_right_hook:";
    public const string HookArrowUpSymbol = ":arrow_heading_up:";
    public const string InfoSymbol = ":information_source:";
    public const string PartyPopperSymbol = ":tada:";
    public const string BrokenHeartSymbol = ":broken_heart:";
    public const string Clap = ":clap:";
    public const string TimerSymbol = ":timer:";
    public const string QuestionSymbol = ":grey_question:";
    public const string Trophy = ":trophy:";
    public const string NoEntry = ":no_entry:";
    public const string Warning = ":warning:";
    public const string Dice = ":game_die:";
    public const string Star = ":star:";

    public const string CorrectMastermindPin = ":large_blue_circle:";
    public const string MisplacedMastermindPin = ":white_circle:";
    public const string IncorrectMastermindPin = "—";

    public const string PlaySymbol = ":arrow_forward:";
    public const string RepeatSymbol = ":repeat:";
    public const string StopSymbol = ":stop_button:";
    public const string ShuffleSymbol = ":twisted_rightwards_arrows:";
    public const string ArrowRightSymbol = ":arrow_right:";
    public const string NoEntrySign = ":no_entry_sign:";

    public const string CheckUnicode = "✅";
    public const string MessageUnicode = "✉️";
    public const string DiscUnicode = "💾";
    public const string SlothUnicode = "🦥";
    public const string ThumbsUpUnicode = "👍";

    // Server emojis
    public const string Splatoon_SplatZones = "<:Splatoon_Mode_Splat_Zones:346072719911026689>";

    public const string Splatoon_Rainmaker = "<:Splatoon_Mode_Rainmaker:346072720112484375>";
    public const string Splatoon_TowerControl = "<:Splatoon_Mode_Tower_Control:346072721521770506>";
    public const string Splatoon_TurfWars = "<:Splatoon_Mode_Turf_Wars:346072720448159745>";
    public const string Splatoon_PrivateBattle = "<:Splatoon_Mode_Private_Battle:346076590230470658>";
    public const string Splatoon_RankedBattle = "<:Splatoon_Mode_Ranked_Battle:346076590356299788> ";
    public const string Splatoon_SquadBattle = "<:Splatoon_Mode_Squad_Battle:346076828911534081>";
    public const string Splatoon_RegularBattle = "<:Splatoon_Mode_Regular_Battle:346078496008699915>";
    public const string Splatoon_LeagueBattle = "<:Splatoon_Mode_League_Battle:346079192351244298>";

    public static readonly ReadOnlyCollection<string> RockSymbols = new ReadOnlyCollection<string>(new[] { ":fist:", ":punch:", ":left_facing_fist:", ":new_moon:" });
    public static readonly ReadOnlyCollection<string> PaperSymbols = new ReadOnlyCollection<string>(new[] { ":hand_splayed:", ":raised_back_of_hand:", ":raised_hand:", ":page_facing_up:" });
    public static readonly ReadOnlyCollection<string> ScissorsSymbols = new ReadOnlyCollection<string>(new[] { ":v:", ":vulcan:", ":fingers_crossed:", ":scissors:" });

    public static readonly ReadOnlyCollection<string> CheerleaderSymbols = new ReadOnlyCollection<string>(new[] { ":ok_woman:", ":ok_woman::skin-tone-1:", ":ok_woman::skin-tone-2:", ":ok_woman::skin-tone-3:", ":ok_woman::skin-tone-4:", ":ok_woman::skin-tone-5:" });
  }
}