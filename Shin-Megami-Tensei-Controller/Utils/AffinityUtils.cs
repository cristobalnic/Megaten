using Shin_Megami_Tensei.DataStructures;
using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.Enums;
using Shin_Megami_Tensei.GameActions.AttackActions;

namespace Shin_Megami_Tensei.Utils;

public static class AffinityUtils
{
    public static double GetDamageByAffinityRules(double baseDamage, AffinityType affinityType)
    {
        
        if (affinityType == AffinityType.Weak)
            baseDamage *= Params.WeakDamageMultiplier;
        else if (affinityType == AffinityType.Resist)
            baseDamage *= Params.ResistDamageMultiplier;
        else if (affinityType == AffinityType.Null)
            baseDamage *= Params.NullDamageMultiplier;
        return baseDamage;
    }

    public static AffinityType GetTargetAffinity(Skill skill, Unit target)
    {
        return target.Affinity.GetAffinity(skill.Type);
    }

    public static bool HasInstantKillSkillMissed(CombatRecord combatRecord, Skill skill)
    {
        if (combatRecord.Affinity is AffinityType.Neutral)
            return !(combatRecord.Attacker.Stats.Lck + skill.Power >= combatRecord.Target.Stats.Lck);
        if (combatRecord.Affinity is AffinityType.Resist)
            return !(combatRecord.Attacker.Stats.Lck + skill.Power >= 2 * combatRecord.Target.Stats.Lck);
        return false;
    }

    public static void ExecuteInstantKillByAffinityRules(CombatRecord combatRecord)
    {
        if (combatRecord.Affinity is AffinityType.Neutral or AffinityType.Resist or AffinityType.Weak)
            AttackUtils.ExecuteInstantKill(combatRecord.Target);
    }
}



