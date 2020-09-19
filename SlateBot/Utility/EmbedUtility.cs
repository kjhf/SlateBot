using Discord;
using System;

namespace SlateBot.Utility
{
  public static class EmbedUtility
  {
    /// <summary>
    /// Convert string to embed with an optional title and rgb colour
    /// </summary>
    /// <remarks>Forwarding method</remarks>
    public static EmbedBuilder ToEmbed(string str, int r, int g, int b, string title = null)
    {
      return ToEmbed(str, new Color(r, g, b), title);
    }

    /// <summary>
    /// Convert string to embed with an optional title and colour
    /// </summary>
    public static EmbedBuilder ToEmbed(string str = null, Color? color = null, string title = null, string imageURL = null)
    {
      if (str == null && imageURL == null)
      {
        throw new ArgumentNullException($"{nameof(str)} and {nameof(imageURL)} cannot both be null.");
      }

      var builder = new EmbedBuilder();

      if (str != null)
      {
        builder = builder.WithDescription(str);
      }
      else if (imageURL != null)
      {
        builder = builder.WithDescription(imageURL);
      }

      if (color != null)
      {
        builder = builder.WithColor((Color)color);
      }

      if (imageURL != null)
      {
        builder = builder.WithImageUrl(imageURL);
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