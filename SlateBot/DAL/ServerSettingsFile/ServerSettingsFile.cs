using SlateBot.Commands.Schedule;
using SlateBot.Errors;
using SlateBot.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace SlateBot.DAL.ServerSettingsFile
{
  /// <summary>
  /// A Server Settings XML file.
  /// </summary>
  public class ServerSettingsFile : ISaveData
  {
    private readonly IErrorLogger errorLogger;

    /// <summary>
    /// The Blocked Modules that this server disallows.
    /// </summary>
    public HashSet<string> BlockedModules { get; private set; } = new HashSet<string>();

    /// <summary>
    /// The Command Symbol for this server
    /// </summary>
    public string CommandSymbol { get; set; } = "!";

    /// <summary>
    /// The Join/Quit message channel id in this server
    /// </summary>
    public string JoinQuitChannelId { get; set; } = Constants.ErrorId.ToString();

    /// <summary>
    /// Join messages
    /// </summary>
    public List<string> JoinServerMessages { get; private set; } = new List<string>();

    /// <summary>
    /// The server's language
    /// </summary>
    public string Language { get; set; } = (Languages.English).ToString();

    /// <summary>
    /// Quit messages
    /// </summary>
    public List<string> QuitServerMessages { get; private set; } = new List<string>();

    /// <summary>
    /// Channels that the bot will add ratings to
    /// </summary>
    public HashSet<string> RateChannels { get; private set; } = new HashSet<string>();

    /// <summary>
    /// Messages that are scheduled on this server
    /// </summary>
    public List<ScheduledMessageData> ScheduledMessages { get; internal set; } = new List<ScheduledMessageData>();

    /// <summary>
    /// Channels that announce Splatoon 2 rotations
    /// </summary>
    public HashSet<string> Splatoon2RotationChannels { get; private set; } = new HashSet<string>();

    //public IList<RSSChannelData> RSSFeedChannels { get; private set; } = new List<RSSChannelData>();

    /// <summary>
    /// The server id this file belongs to
    /// </summary>
    public string ServerId { get; set; } = Constants.ErrorId.ToString();

    /// <summary>
    /// Should the bot track deleted messages?
    /// </summary>
    public bool TrackDeletedMessages { get; set; } = false;

    public ServerSettingsFile(IErrorLogger errorLogger)
    {
      this.errorLogger = errorLogger;
    }

    /// <summary>
    /// Load the <see cref="ServerSettingsFile"/> from xml.
    /// Returns success.
    /// </summary>
    /// <param name="xml"></param>
    /// <returns></returns>
    public bool FromXML(string xml)
    {
      bool retVal = true;
      try
      {
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(xml);
        XmlNode root = doc.LastChild; // FirstChild is decl

        var blockedModuleNodes = root["BlockedModules"];
        if (blockedModuleNodes != null)
        {
          foreach (XmlElement node in blockedModuleNodes.OfType<XmlElement>())
          {
            BlockedModules.Add(node.InnerText);
          }
        }
        CommandSymbol = root["CommandSymbol"].InnerText;
        JoinQuitChannelId = (root["JoinQuitChannelId"].InnerText);
        var joinServerMessageNodes = root["JoinServerMessages"];
        if (joinServerMessageNodes != null)
        {
          foreach (XmlElement node in joinServerMessageNodes.OfType<XmlElement>())
          {
            JoinServerMessages.Add(node.InnerText);
          }
        }
        Language = root["Language"].InnerText;
        var quitServerMessageNodes = root["QuitServerMessages"];
        if (quitServerMessageNodes != null)
        {
          foreach (XmlElement node in quitServerMessageNodes.OfType<XmlElement>())
          {
            QuitServerMessages.Add(node.InnerText);
          }
        }
        var rateChannelNodes = root["RateChannels"];
        if (rateChannelNodes != null)
        {
          foreach (XmlElement node in rateChannelNodes.OfType<XmlElement>())
          {
            RateChannels.Add(node.InnerText);
          }
        }
        var scheduledMessagesNode = root["ScheduledMessages"];
        if (scheduledMessagesNode != null)
        {
          foreach (XmlElement node in scheduledMessagesNode.OfType<XmlElement>())
          {
            ulong channelId = ulong.Parse(node.GetElementsByTagName("Channel")[0].InnerText);
            ushort id = ushort.Parse(node.GetElementsByTagName("Id")[0].InnerText);
            ScheduledMessages.Add(
              new ScheduledMessageData(channelId, id)
              {
                enabled = bool.Parse(node.GetElementsByTagName("Enabled")[0].InnerText),
                message = node.GetElementsByTagName("Message")[0].InnerText,
                nextDue = new DateTime(long.Parse(node.GetElementsByTagName("NextDueTicks")[0].InnerText)),
                repetitionTimeSpan = new TimeSpan(long.Parse(node.GetElementsByTagName("RepetitionTicks")[0].InnerText)),
              }
            );
          }
        }
        var splatoon2RotationChannelNodes = root["Splatoon2RotationChannels"];
        if (splatoon2RotationChannelNodes != null)
        {
          foreach (XmlElement node in splatoon2RotationChannelNodes.OfType<XmlElement>())
          {
            Splatoon2RotationChannels.Add(node.InnerText);
          }
        }
        ServerId = (root["ServerId"].InnerText);
        TrackDeletedMessages = bool.Parse(root["TrackDeletedMessages"]?.InnerText ?? "false");
      }
      catch (Exception ex)
      {
        retVal = false;
        errorLogger.LogException(ex, ErrorSeverity.Error);
      }

      return retVal;
    }

    /// <summary>
    /// Initialise this command file with the command to write.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="extraData"></param>
    public void Initialise(ServerSettings serverSettings)
    {
      this.BlockedModules = new HashSet<string>(serverSettings.BlockedModules.Select(m => m.ToString()));
      this.CommandSymbol = serverSettings.CommandSymbol;
      this.JoinQuitChannelId = serverSettings.JoinQuitChannelId.ToString();
      this.JoinServerMessages = serverSettings.JoinServerMessages;
      this.Language = serverSettings.Language.ToString();
      this.QuitServerMessages = serverSettings.QuitServerMessages;
      this.RateChannels = new HashSet<string>(serverSettings.RateChannels.Select(c => c.ToString()));
      this.ScheduledMessages = serverSettings.ScheduledMessages;
      this.ServerId = serverSettings.ServerId.ToString();
      this.Splatoon2RotationChannels = new HashSet<string>(serverSettings.Splatoon2RotationChannels.Select(c => c.ToString()));
      this.TrackDeletedMessages = serverSettings.TrackDeletedMessages;
    }

    /// <summary>
    /// Converts the <see cref="Data"/> to XML, or null if unsuccessful.
    /// </summary>
    /// <returns></returns>
    public string ToXML()
    {
      try
      {
        XmlDocument doc = new XmlDocument();
        var decl = doc.CreateXmlDeclaration("1.0", "UTF-8", "yes");
        doc.AppendChild(decl);
        XmlElement root = doc.CreateElement("Root");
        doc.AppendChild(root);

        XmlElement node;
        var blockedModulesNode = doc.CreateElement("BlockedModules");
        root.AppendChild(blockedModulesNode);
        foreach (var module in BlockedModules)
        {
          node = doc.CreateElement("M");
          node.InnerText = module;
          blockedModulesNode.AppendChild(node);
        }
        node = doc.CreateElement("CommandSymbol");
        node.InnerText = CommandSymbol;
        root.AppendChild(node);
        node = doc.CreateElement("JoinQuitChannelId");
        node.InnerText = JoinQuitChannelId;
        root.AppendChild(node);
        var joinServerMessagesNode = doc.CreateElement("JoinServerMessages");
        root.AppendChild(joinServerMessagesNode);
        foreach (var message in JoinServerMessages)
        {
          node = doc.CreateElement("J");
          node.InnerText = message;
          joinServerMessagesNode.AppendChild(node);
        }
        node = doc.CreateElement("Language");
        node.InnerText = Language;
        root.AppendChild(node);
        var quitServerMessagesNode = doc.CreateElement("QuitServerMessages");
        root.AppendChild(quitServerMessagesNode);
        foreach (var message in QuitServerMessages)
        {
          node = doc.CreateElement("Q");
          node.InnerText = message;
          quitServerMessagesNode.AppendChild(node);
        }
        var rateChannelsNode = doc.CreateElement("RateChannels");
        root.AppendChild(rateChannelsNode);
        foreach (var channel in RateChannels)
        {
          node = doc.CreateElement("C");
          node.InnerText = channel;
          rateChannelsNode.AppendChild(node);
        }
        var scheduledMessagesNode = doc.CreateElement("ScheduledMessages");
        root.AppendChild(scheduledMessagesNode);
        foreach (var data in ScheduledMessages)
        {
          var parentNode = doc.CreateElement("M");
          scheduledMessagesNode.AppendChild(parentNode);

          node = doc.CreateElement("Channel");
          node.InnerText = data.channelId.ToString();
          parentNode.AppendChild(node);

          node = doc.CreateElement("Id");
          node.InnerText = data.id.ToString();
          parentNode.AppendChild(node);

          node = doc.CreateElement("Enabled");
          node.InnerText = data.enabled.ToString();
          parentNode.AppendChild(node);

          node = doc.CreateElement("Message");
          node.InnerText = data.message;
          parentNode.AppendChild(node);

          node = doc.CreateElement("NextDueTicks");
          node.InnerText = data.nextDue.Ticks.ToString();
          parentNode.AppendChild(node);

          node = doc.CreateElement("RepetitionTicks");
          node.InnerText = data.repetitionTimeSpan.Ticks.ToString();
          parentNode.AppendChild(node);
        }
        node = doc.CreateElement("ServerId");
        node.InnerText = ServerId;
        root.AppendChild(node);
        var splatoon2RotationChannelsNode = doc.CreateElement("Splatoon2RotationChannels");
        root.AppendChild(splatoon2RotationChannelsNode);
        foreach (var channel in Splatoon2RotationChannels)
        {
          node = doc.CreateElement("C");
          node.InnerText = channel;
          splatoon2RotationChannelsNode.AppendChild(node);
        }
        node = doc.CreateElement("TrackDeletedMessages");
        node.InnerText = TrackDeletedMessages.ToString();
        root.AppendChild(node);

        return doc.OuterXml;
      }
      catch (Exception ex)
      {
        errorLogger.LogException(ex, ErrorSeverity.Error);
        return null;
      }
    }

    public override string ToString()
    {
      return $"{nameof(ServerSettingsFile)}: {ServerId}";
    }
  }
}