using SlateBot.DAL.CommandFile;

namespace SlateBot.Commands.ReceiveFile
{
  internal class ReceiveFileCommandHandler : ICommandHandler
  {
    public CommandHandlerType CommandHandlerType => CommandHandlerType.ReceiveFile;

    public Command CreateCommand(SlateBotController controller, CommandFile _)
    {
      return new ReceiveFileCommand(controller.dal, controller.commandHandlerController, controller);
    }
  }
}