using Shin_Megami_Tensei.Entities;

namespace Shin_Megami_Tensei.GameData;

public class Table
{
    public readonly List<Unit> ActiveUnits = [];
    public List<Unit> Reserve = [];
    private readonly Player _player;

    public Table(Player player)
    {
        _player = player;
    }


    public void AddSamuraiToTable(Samurai samurai)
    {
        AddMonster(samurai);
    }

    public void AddMonster(Unit monster)
    {
        if (ActiveUnits.Count < Params.MaxUnitsAllowedInTablePerSide)
        {
            ActiveUnits.Add(monster);
        }
        else
        {
            Reserve.Add(monster);
        }
    }

    public void HandleDeath(Unit deadMonster)
    {
        for (int i = 0; i < ActiveUnits.Count; i++)
        {
            if (ActiveUnits[i] != deadMonster || deadMonster is Samurai) continue;
            ActiveUnits[i] = EmptySlot.Build();
            Reserve.Add(deadMonster);
            ReorderReserve();
            break;
        }
    }

    public void FillEmptySlots()
    {
        int emptySlots = Params.MaxUnitsAllowedInTablePerSide - ActiveUnits.Count;
        for (int i = 0; i < emptySlots; i++) ActiveUnits.Add(EmptySlot.Build());
    }

    public void ReplaceMonster(Unit activeMonster, Unit reserveMonster)
    {
        int activeIndex = ActiveUnits.IndexOf(activeMonster);
        int reserveIndex = Reserve.IndexOf(reserveMonster);
        ActiveUnits[activeIndex] = reserveMonster;
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