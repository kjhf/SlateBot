using Discord;

namespace SlateBot.Commands
{
  public class Response
  {
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
