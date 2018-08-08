using Discord;

namespace SlateBot.Commands
{
  public class Response
  {
    /// <summary>
    /// The command that was executed.
    /// </summary>
    public Command command;

    /// <summary>
    /// The <see cref="EmbedBuilder"/> response message.
    /// </summary>
    public EmbedBuilder embed;

    /// <summary>
    /// The message to respond with (if Embed is null or not supported).
    /// </summary>
    public string message;

    /// <summary>
    /// How the bot should respond.
    /// </summary>
    public ResponseType responseType;

    public override string ToString()
    {
      return $"{nameof(Response)}: \"{message}\" sending to {responseType} from command {command}";
    }
  }
}