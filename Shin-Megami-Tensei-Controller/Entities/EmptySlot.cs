using Shin_Megami_Tensei.DataStructures;
using Shin_Megami_Tensei.GameLoop;

namespace Shin_Megami_Tensei.Entities;

public class EmptySlot : Unit
{
    public EmptySlot(UnitData unitData) : base(unitData)
    {
    }

    public override void Summon(Unit monsterSummon, Table summonerTable, SelectionUtils selectionUtils)
    {
        throw new NotImplementedException();
    }

    public static EmptySlot Build()
    {
        var unitData = new UnitData
        {
            Name = "Vacío",
        };
        return new EmptySlot(unitData);
    }
}