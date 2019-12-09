using System.Collections.Generic;

namespace PokemonLibrary
{
  public class Stats
  {
    public readonly Dictionary<Stat, byte> BaseStats = new Dictionary<Stat, byte>();

    public Dictionary<Stat, Range> AtLevel50
    {
      get
      {
        var at50 = new Dictionary<Stat, Range>();
        int attack = BaseStats[Stat.Attack];
        int defense = BaseStats[Stat.Defense];
        int hp = BaseStats[Stat.HP];
        int spAttack = BaseStats[Stat.SpecialAttack];
        int spDefense = BaseStats[Stat.SpecialDefense];
        int speed = BaseStats[Stat.Speed];
        at50.Add(Stat.Attack, new Range((int)((((0 + 2 * attack + (0 / 4)) * 50 / 100) + 5) * .9), (int)((((31 + 2 * attack + (252 / 4)) * 50 / 100) + 5) * 1.1)));
        at50.Add(Stat.Defense, new Range((int)((((0 + 2 * defense + (0 / 4)) * 50 / 100) + 5) * .9), (int)((((31 + 2 * defense + (252 / 4)) * 50 / 100) + 5) * 1.1)));
        at50.Add(Stat.HP, new Range(((0 + 2 * hp + (0 / 4)) * 50 / 100) + 10 + 50, ((31 + 2 * hp + (252 / 4)) * 50 / 100) + 10 + 50));
        at50.Add(Stat.SpecialAttack, new Range((int)((((0 + 2 * spAttack + (0 / 4)) * 50 / 100) + 5) * .9), (int)((((31 + 2 * spAttack + (252 / 4)) * 50 / 100) + 5) * 1.1)));
        at50.Add(Stat.SpecialDefense, new Range((int)((((0 + 2 * spDefense + (0 / 4)) * 50 / 100) + 5) * .9), (int)((((31 + 2 * spDefense + (252 / 4)) * 50 / 100) + 5) * 1.1)));
        at50.Add(Stat.Speed, new Range((int)((((0 + 2 * speed + (0 / 4)) * 50 / 100) + 5) * .9), (int)((((31 + 2 * speed + (252 / 4)) * 50 / 100) + 5) * 1.1)));
        return at50;
      }
    }

    public Dictionary<Stat, Range> AtLevel100
    {
      get
      {
        var at100 = new Dictionary<Stat, Range>();
        int attack = BaseStats[Stat.Attack];
        int defense = BaseStats[Stat.Defense];
        int hp = BaseStats[Stat.HP];
        int spAttack = BaseStats[Stat.SpecialAttack];
        int spDefense = BaseStats[Stat.SpecialDefense];
        int speed = BaseStats[Stat.Speed];
        at100.Add(Stat.Attack, new Range((int)((((0 + 2 * attack + (0 / 4)) * 100 / 100) + 5) * .9), (int)((((31 + 2 * attack + (252 / 4)) * 100 / 100) + 5) * 1.1)));
        at100.Add(Stat.Defense, new Range((int)((((0 + 2 * defense + (0 / 4)) * 100 / 100) + 5) * .9), (int)((((31 + 2 * defense + (252 / 4)) * 100 / 100) + 5) * 1.1)));
        at100.Add(Stat.HP, new Range(((0 + 2 * hp + (0 / 4)) * 100 / 100) + 10 + 100, ((31 + 2 * hp + (252 / 4)) * 100 / 100) + 10 + 100));
        at100.Add(Stat.SpecialAttack, new Range((int)((((0 + 2 * spAttack + (0 / 4)) * 100 / 100) + 5) * .9), (int)((((31 + 2 * spAttack + (252 / 4)) * 100 / 100) + 5) * 1.1)));
        at100.Add(Stat.SpecialDefense, new Range((int)((((0 + 2 * spDefense + (0 / 4)) * 100 / 100) + 5) * .9), (int)((((31 + 2 * spDefense + (252 / 4)) * 100 / 100) + 5) * 1.1)));
        at100.Add(Stat.Speed, new Range((int)((((0 + 2 * speed + (0 / 4)) * 100 / 100) + 5) * .9), (int)((((31 + 2 * speed + (252 / 4)) * 100 / 100) + 5) * 1.1)));
        return at100;
      }
    }
  }
}