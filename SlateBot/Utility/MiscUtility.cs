using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlateBot.Utility
{
  public static class MiscUtility
  {
    /// <summary>
    /// Dump an object's state to the returned StringBuilder.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="format"></param>
    /// <returns></returns>
    public static StringBuilder DumpObject(object obj, string format = "{0}={1}\r\n")
    {
      StringBuilder sb = new StringBuilder();
      foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(obj))
      {
        string name = descriptor.Name;
        object value = descriptor.GetValue(obj);

        // If the value is enumerable, try and get a sensible string from its members.
        if ((!(value is string)) && value is IEnumerable enumerable)
        {
          StringBuilder enumerableStr = new StringBuilder("{ ");
          var en = enumerable.GetEnumerator();
          if (en.MoveNext())
          {
            if (en.Current != null)
            {
              enumerableStr.Append(en.Current);
            }

            while (en.MoveNext())
            {
              enumerableStr.Append(", ");
              if (en.Current != null)
              {
                enumerableStr.Append(en.Current);
              }
            }
          }
          enumerableStr.Append(" }");
          sb.AppendFormat(format, name, enumerableStr.ToString());
        }
        else
        {
          // Otherwise just string it.
          sb.AppendFormat(format, name, value);
        }
      }
      return sb;
    }
  }
}
