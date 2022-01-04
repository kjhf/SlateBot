using System;
using System.Collections.Generic;
using System.Linq;

namespace SlateBot.Splatoon
{
  public static class SplatoonDefs
  {
    private static readonly Dictionary<string, Set<string>> weapons = new()
    {
      { ".52 Gal": new Set { "gal", "52", "v52", "52g" } },  // default gal
      { ".52 Gal Deco": new Set { "galdeco", "52deco", "52galdeco", "52gd" } },
      { ".96 Gal": new Set { "96", "v96", "96g" } },
      { ".96 Gal Deco": new Set { "96deco", "96galdeco", "96gd" } },
      { "Aerospray MG": new Set { "mg", "aeromg", "silveraero", "silveraerospray", "aero", "aerospray" } },  // default aero
      { "Aerospray PG": new Set { "pg", "aeropg", "bronzeaero", "bronzeaerospray" } },
      { "Aerospray RG": new Set { "rg", "aerorg", "goldaero", "goldaerospray" } },
      { "Ballpoint Splatling": new Set { "ballpoint", "bp", "pen" } },  // default ballpoint
      { "Ballpoint Splatling Nouveau": new Set { "ballpointnouveau", "bpn", "bpsn", "bsn" } },
      { "Bamboozler 14 Mk I": new Set { "bambooi", "bamboo1", "bamboo14mki", "bamboomki", "bamboomk1" } },
      { "Bamboozler 14 Mk II": new Set { "bambooii", "bamboo2", "bamboo14mkii", "bamboomkii", "bamboomk2" } },
      { "Bamboozler 14 Mk III": new Set { "bambooiii", "bamboo3", "bamboo14mkiii", "bamboomkiii", "bamboomk3" } },
      { "Blaster": new Set { "vblaster" } },
      { "Bloblobber": new Set { "blob", "vblob" } },
      { "Bloblobber Deco": new Set { "blobdeco" } },
      { "Carbon Roller": new Set { "carbon", "vcarbon" } },
      { "Carbon Roller Deco": new Set { "carbondeco", "crd" } },
      { "Cherry H-3 Nozzlenose": new Set { "cherry", "ch3", "ch3n", "cherrynozzle" } },
      { "Clash Blaster": new Set { "clash", "vclash", "clashter" } },
      { "Clash Blaster Neo": new Set { "clashneo", "clashterneo", "cbn" } },
      { "Classic Squiffer": new Set { "csquif", "csquiff", "bluesquif", "bluesquiff", "squif", "squiff", "squiffer" } },  // default squiffer
      { "Clear Dapple Dualies": new Set { "cdapple", "cdapples", "cleardualies", "clapples", "clappies", "cdd" } },
      { "Custom Blaster": new Set { "cblaster" } },
      { "Custom Dualie Squelchers": new Set { "cds", "customdualies", "cdualies" } },
      { "Custom E-Liter 4K": new Set { "c4k", "ce4k", "celiter", "celitre", "celiter4k", "celitre4k", "custom4k" } },
      { "Custom E-Liter 4K Scope": new Set { "c4ks", "ce4ks", "celiterscope", "celitrescope", "celiter4kscope", "celitre4kscope", "custom4kscope" } },
      { "Custom Explosher": new Set { "cex", "cexplo", "cexplosher" } },
      { "Custom Goo Tuber": new Set { "customgoo", "cgoo", "cgootube", "cgootuber", "cgt" } },
      { "Custom Hydra Splatling": new Set { "customhyra", "chydra", "chydrasplatling", "chs" } },
      { "Custom Jet Squelcher": new Set { "customjet", "cjet", "cjets", "cjs", "cjsquelcher", "cjetsquelcher" } },
      { "Custom Range Blaster": new Set { "customrange", "crange", "crblaster", "crb" } },
      { "Custom Splattershot Jr.": new Set { "customjunior", "cjr", "cjnr", "cjunior", "csj" } },
      { "Dapple Dualies": new Set { "dapples", "vdapples", "vdd", "dd", "ddualies" } },
      { "Dapple Dualies Nouveau": new Set { "dapplesnouveau", "ddn", "ddualiesn" } },
      { "Dark Tetra Dualies": new Set { "tetra", "tetras", "tetradualies", "dark", "darks", "darktetra", "darktetras", "darkdualies", "dtd" } },  // default tetras
      { "Dualie Squelchers": new Set { "ds", "vds" } },
      { "Dynamo Roller": new Set { "dyna", "dynamo", "vdynamo", "silverdynamo" } },
      { "E-liter 4K": new Set { "4k", "e4k", "eliter", "elitre", "eliter4k", "elitre4k" } },
      { "E-liter 4K Scope": new Set { "4ks", "e4ks", "eliterscope", "elitrescope", "eliter4kscope", "elitre4kscope" } },
      { "Enperry Splat Dualies": new Set { "edualies", "enperries", "enperrydualies", "esd" } },
      { "Explosher": new Set { "vex", "explo", "vexplo" } },
      { "Firefin Splat Charger": new Set { "firefin", "firefincharger", "fsc", "ffin" } },
      { "Firefin Splatterscope": new Set { "firefinscope", "ffinscope" } },
      { "Flingza Roller": new Set { "fling", "flingza", "vfling", "vflingza" } },
      { "Foil Flingza Roller": new Set { "foilfling", "foilflingza", "ffling", "fflingza", "ffr" } },
      { "Foil Squeezer": new Set { "fsqueezer" } },
      { "Forge Splattershot Pro": new Set { "forge", "forgepro", "fpro", "fsp" } },
      { "Fresh Squiffer": new Set { "fsquif", "fsquiff", "redsquif", "redsquiff" } },
      { "Glooga Dualies": new Set { "glooga", "gloogas", "glues", "vglues", "vgloogas", "gd", "vgd" } },
      { "Glooga Dualies Deco": new Set { "gloogadeco", "gloogasdeco", "gluesdeco", "dglues", "dgloogas", "gdd", "dgd" } },
      { "Gold Dynamo Roller": new Set { "golddyna", "golddynamo", "gdr" } },
      { "Goo Tuber": new Set { "goo", "vgoo", "gootube", "vgootube", "vgootuber" } },
      { "Grim Range Blaster": new Set { "grim", "grange", "grblaster", "grb" } },
      { "H-3 Nozzlenose": new Set { "h3", "vh3", "h3nozzle", "h3n" } },
      { "H-3 Nozzlenose D": new Set { "h3d", "h3dnozzle", "h3nd", "h3dn" } },
      { "Heavy Splatling": new Set { "heavy", "vheavy" } },
      { "Heavy Splatling Deco": new Set { "heavyd", "heavydeco", "hsd" } },
      { "Heavy Splatling Remix": new Set { "remix", "heavyremix", "hsr" } },
      { "Hero Blaster Replica": new Set { "heroblaster" } },
      { "Hero Brella Replica": new Set { "herobrella" } },
      { "Hero Charger Replica": new Set { "herocharger" } },
      { "Hero Dualie Replicas": new Set { "herodualie", "herodualies", "hdualie", "hdualies" } },
      { "Hero Roller Replica": new Set { "heroroller" } },
      { "Hero Shot Replica": new Set { "heroshot" } },
      { "Hero Slosher Replica": new Set { "heroslosh", "heroslosher" } },
      { "Hero Splatling Replica": new Set { "herosplatling", "heroheavy" } },
      { "Herobrush Replica": new Set { "herobrush" } },
      { "Hydra Splatling": new Set { "hydra", "vhydra", "vhydrasplatling" } },
      { "Inkbrush": new Set { "brush", "vbrush", "vinkbrush" } },  // default brush
      { "Inkbrush Nouveau": new Set { "brushn", "brushnouveau", "nbrush", "inkbrushn" } },
      { "Jet Squelcher": new Set { "jet", "vjet", "jets", "vjets", "js", "vjs", "jsquelcher", "vjsquelcher", "vjetsquelcher" } },
      { "Kensa .52 Gal": new Set { "kgal", "k52", "k52gal" } },  // default kgal
      { "Kensa Charger": new Set { "kcharger" } },
      { "Kensa Dynamo Roller": new Set { "kdyna", "kdynamo", "kensadynamo", "kdr" } },
      { "Kensa Glooga Dualies": new Set { "kensaglooga", "kensagloogas", "kensaglues", "klues", "kglues", "klooga", "kloogas", "kgloogas", "kgd" } },
      { "Kensa L-3 Nozzlenose": new Set { "knozzle", "kl3", "kl3n", "kl3nozzle" } },
      { "Kensa Luna Blaster": new Set { "kensaluna", "kluna", "kuna", "kunablaster", "klb" } },
      { "Kensa Mini Splatling": new Set { "kensamini", "kmini", "kimi", "kimisplatling", "kminisplatling", "kms" } },
      { "Kensa Octobrush": new Set { "kensabrush", "kbrush", "krush", "kocto", "koctobrush", "kob" } },
      { "Kensa Rapid Blaster": new Set { "kensarapid", "krapid", "krapidblaster", "kraster", "krb" } },
      { "Kensa Sloshing Machine": new Set { "kensasloshmachine", "ksloshmachine", "kensamachine", "kmachine", "kachine", "kachin", "ksm" } },
      { "Kensa Splat Dualies": new Set { "kensadualie", "kensadualies", "kdaulies", "kdaulie", "kdualie", "kdualies", "kaulies", "kualies", "kaulie", "kualie", "ksd" } },
      { "Kensa Splat Roller": new Set { "kensaroller", "kroller", "kroll", "ksr" } },
      { "Kensa Splatterscope": new Set { "kensascope", "ksscope", "kscope", "kss" } },
      { "Kensa Splattershot": new Set { "kensashot", "ksshot", "kshot" } },
      { "Kensa Splattershot Jr.": new Set { "kensajunior", "kjr", "kjnr", "kjunior", "ksj" } },
      { "Kensa Splattershot Pro": new Set { "kensapro", "kpro", "ksp" } },
      { "Kensa Undercover Brella": new Set { "kensaundercover", "kunder", "kensabrella", "kub" } },
      { "Krak-On Splat Roller": new Set { "krakon", "krakonroller", "krack", "krackonroller", "krak", "krakenroller", "koroller", "koro", "kosr" } },
      { "L-3 Nozzlenose": new Set { "l3", "vl3", "l3nozzle", "l3n" } },
      { "L-3 Nozzlenose D": new Set { "l3d", "l3dnozzle", "l3nd", "l3dn" } },
      { "Light Tetra Dualies": new Set { "light", "lights", "lightdualies", "lighttetra", "lighttetras" } },
      { "Luna Blaster": new Set { "luna", "vluna", "vuna", "vlunablaster" } },
      { "Luna Blaster Neo": new Set { "lunaneo", "lbn" } },
      { "Mini Splatling": new Set { "mini", "vmini", "vimi", "vimisplatling", "vminisplatling", "vms" } },
      { "N-ZAP '83": new Set { "zap83", "83", "bronzenzap", "bronzezap", "brownnzap", "brownzap", "rednzap", "redzap" } },  // By Twitter poll, this zap is the red one.
      { "N-ZAP '85": new Set { "zap85", "85", "greynzap", "greyzap", "graynzap", "grayzap", "zap", "nzap" } },  // default zap
      { "N-ZAP '89": new Set { "zap89", "89", "orangenzap", "orangezap" } },
      { "Nautilus 47": new Set { "naut47", "47", "naut" } },  // default nautilus
      { "Nautilus 79": new Set { "naut79", "79" } },
      { "Neo Splash-o-matic": new Set { "neosplash", "nsplash", "nsplashomatic" } },
      { "Neo Sploosh-o-matic": new Set { "neosploosh", "nsploosh", "nsplooshomatic" } },
      { "New Squiffer": new Set { "nsquif", "nsquiff", "newsquif", "newsquiff" } },
      { "Octobrush": new Set { "octo", "obrush", "vocto", "voctobrush", "vobrush" } },
      { "Octobrush Nouveau": new Set { "octon", "obrushn", "octobrushn" } },
      { "Octo Shot Replica": new Set { "oshot", "osr" } },
      { "Permanent Inkbrush": new Set { "pbrush", "permabrush", "permanentbrush", "pinkbrush", "permainkbrush" } },
      { "Range Blaster": new Set { "range", "vrange", "vrangeblaster" } },
      { "Rapid Blaster": new Set { "rapid", "vrapid", "vrapidblaster" } },
      { "Rapid Blaster Deco": new Set { "rapiddeco", "rapidd", "rapidblasterd", "rbd" } },
      { "Rapid Blaster Pro": new Set { "rapidpro", "prorapid", "rbp" } },
      { "Rapid Blaster Pro Deco": new Set { "rapidprodeco", "prodecorapid", "rbpd" } },
      { "Slosher": new Set { "slosh", "vslosh" } },
      { "Slosher Deco": new Set { "sloshd", "sloshdeco" } },
      { "Sloshing Machine": new Set { "sloshmachine", "vsloshmachine", "vmachine", "machine", "vachine", "vsm" } },
      { "Sloshing Machine Neo": new Set { "sloshmachineneo", "neosloshmachine", "neomachine", "machineneo", "smn" } },
      { "Soda Slosher": new Set { "soda", "sodaslosh" } },
      { "Sorella Brella": new Set { "sorella", "sbrella", "srella" } },
      { "Splash-o-matic": new Set { "splash", "vsplash", "vsplashomatic" } },
      { "Splat Brella": new Set { "brella", "vbrella", "vsplatbrella" } },
      { "Splat Charger": new Set { "charger", "vcharger", "vsplatcharger" } },
      { "Splat Dualies": new Set { "dualies", "vdualies", "vsplatdualies" } },
      { "Splat Roller": new Set { "roller", "vroller", "vsplatroller" } },
      { "Splatterscope": new Set { "scope", "vscope", "vsplatscope", "vsplatterscope" } },
      { "Splattershot": new Set { "shot", "vshot", "vsplatshot", "vsplattershot" } },
      { "Splattershot Jr.": new Set { "junior", "jr", "vjr", "jnr", "vjnr", "vjunior", "vsj" } },
      { "Splattershot Pro": new Set { "pro", "vpro", "vsplatshotpro", "vsplatterpro" } },
      { "Sploosh-o-matic": new Set { "sploosh", "vsploosh", "vsplooshomatic" } },
      { "Sploosh-o-matic 7": new Set { "7", "sploosh7", "7sploosh", "7splooshomatic" } },
      { "Squeezer": new Set { "vsqueezer" } },
      { "Tenta Brella": new Set { "tent", "vent", "vtent", "tentbrella", "vtentbrella" } },
      { "Tenta Camo Brella": new Set { "tentcamo", "camo", "camotent", "camobrella", "tentcamobrella", "tcb" } },
      { "Tenta Sorella Brella": new Set { "tentsorella", "tsorella", "sorellatent", "tsorellabrella", "tentsorellabrella", "tsb" } },
      { "Tentatek Splattershot": new Set { "ttek", "ttekshot", "tshot", "ttshot", "ttsplatshot", "ttsplattershot", "ttss", "ttk" } },
      { "Tri-Slosher": new Set { "tri", "trislosh", "vtri", "vtrislosh", "vtrislosher" } },
      { "Tri-Slosher Nouveau": new Set { "trin", "trisloshn", "trinouveau", "trisloshnouveau", "tsn" } },
      { "Undercover Brella": new Set { "undercover", "ubrella", "vundercover", "vundercoverbrella" } },
      { "Undercover Sorella Brella": new Set { "sunder", "sundercover", "undercoversorella", "sundercoverbrella", "usb" } },
      { "Zink Mini Splatling": new Set { "zinkmini", "zmini", "zimi", "zimisplatling", "zminisplatling", "zms" } },
    };

    /// <summary>
    /// Weapons but have undergone <see cref="TransformWeapon"/>.
    /// </summary>
    private static readonly string[] weaponsTransformed = weapons.Keys().Select(TransformWeapon).ToArray();

    /// <summary>
    /// Try and get a weapon from this query.
    /// </summary>
    /// <param name="query"></param>
    /// <param name="exact"></param>
    /// <returns></returns>
    public static string TryFindWeapon(string query, bool exact = false)
    {
      // First, match exact against the known weapons and then their transformed varients.
      var result = weapons.FirstOrDefault(pair => pair.Key.Equals(query));
      if (result == null)
      {
        result = weaponsTransformed.FirstOrDefault(pair => pair.Key.Equals(query));
      }

      if (result || exact)
      {
        return result;
      }

      // Search inexact
      query = TransformWeapon(query);
      result = weaponsTransformed.FirstOrDefault(w => w.Equals(query));
      return result;
    }

    /// <summary>
    /// Transform the weapon into a searchable format.
    /// </summary>
    /// <param name="wep"></param>
    /// <returns></returns>
    private static string TransformWeapon(string wep)
    {
      // Lowercase and remove spaces and punctuation
      wep = wep
        .ToLowercase()
        .Replace("[", "")
        .Replace(" ", "")
        .Replace(".", "")
        .Replace("\\", "")
        .Replace("-", "")
        .Replace("'", "")
        .Replace("]", "")
        
      // Typo corrections
        .Replace("duel", "dual")
        ;

      return wep
    }
  }
}