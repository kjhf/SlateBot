using CsHelper;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SlateBot.Utility
{
  public static class JSONHelper
  {
    public static async Task<object> GetJsonAsync(string website, string cookies = null)
    {
      using WebClient client = new WebClient { Encoding = Encoding.UTF8 };
      if (cookies != null)
      {
        client.Headers.Add(HttpRequestHeader.Cookie, cookies);
      }

      Uri uri = new Uri(website);
      string value = await client.DownloadStringTaskAsync(uri).ConfigureAwait(false);
      return JsonConvert.DeserializeObject(value);
    }

    /// <summary>
    /// Load JSON object from a file.
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static async Task<object> LoadJSONFromFile(string filePath)
    {
      string value = await FileAsync.ReadAllTextAsync(filePath).ConfigureAwait(false);
      return JsonConvert.DeserializeObject(value);
    }
  }
}