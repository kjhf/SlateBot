using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CsHelper
{
  /// <summary>
  /// Library for asynchronous file operations.
  /// </summary>
  public static class FileAsync
  {
    /// <summary>
    /// Read all text in a file asynchronously. Assumes UTF8 encoding.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static Task<string> ReadAllTextAsync(string path)
    {
      return ReadAllTextAsync(path, Encoding.UTF8);
    }

    /// <summary>
    /// Read all text in a file asynchronously.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="encoding"></param>
    /// <returns></returns>
    public static async Task<string> ReadAllTextAsync(string path, Encoding encoding)
    {
      if (path == null)
      {
        throw new ArgumentNullException(nameof(path));
      }

      if (encoding == null)
      {
        throw new ArgumentNullException(nameof(encoding));
      }

      string result;
      using (var reader = new StreamReader(path, encoding))
      {
        result = await reader.ReadToEndAsync();
      }

      return result;
    }

    /// <summary>
    /// Read all text in a file and asynchronously return delimited by line. Assumes UTF8 encoding.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static Task<string[]> ReadAllLinesAsync(string path)
    {
      return ReadAllLinesAsync(path, Encoding.UTF8);
    }

    /// <summary>
    /// Read all text in a file and asynchronously return delimited by line.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="encoding"></param>
    /// <returns></returns>
    public static async Task<string[]> ReadAllLinesAsync(string path, Encoding encoding)
    {
      if (path == null)
      {
        throw new ArgumentNullException(nameof(path));
      }

      if (encoding == null)
      {
        throw new ArgumentNullException(nameof(encoding));
      }

      var lines = new List<string>();
      using (var reader = new StreamReader(path, encoding))
      {
        string line;
        while ((line = await reader.ReadLineAsync()) != null)
        {
          lines.Add(line);
        }
      }

      return lines.ToArray();
    }

    /// <summary>
    /// Write all text to a path asynchronously. Saves in UTF8 encoding.
    /// Specify the file mode, by default if the file already exists, it is overwritten.
    /// </summary>
    /// <param name="fileMode"></param>
    /// <param name="lines"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    public static Task WriteAllLinesAsync(string path, IEnumerable<string> lines, FileMode fileMode = FileMode.Create)
    {
      return WriteAllLinesAsync(path, lines, fileMode, Encoding.UTF8);
    }

    /// <summary>
    /// Write all text lines to a path asynchronously.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="fileMode"></param>
    /// <param name="lines"></param>
    /// <param name="encoding"></param>
    /// <returns></returns>
    public static async Task WriteAllLinesAsync(string path, IEnumerable<string> lines, FileMode fileMode, Encoding encoding)
    {
      if (path == null)
      {
        throw new ArgumentNullException(nameof(path));
      }

      if (lines == null)
      {
        throw new ArgumentNullException(nameof(lines));
      }

      if (encoding == null)
      {
        throw new ArgumentNullException(nameof(encoding));
      }

      using (FileStream sourceStream = new FileStream(path, fileMode, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
      {
        foreach (string line in lines)
        {
          byte[] encodedText = encoding.GetBytes(line + Environment.NewLine);
          await sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
        }
      };
    }

    /// <summary>
    /// Write all text to a path asynchronously. Saves in UTF8 encoding.
    /// Specify the file mode, by default if the file already exists, it is overwritten.
    /// </summary>
    /// <param name="fileMode"></param>
    /// <param name="path"></param>
    /// <param name="text"></param>
    /// <returns></returns>
    public static Task WriteAllTextAsync(string path, string text, FileMode fileMode = FileMode.Create)
    {
      return WriteAllTextAsync(path, text, fileMode, Encoding.UTF8);
    }

    /// <summary>
    /// Write all text to a path asynchronously.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="text"></param>
    /// <param name="fileMode"></param>
    /// <param name="encoding"></param>
    /// <returns></returns>
    public static async Task WriteAllTextAsync(string path, string text, FileMode fileMode, Encoding encoding)
    {
      if (path == null)
      {
        throw new ArgumentNullException(nameof(path));
      }

      if (text == null)
      {
        throw new ArgumentNullException(nameof(text));
      }

      if (encoding == null)
      {
        throw new ArgumentNullException(nameof(encoding));
      }

      var encodedText = encoding.GetBytes(text);

      using (FileStream sourceStream = new FileStream(path, fileMode, FileAccess.Write, FileShare.Read, bufferSize: 4096, useAsync: true))
      {
        await sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
      };
    }
  }
}