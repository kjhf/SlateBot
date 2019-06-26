using System;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SlateBot.Utility
{
  public static class HTTPHelper
  {
    public readonly static Regex URL_REGEX = new Regex(@"(www|http:|https:)+[^\s]+[\w]", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline);

    public static async Task<Tuple<HttpResponseMessage, byte[]>> DownloadFile(string url)
    {
      using (var client = new HttpClient())
      {
        client.MaxResponseContentBufferSize = 0x100000; // 1MB
        client.Timeout = TimeSpan.FromSeconds(3);
        using (HttpResponseMessage result = await client.GetAsync(url, HttpCompletionOption.ResponseContentRead))
        {
          if (result.IsSuccessStatusCode)
          {
            return new Tuple<HttpResponseMessage, byte[]>(result, await result.Content.ReadAsByteArrayAsync());
          }
        }
      }
      return null;
    }
    public static bool IsImageUrl(string URL)
    {
      // First check the URL extension ending
      if (URL.Length > 4)
      {
        string urlExtension = URL.Substring(URL.Length - 5).ToLowerInvariant();
        if (urlExtension.EndsWith(".jpeg") || URL.EndsWith(".jpg") || URL.EndsWith(".png") || URL.EndsWith(".tff") || URL.EndsWith(".gif"))
        {
          return true;
        }
      }

      // If ambiguous, make a request to ask the type of the page.
      var req = (HttpWebRequest)WebRequest.Create(URL);
      req.Method = "HEAD";
      req.AllowAutoRedirect = true;
      req.Timeout = 3000;
      using (var resp = req.GetResponse())
      {
        string contentType = resp.ContentType;
        return contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase) || contentType.Equals("jpeg") || contentType.Equals("png");
      }
    }
  }
}
