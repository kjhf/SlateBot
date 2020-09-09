using Microsoft.VisualStudio.TestTools.UnitTesting;
using SlateBot;
using SlateBot.Commands;
using SlateBot.Commands.Replace;
using SlateBot.Errors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SlateBotUnitTests
{
  [TestClass]
  public class ReplaceTests
  {
    [TestMethod]
    public void TestReplaceCommand()
    {
      bool reacted = false;
      void Controller_OnCommandReceived(object sender, SlateBot.Events.CommandReceivedEventArgs args)
      {
        reacted = true;
      }

      var controller = new SlateBotController();
      controller.OnCommandReceived += Controller_OnCommandReceived;
      controller.Initialise();
      controller.HandleConsoleCommand("react aa");
      Assert.IsTrue(reacted);

      reacted = false;
      controller.HandleConsoleCommand("react ? ?");
      Assert.IsTrue(reacted);
    }
  }
}
