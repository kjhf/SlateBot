using SlateBot.Errors;
using SlateBot.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace SlateBot.DAL.UserSettingsFile
{
  /// <summary>
  /// A User Settings XML file.
  /// </summary>
  public class UserSettingsFile : ISaveData
  {
    private readonly IErrorLogger errorLogger;
    
    /// <summary>
    /// The user id this file belongs to
    /// </summary>
    public string UserId { get; set; } = Constants.ErrorId.ToString();
    public string CommandsCount { get; private set; }
    
    public UserSettingsFile(IErrorLogger errorLogger)
    {
      this.errorLogger = errorLogger;
    }

    /// <summary>
    /// Load the <see cref="UserSettingsFile"/> from xml.
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

        CommandsCount = (root["UserStats"]["CommandsCount"].InnerText);
        UserId = (root["UserId"].InnerText);
      }
      catch (Exception ex)
      {
        retVal = false;
        errorLogger.LogException(ex, ErrorSeverity.Error);
      }

      return retVal;
    }

    /// <summary>
    /// Initialise this file with the data to write.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="extraData"></param>
    public void Initialise(UserSettings userSettings)
    {
      this.UserId = userSettings.UserId.ToString();
      this.CommandsCount = userSettings.UserStats.commandsIssued.ToString();
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
        node = doc.CreateElement("UserId");
        node.InnerText = UserId;
        root.AppendChild(node);
        XmlElement userStatsNode = doc.CreateElement("UserStats");
        root.AppendChild(userStatsNode);
        node = doc.CreateElement("CommandsCount");
        node.InnerText = CommandsCount;
        userStatsNode.AppendChild(node);

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
      return $"{nameof(UserSettingsFile)}: {UserId}";
    }
  }
}