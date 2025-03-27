using Shin_Megami_Tensei.DataStructures;

namespace Shin_Megami_Tensei.Entities;

public abstract class Unit
{
    public readonly string Name;
    public readonly Stats Stats;
    private Affinity _affinity;
    public readonly List<Skill> Skills = [];

    protected Unit(UnitData unitData)
    {
        Name = unitData.Name;
        Stats = unitData.Stats;
        Stats.MaxHp = unitData.Stats.Hp;
        Stats.MaxMp = unitData.Stats.Mp;
        _affinity= unitData.Affinity;
    }
    
    
}