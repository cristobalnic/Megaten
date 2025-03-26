using Shin_Megami_Tensei.DataStructures;

namespace Shin_Megami_Tensei.Entities;

public abstract class Unit
{
    public readonly string Name;
    private Stats _stats;
    private Affinity _affinity;
    public List<string>? Skills;
    

    protected Unit(UnitData data)
    {
        Name = data.Name;
        _stats = data.Stats;
        _affinity= data.Affinity;
        Skills = data.Skills;
    }
}