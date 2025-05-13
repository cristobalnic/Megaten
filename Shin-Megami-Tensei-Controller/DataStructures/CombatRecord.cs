using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.Enums;

namespace Shin_Megami_Tensei.DataStructures;

public struct CombatRecord
{
    public readonly Unit Attacker;
    public readonly Unit Target;
    public int Damage;
    public readonly AffinityType Affinity;
    
    public CombatRecord(Unit attacker, Unit target, int damage, AffinityType affinity)
    {
        Attacker = attacker;
        Target = target;
        Damage = damage;
        Affinity = affinity;
    }
}