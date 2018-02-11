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
    public IList<string> JoinServerMessages { get; private set; } = new List<string>();

    /// <summary>
    /// The server's language
    /// </summary>
    public string Language { get; set; } = (Languages.English).ToString();

    /// <summary>
    /// Quit messages
    /// </summary>
    public IList<string> QuitServerMessages { get; private set; } = new List<string>();

    /// <summary>
    /// Channels that the bot will add ratings to
    /// </summary>
    public HashSet<string> RateChannels { get; private set; } = new HashSet<string>();
    
    //public IList<RepeatingCommandData> RepeatingChannels { get; private set; } = new List<RepeatingCommandData>();

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
        foreach (XmlElement node in blockedModuleNodes?.OfType<XmlElement>())
        {
          BlockedModules.Add(node.InnerText);
        }
        CommandSymbol = root["CommandSymbol"].InnerText;
        JoinQuitChannelId = (root["JoinQuitChannelId"].InnerText);
        var joinServerMessageNodes = root["JoinServerMessages"];
        foreach (XmlElement node in joinServerMessageNodes?.OfType<XmlElement>())
        {
          JoinServerMessages.Add(node.InnerText);
        }
        Language = root["Language"].InnerText;
        var quitServerMessageNodes = root["QuitServerMessages"];
        foreach (XmlElement node in quitServerMessageNodes?.OfType<XmlElement>())
        {
          QuitServerMessages.Add(node.InnerText);
        }
        var rateChannelNodes = root["RateChannels"];
        foreach (XmlElement node in rateChannelNodes?.OfType<XmlElement>())
        {
          RateChannels.Add(node.InnerText);
        }
        var splatoon2RotationChannelNodes = root["Splatoon2RotationChannels"];
        foreach (XmlElement node in splatoon2RotationChannelNodes?.OfType<XmlElement>())
        {
          Splatoon2RotationChannels.Add(node.InnerText);
        }
        ServerId = (root["ServerId"].InnerText);
        TrackDeletedMessages = bool.Parse(root["TrackDeletedMessages"].InnerText);
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
        doc.CreateXmlDeclaration("1.0", "UTF-8", "yes");
        XmlElement root = doc.CreateElement("Root");
        XmlElement node;

        var blockedModulesNode = doc.CreateElement("BlockedModule");
        doc.AppendChild(blockedModulesNode);
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
        doc.AppendChild(joinServerMessagesNode);
        foreach (var message in JoinServerMessages)
        {
          node = doc.CreateElement("M");
          node.InnerText = message;
          joinServerMessagesNode.AppendChild(node);
        }
        node = doc.CreateElement("Language");
        node.InnerText = Language;
        root.AppendChild(node);
        var quitServerMessagesNode = doc.CreateElement("QuitServerMessages");
        doc.AppendChild(quitServerMessagesNode);
        foreach (var message in QuitServerMessages)
        {
          node = doc.CreateElement("M");
          node.InnerText = message;
          quitServerMessagesNode.AppendChild(node);
        }
        var rateChannelsNode = doc.CreateElement("RateChannels");
        doc.AppendChild(rateChannelsNode);
        foreach (var channel in RateChannels)
        {
          node = doc.CreateElement("C");
          node.InnerText = channel;
          rateChannelsNode.AppendChild(node);
        }
        node = doc.CreateElement("ServerId");
        node.InnerText = ServerId;
        root.AppendChild(node);
        var splatoon2RotationChannelsNode = doc.CreateElement("Splatoon2RotationChannels");
        doc.AppendChild(splatoon2RotationChannelsNode);
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