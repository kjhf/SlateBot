using SlateBot.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace SlateBot.DAL.CommandFile
{
  /// <summary>
  /// A command XML file.
  /// </summary>
  public class CommandFile : ISaveData
  {
    private readonly IErrorLogger errorLogger;

    /// <summary> The command's handler type. </summary>
    public string CommandType { get; private set; }

    /// <summary> The command's module. </summary>
    public string Module { get; private set; }

    /// <summary> Array of aliases this command accepts. </summary>
    public string[] Aliases { get; private set; }

    /// <summary> Aliases this command accepts as a display string </summary>
    public string AliasesStr => $"[{string.Join(" ", Aliases)}]";

    /// <summary> What to display if the sender requests help for this command </summary>
    public string Help { get; private set; }

    /// <summary> Examples for this command </summary>
    public string Examples { get; private set; }

    /// <summary> How the handler should use this command's response. </summary>
    public string ResponseType { get; private set; }

    /// <summary> Additional data to load or save, keys are node names, values are node values. </summary>
    public Dictionary<string, string> ExtraData { get; private set; }

    public CommandFile(IErrorLogger errorLogger)
    {
      this.errorLogger = errorLogger;
    }

    /// <summary>
    /// Load the <see cref="CommandFile"/> from xml.
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
        
        Aliases = root["Aliases"].InnerText.Split(',').Select(s => s.Trim()).ToArray();
        CommandType = root["CommandType"].InnerText;
        Examples = root["Examples"].InnerText;
        Help = root["Help"].InnerText;
        Module = (root["Module"]?.InnerText) ?? Commands.ModuleType.General.ToString();
        ResponseType = (root["ResponseType"]?.InnerText) ?? Commands.ResponseType.Default.ToString();

        ExtraData = new Dictionary<string, string>();
        var extraDataNodes = root["ExtraData"];
        foreach (XmlElement node in extraDataNodes?.OfType<XmlElement>() ?? new XmlElement[0])
        {
          ExtraData[node.Name] = node.InnerText;
        }
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
    public void Initialise(Commands.Command command)
    {
      this.Aliases = command.Aliases;
      this.CommandType = command.CommandHandlerType.ToString();
      this.Examples = command.Examples;
      this.ExtraData = command.ExtraData;
      this.Help = command.Help;
      this.Module = command.Module.ToString();
      this.ResponseType = command.ResponseType.ToString();
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
        XmlNode root = doc.CreateElement("Root");

        var node = doc.CreateElement("Aliases");
        node.InnerText = string.Join(",", Aliases);
        root.AppendChild(node);
        node = doc.CreateElement("CommandType");
        node.InnerText = CommandType;
        root.AppendChild(node);
        node = doc.CreateElement("Examples");
        node.InnerText = Examples;
        root.AppendChild(node);
        node = doc.CreateElement("Help");
        node.InnerText = Help;
        root.AppendChild(node);
        node = doc.CreateElement("Module");
        node.InnerText = Module;
        root.AppendChild(node);
        node = doc.CreateElement("ResponseType");
        node.InnerText = ResponseType;
        root.AppendChild(node);
        var extraDataNode = doc.CreateElement("ExtraData");
        doc.AppendChild(extraDataNode);

        foreach (var pair in ExtraData)
        {
          node = doc.CreateElement(pair.Key);
          node.InnerText = pair.Value;
          extraDataNode.AppendChild(node);
        }

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
      return $"{nameof(CommandFile)}: {CommandType} in {Module}: {AliasesStr}";
    }
  }
}