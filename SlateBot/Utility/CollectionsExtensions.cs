using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlateBot.Utility
{
  public static class CollectionsExtensions
  {
    /// <summary>
    /// Add a value to a dictionary's inner collection, creating the key if it does not exist already.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary</typeparam>
    /// <typeparam name="TCollection">The full type of the values, which is a collection in the dictionary</typeparam>
    /// <typeparam name="TElement">The type of each element in the collection in the value</typeparam>
    /// <param name="dictionary">The dictionary to add to</param>
    /// <param name="key">The key to add into</param>
    /// <param name="value">The value to add into the collection</param>
    public static void AddToInner<TKey, TCollection, TElement>(this IDictionary<TKey, TCollection> dictionary, TKey key, TElement value) where TCollection : ICollection<TElement>, new()
    {
      if (!dictionary.ContainsKey(key))
      {
        dictionary.Add(key, new TCollection());
      }
      dictionary[key].Add(value);
    }

    /// <summary>
    /// Remove a value from a dictionary's inner collection, also removing the key if it is now empty.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary</typeparam>
    /// <typeparam name="TCollection">The full type of the values, which is a collection in the dictionary</typeparam>
    /// <typeparam name="TElement">The type of each element in the collection in the value</typeparam>
    /// <param name="dictionary">The dictionary to remove from</param>
    /// <param name="key">The key to remove</param>
    /// <param name="value">The value to remove from the collection</param>
    public static void RemoveFromInner<TKey, TCollection, TElement>(this IDictionary<TKey, TCollection> dictionary, TKey key, TElement value) where TCollection : ICollection<TElement>, new()
    {
      // Only if the dictionary contains the key.
      if (dictionary.ContainsKey(key))
      {
        dictionary[key].Remove(value);

        // If the dictionary's collection is now empty.
        if (dictionary[key].Count == 0)
        {
          // Remove it.
          dictionary.Remove(key);
        }
      }
    }
  }
}
