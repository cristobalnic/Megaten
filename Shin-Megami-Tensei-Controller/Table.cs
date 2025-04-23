using Shin_Megami_Tensei.Entities;

namespace Shin_Megami_Tensei;

public class Table
{
    public Samurai? Samurai;
    public readonly List<Unit> Monsters = [];
    public List<Unit> Reserve = [];
    private readonly Player _player;

    public Table(Player player)
    {
        _player = player;
    }


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
            Reserve.Add(monster);
        }
    }

    public void HandleDeath(Unit deadMonster)
    {
        for (int i = 0; i < Monsters.Count; i++)
        {
            if (Monsters[i] != deadMonster || deadMonster is Samurai) continue;
            Monsters[i] = Monster.EmptySlot;
            Reserve.Add(deadMonster);
            ReorderReserve();
            break;
        }
    }

    public void FillEmptySlots()
    {
        int emptySlots = Params.MaxUnitsAllowedInTablePerSide - Monsters.Count;

        for (int i = 0; i < emptySlots; i++)
        {
            Monsters.Add(Monster.EmptySlot);
        }
    }

    public void ReplaceMonster(Unit activeMonster, Unit reserveMonster)
    {
        int activeIndex = Monsters.IndexOf(activeMonster);
        int reserveIndex = Reserve.IndexOf(reserveMonster);
        Monsters[activeIndex] = reserveMonster;
        Reserve[reserveIndex] = activeMonster;
        ReorderReserve();
    }

    private void ReorderReserve()
    {
        List<Unit> reorderedReserve = [];
        foreach (Unit monster in _player.Units)
        {
            if (!Reserve.Contains(monster)) continue;
            reorderedReserve.Add(monster);
        }
        Reserve = reorderedReserve;
    }
}