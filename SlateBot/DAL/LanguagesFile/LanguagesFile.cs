using SlateBot.Errors;
using SlateBot.Language;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SlateBot.DAL.LanguagesFile
{
  /// <summary>
  /// A languages XML file.
  /// </summary>
  class LanguagesFile : ISaveData
  {
    private readonly IErrorLogger errorLogger;
    internal Dictionary<string, string> Data { get; set; }
    
    public LanguagesFile(IErrorLogger errorLogger)
    {
      this.errorLogger = errorLogger;
    }

    /// <summary>
    /// Load the <see cref="LanguagesFile"/> from xml.
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

        Data = new Dictionary<string, string>();
        var nodes = doc["resources"];

        foreach (XmlElement node in nodes?.OfType<XmlElement>())
        {
          string key = (node.HasAttribute("name")) ? node.GetAttribute("name") : node.Name;
          string value = node.InnerText.TrimStart('\"').TrimEnd('\"');
          Data.Add(key, value);
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
    /// Converts the <see cref="Data"/> to XML.
    /// </summary>
    /// <returns></returns>
    public string ToXML()
    {
      throw new InvalidOperationException(nameof(Data) + " Languages File cannot be saved. Generate using POEditor.");
    }
  }
}
