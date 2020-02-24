﻿using Microsoft.CSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.DrawingCore;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SlateBot.Imaging
{
  public static class ImageManipulator
  {
    /// <summary>
    /// Creates a Color from alpha, hue, saturation and brightness.
    /// </summary>
    /// <param name="alpha">The alpha channel value.</param>
    /// <param name="hue">The hue value. Note hue is from 0-360 degrees.</param>
    /// <param name="saturation">The saturation value from 0-1.</param>
    /// <param name="brightness">The brightness value from 0-1.</param>
    /// <returns>A Color with the given values.</returns>
    public static Color FromAHSB(byte alpha, float hue, float saturation, float brightness)
    {
      Contract.Requires(0f <= hue && hue <= 360f);
      Contract.Requires(0f <= saturation && saturation <= 1f);
      Contract.Requires(0f <= brightness && brightness <= 1f);
      Contract.EndContractBlock();

      if (0 == saturation)
      {
        return Color.FromArgb(
                            alpha,
                            Convert.ToInt32(brightness * 255),
                            Convert.ToInt32(brightness * 255),
                            Convert.ToInt32(brightness * 255));
      }

      float fMax, fMid, fMin;
      int iSextant, iMax, iMid, iMin;

      if (0.5 < brightness)
      {
        fMax = brightness - brightness * saturation + saturation;
        fMin = brightness + brightness * saturation - saturation;
      }
      else
      {
        fMax = brightness + brightness * saturation;
        fMin = brightness - brightness * saturation;
      }

      iSextant = (int)Math.Floor(hue / 60f);
      if (300f <= hue)
      {
        hue -= 360f;
      }

      hue /= 60f;
      hue -= 2f * (float)Math.Floor((iSextant + 1f) % 6f / 2f);
      if (0 == iSextant % 2)
      {
        fMid = hue * (fMax - fMin) + fMin;
      }
      else
      {
        fMid = fMin - hue * (fMax - fMin);
      }

      iMax = Convert.ToInt32(fMax * 255);
      iMid = Convert.ToInt32(fMid * 255);
      iMin = Convert.ToInt32(fMin * 255);

      switch (iSextant)
      {
        case 1:
          return Color.FromArgb(alpha, iMid, iMax, iMin);
        case 2:
          return Color.FromArgb(alpha, iMin, iMax, iMid);
        case 3:
          return Color.FromArgb(alpha, iMin, iMid, iMax);
        case 4:
          return Color.FromArgb(alpha, iMid, iMin, iMax);
        case 5:
          return Color.FromArgb(alpha, iMax, iMin, iMid);
        default:
          return Color.FromArgb(alpha, iMax, iMid, iMin);
      }
    }

    public static void ChangeHue(Bitmap image, int hueChange)
    {
      for (int x = 0; x < image.Width; x++)
      {
        for (int y = 0; y < image.Height; y++)
        {
          Color px = image.GetPixel(x, y);
          byte alpha = px.A;
          float hue = (px.GetHue() + hueChange) % 360;
          float sat = px.GetSaturation();
          float bri = px.GetBrightness();
          image.SetPixel(x, y, FromAHSB(alpha, hue, sat, bri));
        }
      }
    }

    public static IDictionary<Color, uint> EvaluateColours(Bitmap image)
    {
      var retVal = new Dictionary<Color, uint>();
      for (int x = 0; x < image.Width; x++)
      {
        for (int y = 0; y < image.Height; y++)
        {
          Color px = image.GetPixel(x, y);
          if (retVal.ContainsKey(px))
          {
            ++retVal[px];
          }
          else
          {
            retVal.Add(px, 1);
          }
        }
      }
      return retVal;
    }

    public static string GreyscaleImageToASCII(Image img)
    {
      StringBuilder output = new StringBuilder();

      // Create a bitmap from the image
      using (Bitmap bmp = new Bitmap(img))
      {
        try
        {
          // Loop through each pixel in the bitmap
          for (int y = 0; y < bmp.Height; y++)
          {
            for (int x = 0; x < bmp.Width; x++)
            {
              // Get the colour of the current pixel
              Color col = bmp.GetPixel(x, y);

              // To convert to greyscale, the easiest method is to add
              // the R+G+B colours and divide by three to get the greyscaled colour.
              col = Color.FromArgb(
                (col.R + col.G + col.B) / 3,
                (col.R + col.G + col.B) / 3,
                (col.R + col.G + col.B) / 3);

              // Get the Red value from the greyscale colour,
              // parse to an int [0-255]
              int rValue = int.Parse(col.R.ToString());

              // Append the "colour" using various darknesses of ASCII character.
              output.Append(GetAsciiChar(rValue));

              // If we're at the width, insert a line break
              if (x == bmp.Width - 1 && y != bmp.Height - 1)
              {
                output.Append("\n");
              }
            }
          }
        }
        catch (Exception exc)
        {
          output.AppendLine(exc.ToString());
        }
      }
      return output.ToString();
    }

    public static void PerformObabo(Bitmap image, MirrorType mirrorType)
    {
      int width = image.Width;
      int height = image.Height;

      switch (mirrorType)
      {
        case MirrorType.LeftOntoRight:
        {
          for (int x = 0; x < width / 2; x++)
          {
            for (int y = 0; y < image.Height; y++)
            {
              Color c = image.GetPixel(x, y);
              image.SetPixel(width - 1 - x, y, c);
            }
          }
          break;
        }

        case MirrorType.RightOntoLeft:
        {
          for (int x = 0; x < width / 2; x++)
          {
            for (int y = 0; y < image.Height; y++)
            {
              Color c = image.GetPixel(width - 1 - x, y);
              image.SetPixel(x, y, c);
            }
          }
          break;
        }

        case MirrorType.TopOntoBottom:
        {
          for (int y = 0; y < height / 2; y++)
          {
            for (int x = 0; x < width; x++)
            {
              Color c = image.GetPixel(x, y);
              image.SetPixel(x, height - 1 - y, c);
            }
          }
          break;
        }

        case MirrorType.BottomOntoTop:
        {
          for (int y = 0; y < height / 2; y++)
          {
            for (int x = 0; x < width; x++)
            {
              Color c = image.GetPixel(x, height - 1 - y);
              image.SetPixel(x, y, c);
            }
          }
          break;
        }
      }
    }

    /// <summary>
    /// Performs a meme and returns the file path.
    /// </summary>
    public static string PerformMeme(Bitmap overlay, string templatePath, Point[] destinationPoints)
    {
      string filePath;
      using (Image i = Image.FromFile(templatePath))
      {
        using (Bitmap template = new Bitmap(i))
        {
          using (Bitmap result = new Bitmap(template.Width, template.Height))
          {
            result.MakeTransparent(Color.White);
            
            using (Graphics g = Graphics.FromImage(result))
            {
              g.DrawImage(overlay, destinationPoints);
              g.DrawImage(template, 0, 0);

              for (int x = 0; x < overlay.Width; x++)
              {
                for (int y = 0; y < overlay.Height; y++)
                {
                  if (overlay.GetPixel(x, y).A == 0)
                  {
                    overlay.SetPixel(x, y, Color.White);
                  }
                }
              }

              g.Save();
            }

            filePath = Path.GetTempFileName() + ".png";
            result.Save(filePath);
          }
        }
      }
      return filePath;
    }

    /// <summary>
    /// Generate an image based off of <paramref name="image"/> with 
    /// wavy rows.
    /// Neither Bitmaps are disposed.
    /// </summary>
    /// <param name="image"></param>
    /// <returns></returns>
    public static Bitmap PerformWave(Bitmap image)
    {
      int maxChange = Math.Max(10, (int)(image.Width * 0.2) + 1);
      Random rand = new Random();
      bool leanRight = rand.Next(0, 2) != 0;

      Bitmap wave = new Bitmap(image.Width + maxChange * 2, image.Height);
      wave.MakeTransparent(Color.Black);

      int offset = maxChange;
      for (int y = 0; y < image.Height; y++)
      {
        for (int x = 0; x < image.Width; x++)
        {
          Color c = image.GetPixel(x, y);
          // Make sure transparent does not collide.
          if (c == Color.Black)
          {
            c = Color.FromArgb(255, 0, 0, 1);
          }
          wave.SetPixel(x + offset, y, c);
        }

        int result = rand.Next(0, 3);
        switch (result)
        {
          case 0: // Going left!
            offset = Math.Max(0, offset - 1);
            break;

          case 1: // Going right!
            offset = Math.Min(maxChange * 2, offset + 1);
            break;

          case 2: // Lean!
            offset = leanRight ? Math.Min(maxChange * 2, offset + 1) : Math.Max(0, offset - 1);
            break;
        }
      }

      return wave;
    }

    private static string GetAsciiChar(int redValue)
    {
      switch (redValue)
      {
        case int testVal when testVal >= 250: return " ";
        case int testVal when testVal >= 240: return ".";
        case int testVal when testVal >= 230: return "'";
        case int testVal when testVal >= 220: return "^";
        case int testVal when testVal >= 200: return "\"";
        case int testVal when testVal >= 180: return ":";
        case int testVal when testVal >= 170: return ";";
        case int testVal when testVal >= 160: return "!";
        case int testVal when testVal >= 150: return "i";
        case int testVal when testVal >= 140: return "~";
        case int testVal when testVal >= 120: return "+";
        case int testVal when testVal >= 100: return "o";
        case int testVal when testVal >= 80: return "X";
        case int testVal when testVal >= 60: return "#";
        case int testVal when testVal >= 40: return "%";
        case int testVal when testVal >= 20: return "@";
        default: return "$";
      }
    }

    /// <summary>
    /// Generates a square Bitmap of the specified colour and size (pixels across).
    /// </summary>
    /// <param name="c"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    public static Bitmap GenerateBitmapFromColour(Color c, int size = 100)
    {
      Bitmap b = new Bitmap(size, size);
      using (Graphics g = Graphics.FromImage(b))
      {
        g.Clear(c);
      }

      return b;
    }

    public static Color FromHexCode(string code, bool setOpaque = true)
    {
      string colorcode = code;
      uint argb = uint.Parse(colorcode.Replace("#", ""), NumberStyles.HexNumber);
      if (setOpaque)
      {
        argb |= 0xFF000000;
      }
      Color c = Color.FromArgb((int)argb);
      return c;
    }

    /// <summary>
    /// Find the first hex colour in a string
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool FindHexColour(string input, out string match)
    {
      const string HexCodeRegex = @"(\b#?([\da-fA-F]{6})\b)\b)";
      match = Regex.Match(input, HexCodeRegex, RegexOptions.Compiled)?.Value;
      return !string.IsNullOrEmpty(match);
    }

    public static int FunctionARGB(byte a, byte r, byte g, byte b)
    {
      return a << 24 | 255 - Math.Max(Math.Max(r, g), b) << 16 | 255 - Math.Max(Math.Max(r, g), b) << 8 | 255 - Math.Max(Math.Max(r, g), b);
    }

    //https://stackoverflow.com/questions/1988833/converting-color-to-consolecolor
    public static ConsoleColor ToConsoleColor(this System.Drawing.Color c)
    {
      int index = (c.R > 128 | c.G > 128 | c.B > 128) ? 8 : 0; // Bright bit
      index |= (c.R > 64) ? 4 : 0; // Red bit
      index |= (c.G > 64) ? 2 : 0; // Green bit
      index |= (c.B > 64) ? 1 : 0; // Blue bit
      return (ConsoleColor)index;
    }
  }
}
