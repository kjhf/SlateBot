using System;
using System.Text;

namespace SlateBot
{
  public static class VariableStrings
  {
    public const string UserNameReplaceString = "%username%";
    public const string MentionUserNameReplaceString = "%mentionusername%";
    public const string ServerNameReplaceString = "%servername%";
    public const string CommandDetailReplaceString = "%commanddetail%";
    public const string EscapedDetailReplaceString = "%escapeddetail%";
    public const string UsernameOrDetailReplaceString = "%usernameordetail%";
    public const string AppIdReplaceString = "%appid%";
    public const string OwnerUserNameReplaceString = "%ownerusername%";
    public const string HelpPageENReplaceString = "%helppageen%";
    public const string HelpPageFRReplaceString = "%helppagefr%";
    public const string BotNameReplaceString = "%botname%";
    public const string DevelopmentServerReplaceString = "%developmentserver%";

    public static StringBuilder Replace(StringBuilder input, string username, string userid, string servername = "", string commandDetail = "")
    {
      return input
        .Replace(OwnerUserNameReplaceString, Constants.Owner)
        .Replace(MentionUserNameReplaceString, "<@" + userid + ">")
        .Replace(UserNameReplaceString, username)
        .Replace(ServerNameReplaceString, servername)
        .Replace(CommandDetailReplaceString, commandDetail)
        .Replace(EscapedDetailReplaceString, Uri.EscapeDataString(commandDetail).Replace("+", "%2B"))
        .Replace(UsernameOrDetailReplaceString, string.IsNullOrWhiteSpace(commandDetail) ? (username) : (commandDetail))
        .Replace(AppIdReplaceString, Tokens.ClientId.ToString())
        .Replace(HelpPageENReplaceString, Constants.HelpPageENURL)
        .Replace(HelpPageFRReplaceString, Constants.HelpPageENURL) // TODO make French help page
        .Replace(BotNameReplaceString, Constants.Username)
        .Replace(DevelopmentServerReplaceString, Constants.DevelopmentServerLink)
        .Replace("\\r\\n", Environment.NewLine);
    }

    public static string Replace(string input, string username, string userid, string servername = "", string commandDetail = "")
    {
      StringBuilder sb = new StringBuilder(input);
      return Replace(sb, username, userid, servername, commandDetail).ToString();
    }
  }
}