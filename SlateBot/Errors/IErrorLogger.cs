using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlateBot.Errors
{
  public interface IErrorLogger
  {
    void LogError(Error error);
    void LogException(Exception ex, ErrorSeverity severity);
  }
}
