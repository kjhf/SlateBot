using System.Collections.ObjectModel;

namespace SlateBot
{
  public static class Emojis
  {
    // Emojis
    public const string TickSymbol = ":white_check_mark:";

    public const string ExclamationSymbol = ":exclamation:";
    public const string RedoSymbol = ":arrow_right_hook:";
    public const string HookArrowUpSymbol = ":arrow_heading_up:";
    public const string InfoSymbol = ":information_source:";
    public const string PartyPopperSymbol = ":tada:";
    public const string BrokenHeartSymbol = ":broken_heart:";
    public const string Clap = ":clap:";
    public const string TimerSymbol = ":timer:";
    public const string QuestionSymbol = ":grey_question:";
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
    public const string CrossUnicode = "❌";
    public const string DiscUnicode = "💾";
    public const string MessageUnicode = "✉️";
    public const string RunningUnicode = "🏃";
    public const string SoonUnicode = "🔜";
    public const string ThumbsUpUnicode = "👍";
    public const string TrophyUnicode = "🏆";
    public const string TurtleUnicode = "🐢";

    // Server emojis
    // To find these, use \:emojiName: in the Discord chat.
    public const string Splatoon_SplatZones = "<:Splatoon_Mode_Splat_Zones:346072719911026689>";
    public const string Splatoon_Rainmaker = "<:Splatoon_Mode_Rainmaker:346072720112484375>";
    public const string Splatoon_TowerControl = "<:Splatoon_Mode_Tower_Control:346072721521770506>";
    public const string Splatoon_TurfWars = "<:Splatoon_Mode_Turf_Wars:346072720448159745>";
    public const string Splatoon_PrivateBattle = "<:Splatoon_Mode_Private_Battle:346076590230470658>";
    public const string Splatoon_RankedBattle = "<:Splatoon_Mode_Ranked_Battle:346076590356299788> ";
    public const string Splatoon_SquadBattle = "<:Splatoon_Mode_Squad_Battle:346076828911534081>";
    public const string Splatoon_RegularBattle = "<:Splatoon_Mode_Regular_Battle:346078496008699915>";
    public const string Splatoon_LeagueBattle = "<:Splatoon_Mode_League_Battle:346079192351244298>";

    public const string Battlefy = "<:battlefy:810346162161844274>";
    public const string Discord = "<:discord:810348495079866370>";
    public const string Twitch = "<:twitch:810346162023432213>";
    public const string Twitter = "<:twitter:810346162418614312>";
    public const string LowInk = "<:LowInk:869389316881809438>";
    public const string Typing = "<a:typing:897094396502224916>";
    public const string Eevee = "<a:eevee_slap:895391059985715241>";
    public const string Top500 = "<:Top500:896488842977230889>";
    
    // Abilities
    public const string AbilityDoubler = "<:AbilityDoubler:841052609648525382>";
    public const string BombDefenseUpDx = "<:BombDefenseUpDX:841052609333821471>";
    public const string Comeback = "<:Comeback:841052609300922419>";
    public const string DropRoller = "<:DropRoller:841052609859289118>";
    public const string Haunt = "<:Haunt:841052609850900530>";
    public const string InkRecoveryUp = "<:InkRecoveryUp:841052609866629120>";
    public const string InkResistanceUp = "<:InkResistanceUp:841052609627684875>";
    public const string InkSaverMain = "<:InkSaverMain:841052609875148911>";
    public const string InkSaverSub = "<:InkSaverSub:841052609992327168>";
    public const string LastDitchEffort = "<:LastDitchEffort:841052609870692362>";
    public const string MainPowerUp = "<:MainPowerUp:841052609972011008>";
    public const string NinjaSquid = "<:NinjaSquid:841052610122219574>";
    public const string ObjectShredder = "<:ObjectShredder:841052609858895914>";
    public const string OpeningGambit = "<:OpeningGambit:841052610080669697>";
    public const string QuickRespawn = "<:QuickRespawn:841052610256437289>";
    public const string QuickSuperJump = "<:QuickSuperJump:841052609723891733>";
    public const string RespawnPunisher = "<:RespawnPunisher:841052610050916383>";
    public const string RunSpeedUp = "<:RunSpeedUp:841052610320400475>";
    public const string SpecialChargeUp = "<:SpecialChargeUp:841052610193784862>";
    public const string SpecialPowerUp = "<:SpecialPowerUp:841052610199027764>";
    public const string SpecialSaver = "<:SpecialSaver:841052610038726667>";
    public const string StealthJump = "<:StealthJump:841052610131656724>";
    public const string SubPowerUp = "<:SubPowerUp:841052609912897567>";
    public const string SwimSpeedUp = "<:SwimSpeedUp:841052610106097674>";
    public const string Tenacity = "<:Tenacity:841052610173206548>";
    public const string ThermalInk = "<:ThermalInk:841052609803583489>";
    public const string UnknownAbility = "<:Unknown:892498504822431754>";

    public static readonly ReadOnlyCollection<string> RockSymbols = new ReadOnlyCollection<string>(new[] { ":fist:", ":punch:", ":left_facing_fist:", ":new_moon:" });
    public static readonly ReadOnlyCollection<string> PaperSymbols = new ReadOnlyCollection<string>(new[] { ":hand_splayed:", ":raised_back_of_hand:", ":raised_hand:", ":page_facing_up:" });
    public static readonly ReadOnlyCollection<string> ScissorsSymbols = new ReadOnlyCollection<string>(new[] { ":v:", ":vulcan:", ":fingers_crossed:", ":scissors:" });

    public static readonly ReadOnlyCollection<string> CheerleaderSymbols = new ReadOnlyCollection<string>(new[] { ":ok_woman:", ":ok_woman::skin-tone-1:", ":ok_woman::skin-tone-2:", ":ok_woman::skin-tone-3:", ":ok_woman::skin-tone-4:", ":ok_woman::skin-tone-5:" });

    public static readonly ReadOnlyCollection<string> NumbersKeyCaps = new ReadOnlyCollection<string>(new[] 
    { 
      "1️⃣","2️⃣","3️⃣","4️⃣","5️⃣","6️⃣","7️⃣","8️⃣","9️⃣","🔟",
      // Big thank you to GabrielDoddOfficialYT -- https://emoji.gg/user/436716390246776833
      "<:keycap_11:895381504023199825>",
      "<:keycap_12:895381503977087026>",
      "<:keycap_13:895381504048386078>",
      "<:keycap_14:895381504467824691>",
      "<:keycap_15:895381504560095322>",
      "<:keycap_16:895381504505557032>",
      "<:keycap_17:895381504576864296>",
      "<:keycap_18:895381504983719937>",
      "<:keycap_19:895381504476196864>",
      "<:keycap_20:895381504149041225>" 
    });
  }
}