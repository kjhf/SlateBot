using System;
using System.Collections.Generic;
using System.Linq;

namespace CsHelper
{
  public class TextParser
  {
    /// <summary> Returns full text the parser was constructed with. </summary>
    public string Text { get; protected set; }

    /// <summary> Returns current position. </summary>
    public int Position { get; protected set; }

    /// <summary> Returns length - current position. </summary>
    public int Remaining { get { return Text.Length - Position; } }

    /// <summary> Returns the remaining text in the reader. </summary>
    public string RemainingText { get { return ExtractRemaining(); } }

    /// <summary> Indicates if the current position is at the end. </summary>
    public bool IsEnd { get { return Position >= Text.Length; } }

    /// <summary>
    /// Constructor of <see cref="TextParser"/>
    /// </summary>
    /// <param name="text"></param>
    public TextParser(string text = null)
    {
      Initialise(text);
    }

    /// <summary>
    /// Sets the current document text and resets the current position to the start.
    /// </summary>
    /// <param name="text">The text to initialise this parser with.</param>
    private void Initialise(string text)
    {
      Text = text ?? string.Empty;
      Position = 0;
    }

    /// <summary>
    /// Returns the character at the specified number of characters beyond the current
    /// position, or a null character if the specified position is at the end.
    /// </summary>
    /// <param name="ahead">The number of characters beyond the current position</param>
    /// <returns>The character at the specified position</returns>
    public char Peek(int ahead = 0)
    {
      int pos = Position + ahead;
      return pos < Text.Length ? Text[pos] : '\0';
    }

    /// <summary>
    /// Returns as a string from the current position until the specified length without consuming any text.
    /// Alias for ExtractUntil(Position + length).
    /// </summary>
    /// <param name="length">The number of characters beyond the current position</param>
    /// <returns>The string at the specified position</returns>
    public string PeekString(int length)
    {
      return ExtractUntil(Position + length);
    }

    /// <summary>
    /// Reads a specified number of characters from the current position and moves
    /// the reader on that much.
    /// </summary>
    /// <param name="length">The number of characters to read.</param>
    public string Read(int length = 1)
    {
      string retVal = PeekString(length);
      MoveAhead(length);
      return retVal;
    }

    /// <summary>
    /// Returns whether the next character to read is equal to the supplied character.
    /// Alias for (Peek().Equals(ch));
    /// </summary>
    /// <param name="ch">Character to test</param>
    /// <returns>The character specified is next to read.</returns>
    public bool IsAt(char ch)
    {
      return Peek().Equals(ch);
    }

    /// <summary>
    /// Returns whether the next characters to read is equal to the supplied string.
    /// Alias for (PeekString(str.Length).Equals(str));
    /// </summary>
    /// <param name="str">The string to test</param>
    /// <returns>The string specified is next to read.</returns>
    public bool IsAt(string str)
    {
      return PeekString(str.Length).Equals(str);
    }

    /// <summary>
    /// Extracts a substring from the current position to the end of the text.
    /// Does not move the reader.
    /// </summary>
    public string ExtractRemaining()
    {
      return ExtractBetween(Position, Text.Length);
    }

    /// <summary>
    /// Extracts a substring between the specified range within the current text.
    /// Does not move the reader.
    /// </summary>
    /// <param name="start">Starting index</param>
    /// <param name="end">Ending index</param>
    public string ExtractBetween(int start, int end)
    {
      return Text.Substring(Math.Max(start, 0), Math.Min(end, Text.Length) - start);
    }

    /// <summary>
    /// Extracts a substring between a start string + its length and end string at their next occurrences.
    /// For example Extract("&lt;a&gt;","&lt;/a&gt;") in a text containing &lt;a&gt;Example&lt;/a&gt;
    /// will return Example.
    /// Returns empty if start or end is missing.
    /// Does not move the reader.
    /// </summary>
    /// <param name="start">Start index of the next instance of the specified string</param>
    /// <param name="end">End index of the next instance of the specified string</param>
    /// <param name="ignoreCase">Ignore case of the strings?</param>
    /// <returns>The string delimited by the specified strings</returns>
    public string ExtractBetween(string start, string end, bool ignoreCase = false)
    {
      int startIndex = NextIndexOf(start, ignoreCase);
      if (startIndex == -1)
      {
        return string.Empty;
      }

      int endIndex = NextIndexOf(startIndex, end, ignoreCase);
      return endIndex == -1 ? string.Empty : ExtractBetween(startIndex + start.Length, endIndex);
    }

    /// <summary>
    /// Extracts a substring between a start string + its length and end string at their next occurrences.
    /// For example Extract("&lt;a&gt;","&lt;/a&gt;") in a text containing &lt;a&gt;Example&lt;/a&gt;
    /// will return Example.
    /// Returns empty if start or end is missing.
    /// Does not move the reader.
    /// </summary>
    /// <param name="start">Start index of the next instance of the specified string</param>
    /// <param name="end">End index of the next instance of any matching specified string</param>
    /// <param name="ignoreCase">Ignore case of the strings?</param>
    /// <returns>The string delimited by the specified strings</returns>
    public string ExtractBetween(string start, IEnumerable<string> end, bool ignoreCase = false)
    {
      int startIndex = NextIndexOf(start, ignoreCase);
      int soonestEndIndex = int.MaxValue;
      if (startIndex != -1)
      {
        foreach (string s in end)
        {
          int thisIndex = NextIndexOf(startIndex + start.Length, s, ignoreCase);
          if (thisIndex >= startIndex && thisIndex < soonestEndIndex)
          {
            soonestEndIndex = thisIndex;
          }
        }
      }

      return startIndex == -1 || soonestEndIndex == -1 || soonestEndIndex == int.MaxValue ? string.Empty : ExtractBetween(startIndex + start.Length, soonestEndIndex);
    }

    /// <summary>
    /// Extracts a substring from the next occurrence of the start string until the end.
    /// Returns empty if no end is found.
    /// Does not move the reader.
    /// </summary>
    /// <param name="start">Start index of the next instance of the specified string</param>
    /// <param name="ignoreCase">Ignore case of the string?</param>
    /// <returns>The string delimited by the specified string until the end.</returns>
    public string ExtractFrom(string start, bool ignoreCase = false)
    {
      return ExtractFrom(new[] { start }, ignoreCase);
    }

    /// <summary>
    /// Extracts a substring from the first occurrence of any start string until the end.
    /// Returns empty if no end is found.
    /// Does not move the reader.
    /// </summary>
    /// <param name="start">The starting strings</param>
    /// <param name="ignoreCase">Ignore case of the strings?</param>
    /// <returns>The string delimited by the specified string until the end.</returns>
    public string ExtractFrom(IEnumerable<string> start, bool ignoreCase = false)
    {
      int soonestIndex = int.MaxValue;
      foreach (string s in start)
      {
        int thisIndex = NextIndexOf(s, ignoreCase);
        if (thisIndex >= 0 && thisIndex < soonestIndex)
        {
          soonestIndex = thisIndex;
        }
      }

      return soonestIndex == int.MaxValue ? string.Empty : ExtractBetween(soonestIndex, Text.Length);
    }

    /// <summary>
    /// Extracts a substring from the current position to the specified end.
    /// </summary>
    /// <param name="end">The position to extract to </param>
    /// <returns>The string between the current position and the specified end.</returns>
    public string ExtractUntil(int end)
    {
      return ExtractBetween(Position, end);
    }

    /// <summary>
    /// Extracts a substring from the current position to the next occurrence of the end string.
    /// Returns empty if no end is found.
    /// Does not move the reader.
    /// Optionally include the end token in the returned string.
    /// </summary>
    /// <param name="end">End index of the next instance of the specified string</param>
    /// <param name="inclusive">Include the end string in the returned string?</param>
    /// <param name="ignoreCase">Ignore case of the string?</param>
    /// <returns>The string between the current potision and the specified end.</returns>
    public string ExtractUntil(string end, bool inclusive = false, bool ignoreCase = false)
    {
      return ExtractUntil(new[] { end }, inclusive, ignoreCase);
    }

    /// <summary>
    /// Extracts a substring from the current position to the next occurrence of any end string.
    /// Returns empty if no end is found.
    /// Does not move the reader.
    /// Optionally include the end token in the returned string.
    /// </summary>
    /// <param name="end">End index of the next instance of any of the specified strings</param>
    /// <param name="inclusive">Include the end string in the returned string?</param>
    /// <param name="ignoreCase">Ignore case of the strings?</param>
    /// <returns>The string between the current potision and the specified end.</returns>
    public string ExtractUntil(IEnumerable<string> end, bool inclusive = false, bool ignoreCase = false)
    {
      string soonestString = "";
      int soonestIndex = int.MaxValue;
      foreach (string s in end)
      {
        int thisIndex = NextIndexOf(s, ignoreCase);
        if (thisIndex >= 0 && thisIndex < soonestIndex)
        {
          soonestIndex = thisIndex;
          soonestString = s;
        }
      }

      return soonestIndex == int.MaxValue ? string.Empty : ExtractBetween(Position, soonestIndex + (inclusive ? soonestString.Length : 0));
    }

    /// <summary>
    /// Moves the current position ahead the specified number of characters
    /// </summary>
    /// <param name="ahead">The number of characters to move ahead</param>
    public void MoveAhead(int ahead = 1)
    {
      Position = Math.Min(Position + ahead, Text.Length);
    }

    /// <summary>
    /// Moves reader to the specified index (or end).
    /// </summary>
    /// <param name="newPosition">The new position of the reader.</param>
    public void MoveTo(int newPosition)
    {
      Position = Math.Max(0, Math.Min(newPosition, Text.Length));
    }

    /// <summary>
    /// Moves to the next occurrence of the specified string.
    /// Returns if position moved (string was found)
    /// </summary>
    /// <param name="s">The string to find and move to</param>
    /// <param name="ignoreCase">Ignore case of the search string?</param>
    /// <returns>If moved</returns>
    public bool MoveTo(string s, bool ignoreCase = false)
    {
      int index = NextIndexOf(s, ignoreCase);

      if (index < 0)
      {
        return false;
      }

      Position = index;
      return true;
    }

    /// <summary>
    /// Moves to the next occurrence of the specified string and then past it.
    /// Does not move if the string was not found.
    /// Returns if moved.
    /// </summary>
    /// <param name="s">The string to find and move past</param>
    /// <param name="ignoreCase">Ignore case of the search string?</param>
    /// <returns>If moved</returns>
    public bool MovePast(string s, bool ignoreCase = false)
    {
      if (MoveTo(s, ignoreCase))
      {
        MoveAhead(s.Length);
        return true;
      }
      return false;
    }

    /// <summary>
    /// Moves to the next occurrence of any one of the specified characters.
    /// Does not move if no characters were found.
    /// Returns if moved.
    /// </summary>
    /// <param name="chars">The characters to find and move to</param>
    /// <returns>If moved</returns>
    public bool MoveTo(char[] chars)
    {
      int index = Text.IndexOfAny(chars, Position);
      if (index < 0)
      {
        return false;
      }

      Position = index;
      return true;
    }

    /// <summary>
    /// Moves to the next occurrence of a character that fulfils the predicate.
    /// </summary>
    /// <param name="predicate">Predicate matching characters, e.g. Char.IsNumber</param>
    public void MoveTo(Predicate<char> predicate)
    {
      char c = Peek();
      while (!predicate(c) && !IsEnd)
      {
        MoveAhead();
        c = Peek();
      }
    }

    /// <summary>
    /// Moves to the next occurrence of any character that is not one
    /// of the specified characters
    /// </summary>
    /// <param name="chars">Array of characters to move past</param>
    public void MovePast(char[] chars)
    {
      while (chars.Any(ch => Peek() == ch))
      {
        MoveAhead();
      }
    }

    /// <summary>
    /// Get the next occurrence of the specified string from the current position or -1 if not found.
    /// </summary>
    /// <param name="s">The string to find</param>
    /// <param name="ignoreCase">Ignore case of the string?</param>
    /// <returns>The index, or -1.</returns>
    public int NextIndexOf(string s, bool ignoreCase = false)
    {
      return NextIndexOf(Position, s, ignoreCase);
    }

    /// <summary>
    /// Get the next occurrence of the specified string from the current position or -1 if not found.
    /// </summary>
    /// <param name="s">The string to find</param>
    /// <param name="ignoreCase">Ignore case of the string?</param>
    /// <returns>The index, or -1.</returns>
    public int NextIndexOf(int startPosition, string s, bool ignoreCase = false)
    {
      return Text.IndexOf(s, startPosition, ignoreCase ?
          StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
    }
  }
}