using SlateBot.DAL.CommandFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlateBot.Commands
{
  internal interface ICommandHandler
  {
    CommandHandlerType CommandHandlerType { get; }

    Command CreateCommand(SlateBotController controller, CommandFile file);
  }
}
