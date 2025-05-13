using Shin_Megami_Tensei.DataStructures;
using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.Enums;
using Shin_Megami_Tensei.GameActions.AttackActions;
using Shin_Megami_Tensei.GameData;

namespace Shin_Megami_Tensei.GameLoop;

public static class AffinityUtils
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

    public static void DealDamageByAffinityRules(CombatRecord combatRecord)
    {
        if (combatRecord.Affinity == AffinityType.Repel)
            AttackUtils.ApplyDamage(combatRecord.Attacker, combatRecord.Damage);
        
        else if (combatRecord.Affinity == AffinityType.Drain)
            AttackUtils.ApplyDrain(combatRecord.Target, combatRecord.Damage);
        else
            AttackUtils.ApplyDamage(combatRecord.Target, combatRecord.Damage);
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



