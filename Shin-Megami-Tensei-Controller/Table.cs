using Shin_Megami_Tensei.Entities;

namespace Shin_Megami_Tensei;

public class Table
{
    public Samurai? Samurai;
    public readonly List<Unit?> Monsters = [];
    private readonly List<Unit?> _reserve = [];


    public void SetSamurai(Samurai? samurai)
    {
        Samurai = samurai;
        if (Samurai != null) AddMonster(Samurai);
    }

    public void AddMonster(Unit monster)
    {
        if (Monsters.Count < Params.MaxUnitsAllowedInTablePerSide)
        {
            Monsters.Add(monster);
        }
        else
        {
            _reserve.Add(monster);
        }
    }

    public void HandleDeath(Unit deadMonster)
    {
        for (int i = 0; i < Monsters.Count; i++)
        {
            if (Monsters[i] != deadMonster || deadMonster is Samurai) continue;
            Monsters[i] = null;
            _reserve.Add(deadMonster);
            break;
        }
    }

    public void FillEmptySlotsToNull()
    {
        int emptySlots = Params.MaxUnitsAllowedInTablePerSide - Monsters.Count;

        for (int i = 0; i < emptySlots; i++)
        {
            Monsters.Add(null);
        }
    }
}