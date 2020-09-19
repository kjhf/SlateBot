using SlateBot.DAL.CommandFile;

namespace SlateBot.Commands
{
  internal interface ICommandHandler
  {
    CommandHandlerType CommandHandlerType { get; }

    Command CreateCommand(SlateBotController controller, CommandFile file);
  }
}