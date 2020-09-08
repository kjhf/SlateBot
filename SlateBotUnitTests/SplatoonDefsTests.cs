using Microsoft.VisualStudio.TestTools.UnitTesting;
using SlateBot.Splatoon;

namespace SlateBotUnitTests
{
  [TestClass]
  public class SplatoonDefsTests
  {
    [TestMethod]
    public void TestExpectedWeaponQueries()
    {
      Assert.AreEqual("Splattershot", SplatoonDefs.TryFindWeapon("Splattershot", true));
      Assert.AreEqual("Splattershot", SplatoonDefs.TryFindWeapon("splattershot", false));
      Assert.AreEqual(null, SplatoonDefs.TryFindWeapon("ss", true));
      Assert.AreEqual(null, SplatoonDefs.TryFindWeapon("ss", false));
      Assert.AreEqual(null, SplatoonDefs.TryFindWeapon("", false));
      Assert.AreEqual(".52 Gal", SplatoonDefs.TryFindWeapon("52 gal", false));
      Assert.AreEqual("Splattershot Pro", SplatoonDefs.TryFindWeapon("pro", false));
      Assert.AreEqual("Kensa Splattershot Pro", SplatoonDefs.TryFindWeapon("kpro", false));
      Assert.AreEqual("Kensa Rapid Blaster", SplatoonDefs.TryFindWeapon("krapid", false));
      Assert.AreEqual("L-3 Nozzlenose", SplatoonDefs.TryFindWeapon("l3", false));
      Assert.AreEqual("H-3 Nozzlenose", SplatoonDefs.TryFindWeapon("h-3", false));
      Assert.AreEqual("Luna Blaster Neo", SplatoonDefs.TryFindWeapon("luna neo", false));
      Assert.AreEqual("Splat Roller", SplatoonDefs.TryFindWeapon("roller", false));
      Assert.AreEqual("Carbon Roller Deco", SplatoonDefs.TryFindWeapon("carbon d", false));
      Assert.AreEqual("Dynamo Roller", SplatoonDefs.TryFindWeapon("dynamo", false));
      Assert.AreEqual("Grim Range Blaster", SplatoonDefs.TryFindWeapon("grim range", false));
      Assert.AreEqual("Blaster", SplatoonDefs.TryFindWeapon("blaster", false));
      Assert.AreEqual("Nautilus 47", SplatoonDefs.TryFindWeapon("naut47", false));
      Assert.AreEqual("Bloblobber Deco", SplatoonDefs.TryFindWeapon("blob d", false));
      Assert.AreEqual("Explosher", SplatoonDefs.TryFindWeapon("explo", false));
      Assert.AreEqual("Mini Splatling", SplatoonDefs.TryFindWeapon("mini", false));
      Assert.AreEqual("Splat Charger", SplatoonDefs.TryFindWeapon("charger", false));
      Assert.AreEqual("Custom Jet Squelcher", SplatoonDefs.TryFindWeapon("CJet", false));
      Assert.AreEqual("Custom Hydra Splatling", SplatoonDefs.TryFindWeapon("chydra", false));
      Assert.AreEqual("Heavy Splatling", SplatoonDefs.TryFindWeapon("heavy", false));
      Assert.AreEqual("Zink Mini Splatling", SplatoonDefs.TryFindWeapon("zink mini", false));
      Assert.AreEqual("Heavy Splatling Remix", SplatoonDefs.TryFindWeapon("remix", false));
      Assert.AreEqual("Hero Splatling Replica", SplatoonDefs.TryFindWeapon("hero splatling", false));
      Assert.AreEqual("Hero Shot Replica", SplatoonDefs.TryFindWeapon("hero shot", false));    
      Assert.AreEqual("Hero Roller Replica", SplatoonDefs.TryFindWeapon("hero roller", false));
      
    }
  }
}