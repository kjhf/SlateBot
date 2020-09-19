using System;
using System.Collections.Generic;
using System.Linq;

namespace SlateBot.Splatoon
{
  public static class SplatoonDefs
  {
    private static readonly HashSet<string> weapons = new HashSet<string>
    {
        "Sploosh-o-matic",
        "Neo Sploosh-o-matic",
        "Sploosh-o-matic 7",
        "Splattershot Jr.",
        "Custom Splattershot Jr.",
        "Kensa Splattershot Jr.",
        "Splash-o-matic",
        "Neo Splash-o-matic",
        "Aerospray MG",
        "Aerospray RG",
        "Aerospray PG",
        "Splattershot",
        "Tentatek Splattershot",
        "Kensa Splattershot",
        "Hero Shot Replica",
        "Octo Shot Replica",
        ".52 Gal",
        ".52 Gal Deco",
        "Kensa .52 Gal",
        "N-ZAP '85",
        "N-ZAP '89",
        "N-ZAP '83",
        "Splattershot Pro",
        "Forge Splattershot Pro",
        "Kensa Splattershot Pro",
        ".96 Gal",
        ".96 Gal Deco",
        "Jet Squelcher",
        "Custom Jet Squelcher",
        "Luna Blaster",
        "Luna Blaster Neo",
        "Kensa Luna Blaster",
        "Blaster",
        "Custom Blaster",
        "Hero Blaster Replica",
        "Range Blaster",
        "Custom Range Blaster",
        "Grim Range Blaster",
        "Clash Blaster",
        "Clash Blaster Neo",
        "Rapid Blaster",
        "Rapid Blaster Deco",
        "Kensa Rapid Blaster",
        "Rapid Blaster Pro",
        "Rapid Blaster Pro Deco",
        "L-3 Nozzlenose",
        "L-3 Nozzlenose D",
        "Kensa L-3 Nozzlenose",
        "H-3 Nozzlenose",
        "H-3 Nozzlenose D",
        "Cherry H-3 Nozzlenose",
        "Squeezer",
        "Foil Squeezer",
        "Carbon Roller",
        "Carbon Roller Deco",
        "Splat Roller",
        "Krak-On Splat Roller",
        "Hero Roller Replica",
        "Kensa Splat Roller",
        "Dynamo Roller",
        "Gold Dynamo Roller",
        "Kensa Dynamo Roller",
        "Flingza Roller",
        "Foil Flingza Roller",
        "Inkbrush",
        "Inkbrush Nouveau",
        "Permanent Inkbrush",
        "Octobrush",
        "Octobrush Nouveau",
        "Herobrush Replica",
        "Kensa Octobrush",
        "Classic Squiffer",
        "New Squiffer",
        "Fresh Squiffer",
        "Splat Charger",
        "Firefin Splat Charger",
        "Hero Charger Replica",
        "Kensa Charger",
        "Splatterscope",
        "Firefin Splatterscope",
        "Kensa Splatterscope",
        "E-liter 4K",
        "Custom E-liter 4K",
        "E-liter 4K Scope",
        "Custom E-liter 4K Scope",
        "Bamboozler 14 Mk I",
        "Bamboozler 14 Mk II",
        "Bamboozler 14 Mk III",
        "Goo Tuber",
        "Custom Goo Tuber",
        "Slosher",
        "Slosher Deco",
        "Soda Slosher",
        "Hero Slosher Replica",
        "Tri-Slosher",
        "Tri-Slosher Nouveau",
        "Sloshing Machine",
        "Sloshing Machine Neo",
        "Kensa Sloshing Machine",
        "Bloblobber",
        "Bloblobber Deco",
        "Explosher",
        "Custom Explosher",
        "Mini Splatling",
        "Zink Mini Splatling",
        "Kensa Mini Splatling",
        "Heavy Splatling",
        "Heavy Splatling Deco",
        "Heavy Splatling Remix",
        "Hero Splatling Replica",
        "Hydra Splatling",
        "Custom Hydra Splatling",
        "Ballpoint Splatling",
        "Ballpoint Splatling Nouveau",
        "Nautilus 47",
        "Nautilus 79",
        "Dapple Dualies",
        "Dapple Dualies Nouveau",
        "Clear Dapple Dualies",
        "Splat Dualies",
        "Enperry Splat Dualies",
        "Hero Dualie Replicas",
        "Kensa Splat Dualies",
        "Glooga Dualies",
        "Glooga Dualies Deco",
        "Kensa Glooga Dualies",
        "Dualie Squelchers",
        "Custom Dualie Squelchers",
        "Dark Tetra Dualies",
        "Light Tetra Dualies",
        "Splat Brella",
        "Sorella Brella",
        "Hero Brella Replica",
        "Tenta Brella",
        "Tenta Sorella Brella",
        "Tenta Camo Brella",
        "Undercover Brella",
        "Undercover Sorella Brella",
        "Kensa Undercover Brella"
    };

    /// <summary>
    /// Weapons but have undergone <see cref="TransformWeapon"/>.
    /// </summary>
    private static readonly string[] weaponsTransformed = weapons.Select(TransformWeapon).ToArray();

    /// <summary>
    /// Try and get a weapon from this query.
    /// </summary>
    /// <param name="query"></param>
    /// <param name="exact"></param>
    /// <returns></returns>
    public static string TryFindWeapon(string query, bool exact = false)
    {
      if (exact)
      {
        weapons.TryGetValue(query, out string result);
        return result;
      }
      else
      {
        if (weapons.TryGetValue(query, out string result))
        {
          return result;
        }

        // Special nick names
        if (query.Equals("clappies", StringComparison.OrdinalIgnoreCase))
        {
          return "Clear Dapple Dualies";
        }
        else if (query.Equals("zimi", StringComparison.OrdinalIgnoreCase))
        {
          return "Zink Mini Splatling";
        }
        else if (query.Equals("kimi", StringComparison.OrdinalIgnoreCase))
        {
          return "Kensa Mini Splatling";
        }
        else if (query.Equals("kunder", StringComparison.OrdinalIgnoreCase))
        {
          return "Kensa Undercover Brella";
        }

        for (int i = 0; i < weaponsTransformed.Length; ++i)
        {
          if (weaponsTransformed[i].Equals(TransformWeapon(query)))
          {
            result = weapons.ElementAt(i);
            break;
          }
        }
        return result;
      }
    }

    /// <summary>
    /// Transform the weapon into a searchable format.
    /// </summary>
    /// <param name="wep"></param>
    /// <returns></returns>
    private static string TransformWeapon(string wep)
    {
      wep = wep
        .ToLowerInvariant()
        .TrimEnd('s', ' ') // In case of "dualies" or tetras"
        .Replace("splat ", "")
        .Replace("classic ", "")

        .Replace("kensa ", "k")
        .Replace("custom ", "c")
        .Replace("deco", "d")
        .Replace("nouveau", "n")
        .Replace("zink ", "z")
        .Replace("replica", "")
        .Replace("-", "")
        .Replace(".", "")
        .Replace("'", "")
        .Replace(" ", "")

        // Special changes
        .Replace("duel", "dual")
        .Replace("nautilus", "naut")
        .Replace("ballpoint", "bp")
        .Replace("omatic", "")
        .Replace("squelcher", "")
        .Replace("squiffer", "squiff")
        .Replace("nozzlenose", "") // The Nozzles are usually known by their names
        .Replace("splatling", "") // The Splatlings are usually known by their names
        .Replace("14mk", "") // These are common to bamboos so boring
        .Replace("tuber", "") // These are common to goo tuber so boring
        .Replace("splattershotpro", "pro") // Known as pros
        .Replace("splattershot", "shot") // Known as shots
        .Replace("splatterscope", "scope") // Known as scopes
        .Replace("sloshingmachine", "machine") // Known as machines
        .Replace("bloblobber", "blob") // Known as blobs
        .Replace("explosher", "explo") // Usually shortened
        .Replace("explosh", "explo") // Usually shortened
        .Replace("tenta", "tent") // Usually shortened
        .Replace("sodaslosher", "soda")
        .Replace("rapidblaster", "rapid")
        .Replace("lunablaster", "luna")
        .Replace("rangeblaster", "range")
        .Replace("dynamoroller", "dynamo")
        .Replace("carbonroller", "carbon")
        .Replace("flingzaroller", "fling")
        .Replace("flingroller", "fling")
        .Replace("flingza", "fling")
        .Replace("heavyremix", "remix")
        ;

      return wep;
    }
  }
}