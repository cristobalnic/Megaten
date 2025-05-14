using Shin_Megami_Tensei.DataStructures;
using Shin_Megami_Tensei.GameData;
using Shin_Megami_Tensei.Utils;

namespace Shin_Megami_Tensei.Entities;

public class EmptySlot : Unit
{
    private EmptySlot(UnitData unitData) : base(unitData)
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
            Name = Params.EmptyUnitName,
        };
        return new EmptySlot(unitData);
    }
}