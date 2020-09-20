using Microsoft.VisualStudio.TestTools.UnitTesting;
using SlateBot;
using SlateBot.Commands;
using SlateBot.Commands.Dice;
using SlateBot.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SlateBotUnitTests
{
  [TestClass]
  public class DiceTests
  {
    [TestMethod]
    public void TestDiceCommand()
    {
      Response receivedResponse = null;
      void Controller_OnCommandReceived(object sender, SlateBot.Events.CommandReceivedEventArgs args)
      {
        receivedResponse = args.response;
      }

      var controller = new SlateBotController();
      controller.OnCommandReceived += Controller_OnCommandReceived;
      controller.Initialise();
      controller.HandleConsoleCommand("dice 10");
      Assert.IsNotNull(receivedResponse);
      Assert.IsTrue(receivedResponse.Message.StartsWith(Emojis.Dice));
    }

    [TestMethod]
    public void TestRollCommand()
    {
      Response receivedResponse = null;
      void Controller_OnCommandReceived(object sender, SlateBot.Events.CommandReceivedEventArgs args)
      {
        receivedResponse = args.response;
      }

      var controller = new SlateBotController();
      controller.OnCommandReceived += Controller_OnCommandReceived;
      controller.Initialise();
      controller.HandleConsoleCommand("roll 10");
      Assert.IsNotNull(receivedResponse);
      Assert.IsTrue(receivedResponse.Message.StartsWith(Emojis.Dice));
    }

    [TestMethod]
    public void TestSuccessfulDVanillaCommand()
    {
      Response receivedResponse = null;
      void Controller_OnCommandReceived(object sender, SlateBot.Events.CommandReceivedEventArgs args)
      {
        receivedResponse = args.response;
      }

      var controller = new SlateBotController();
      controller.OnCommandReceived += Controller_OnCommandReceived;
      controller.Initialise();

      controller.HandleConsoleCommand("2d4");
      Assert.IsNotNull(receivedResponse);
      Assert.IsTrue(receivedResponse.Message.StartsWith(Emojis.Dice));
      receivedResponse = null;

      controller.HandleConsoleCommand("1d10");
      Assert.IsNotNull(receivedResponse);
      Assert.IsTrue(receivedResponse.Message.StartsWith(Emojis.Dice));
      receivedResponse = null;

      controller.HandleConsoleCommand("10d5");
      Assert.IsNotNull(receivedResponse);
      Assert.IsTrue(receivedResponse.Message.StartsWith(Emojis.Dice));
      receivedResponse = null;
    }

    [TestMethod]
    public void TestSuccessfulDVanillaNoPrefixCommand()
    {
      Response receivedResponse = null;
      void Controller_OnCommandReceived(object sender, SlateBot.Events.CommandReceivedEventArgs args)
      {
        receivedResponse = args.response;
      }

      var controller = new SlateBotController();
      controller.OnCommandReceived += Controller_OnCommandReceived;
      controller.Initialise();

      controller.HandleConsoleCommand("d4");
      Assert.IsNotNull(receivedResponse);
      Assert.IsTrue(receivedResponse.Message.StartsWith(Emojis.Dice));
      receivedResponse = null;

      controller.HandleConsoleCommand("d10");
      Assert.IsNotNull(receivedResponse);
      Assert.IsTrue(receivedResponse.Message.StartsWith(Emojis.Dice));
      receivedResponse = null;

      controller.HandleConsoleCommand("d5");
      Assert.IsNotNull(receivedResponse);
      Assert.IsTrue(receivedResponse.Message.StartsWith(Emojis.Dice));
      receivedResponse = null;
    }

    [TestMethod]
    public void TestUnsuccessfulDVanillaNoSuffixCommand()
    {
      Response receivedResponse = null;
      void Controller_OnCommandReceived(object sender, SlateBot.Events.CommandReceivedEventArgs args)
      {
        receivedResponse = args.response;
      }

      var controller = new SlateBotController();
      controller.OnCommandReceived += Controller_OnCommandReceived;
      controller.Initialise();

      controller.HandleConsoleCommand("2d");
      Assert.IsNull(receivedResponse);

      controller.HandleConsoleCommand("1d");
      Assert.IsNull(receivedResponse);

      controller.HandleConsoleCommand("10d");
      Assert.IsNull(receivedResponse);
    }

    [TestMethod]
    public void TestSuccessfulDWithForceAndModifiersCommand()
    {
      Response receivedResponse = null;
      void Controller_OnCommandReceived(object sender, SlateBot.Events.CommandReceivedEventArgs args)
      {
        receivedResponse = args.response;
      }

      var controller = new SlateBotController();
      controller.OnCommandReceived += Controller_OnCommandReceived;
      controller.Initialise();

      controller.HandleConsoleCommand("2DF-1");
      Assert.IsNotNull(receivedResponse);
      Assert.IsTrue(receivedResponse.Message.StartsWith(Emojis.Dice));
      receivedResponse = null;

      controller.HandleConsoleCommand("3dF +2");
      Assert.IsNotNull(receivedResponse);
      Assert.IsTrue(receivedResponse.Message.StartsWith(Emojis.Dice));
      receivedResponse = null;

      controller.HandleConsoleCommand("4d10-3");
      Assert.IsNotNull(receivedResponse);
      Assert.IsTrue(receivedResponse.Message.StartsWith(Emojis.Dice));
      receivedResponse = null;

      controller.HandleConsoleCommand("2d100 -4");
      Assert.IsNotNull(receivedResponse);
      Assert.IsTrue(receivedResponse.Message.StartsWith(Emojis.Dice));
      receivedResponse = null;
    }

    [TestMethod]
    public void TestDiceRegexDoesNotConflict()
    {
      var controller = new SlateBotController();
      controller.Initialise();
      List<Command> allCommands = new List<Command>();
      foreach (Languages language in Enum.GetValues(typeof(Languages)))
      {
        allCommands.AddRange(controller.commandHandlerController.GetCommandsForLanguage(language, false));
      }
      Assert.IsTrue(allCommands.Count > 3, "All commands not loaded?");

      // Test if any aliases match the dice Regex.
      string[] matchedRegexFailures = allCommands.SelectMany(c => c.Aliases.Where(alias => Regex.IsMatch(alias, DiceCommand.D_REGEX))).ToArray();
      Assert.IsTrue(matchedRegexFailures.Length == 0, $"Failed aliases: {string.Join(", ", matchedRegexFailures)}");
    }
  }
}