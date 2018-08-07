using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlateBot.Utility
{
  public static class EmbedUtility
  {
    public static Embed StringToEmbed(string str, int r = 0, int g = 0, int b = 0, string title = null)
    {
      return StringToEmbed(str, new Color(r, g, b), title);
    }

    public static Embed StringToEmbed(string str, Color color, string title = null)
    {
      var builder = new EmbedBuilder()
        .WithDescription(str)
        .WithColor(color)
        .WithTimestamp(DateTimeOffset.UtcNow);

      if (title != null)
      {
        builder.WithAuthor(author =>
        {
          author.WithName(title);
        });
      }
      var embed = builder.Build();
      return embed;
    }
  }
}
