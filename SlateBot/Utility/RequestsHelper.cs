using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SlateBot.Utility
{
  public static class RequestsHelper
  {
    public static async Task<string> CurlGetCommand(string uri, string origin = null)
    {
      HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);

      request.Accept = "application/json";
      request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.Brotli;
      request.ContentType = "application/json";
      request.KeepAlive = true;
      request.Method = "GET";

      if (origin != null)
      {
        request.Headers.Add("Origin", origin);
      }

      using HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync().ConfigureAwait(false);
      using Stream stream = response.GetResponseStream();
      using StreamReader reader = new StreamReader(stream);
      return await reader.ReadToEndAsync().ConfigureAwait(false);
    }

    public static async Task<string> CurlPostCommand(string uri, string data, string origin = null)
    {
      HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);

      request.Accept = "application/json";
      request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.Brotli;
      request.ContentType = "application/json";
      request.KeepAlive = true;
      request.ContentLength = data.Length;
      request.Method = "POST";

      if (origin != null)
      {
        request.Headers.Add("Origin", origin);
      }

      using (Stream ds = request.GetRequestStream())
      {
        await ds.WriteAsync(Encoding.UTF8.GetBytes(data), 0, data.Length).ConfigureAwait(false);
      }

      using HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync().ConfigureAwait(false);
      using Stream stream = response.GetResponseStream();
      using StreamReader reader = new StreamReader(stream);
      return await reader.ReadToEndAsync().ConfigureAwait(false);
    }
  }
}