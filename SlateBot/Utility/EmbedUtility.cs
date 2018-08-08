using Discord;

namespace SlateBot.Utility
{
  public static class EmbedUtility
  {
    public static EmbedBuilder StringToEmbed(string str, string title = null)
    {
      return StringToEmbed(str, null, title);
    }

    public static EmbedBuilder StringToEmbed(string str, int r, int g, int b, string title = null)
    {
      return StringToEmbed(str, new Color(r, g, b), title);
    }

    public static EmbedBuilder StringToEmbed(string str, Color? color, string title = null)
    {
      var builder = new EmbedBuilder()
        .WithDescription(str);

      if (color != null)
      {
        builder = builder.WithColor((Color)color);
      }

      if (title != null)
      {
        builder = builder.WithAuthor(author =>
        {
          author.WithName(title);
        });
      }
      return builder;
    }
  }
}