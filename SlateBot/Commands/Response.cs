using Discord;
using System.Collections.Generic;

namespace SlateBot.Commands
{
  public class Response
  {
    /// <summary>
    /// The command was recognised but the response is coming asynchronously.
    /// </summary>
    public static Response WaitForAsync = new Response { ResponseType = ResponseType.None, Message = "" };

    /// <summary>
    /// The command was unrecognised or no response is coming.
    /// </summary>
    public static IList<Response> NoResponse = new Response[0];

    /// <summary>
    /// Create a Response object from a message to return using default parameters.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="responseColor"></param>
    /// <returns></returns>
    public static Response CreateFromString(string message, Color? responseColor = null, string imageUrl = null)
    {
      return new Response
      {
        Embed = Utility.EmbedUtility.ToEmbed(message, responseColor),
        Message = message,
        ResponseType = ResponseType.Default
      };
    }

    /// <summary>
    /// Create a Response object to react to the last message with the given emoji.
    /// </summary>
    public static Response CreateFromReact(params string[] emojis)
    {
      return new Response
      {
        Message = string.Join("|", emojis),
        ResponseType = ResponseType.Default_React
      };
    }

    /// <summary>
    /// Create a Response object as an array from a message to return using default parameters.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="responseColor"></param>
    /// <returns></returns>
    public static Response[] CreateArrayFromString(string message, Color? responseColor = null)
    {
      return new[]
      {
        CreateFromString(message, responseColor)
      };
    }

    /// <summary>
    /// The <see cref="EmbedBuilder"/> response message.
    /// </summary>
    public EmbedBuilder Embed { get; set; }

    /// <summary>
    /// A file path to respond with (uploads a file).
    /// </summary>
    public string FilePath { get; set; }

    /// <summary>
    /// The message to respond with (if Embed is null or not supported).
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// How the bot should respond.
    /// </summary>
    public ResponseType ResponseType { get; set; } = ResponseType.Default;

    public override string ToString()
    {
      return $"{nameof(Response)}: \"{Message}\" sending to {ResponseType}";
    }
  }
}