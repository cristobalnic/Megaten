using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.Enums;

namespace Shin_Megami_Tensei.GameLoop;

public static class AffinityHandler
{
    public static double GetDamageByAffinityRules(double baseDamage, AffinityType affinityType)
    {
        
        if (affinityType == AffinityType.Weak)
            baseDamage *= Params.WeakDamageMultiplier;
        else if (affinityType == AffinityType.Resist)
            baseDamage *= Params.ResistDamageMultiplier;
        else if (affinityType == AffinityType.Null)
            baseDamage = 0;
        return baseDamage;
    }

    public static void DealDamageByAffinityRules(Unit attacker, int damage, Unit target, AffinityType affinityType)
    {
        if (affinityType == AffinityType.Repel)
            ActionUtils.ApplyDamage(attacker, damage);
        
        else if (affinityType == AffinityType.Drain)
            ActionUtils.ApplyDrain(target, damage);
        else
            ActionUtils.ApplyDamage(target, damage);
    }
}



