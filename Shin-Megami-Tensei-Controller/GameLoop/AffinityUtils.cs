using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.Enums;
using Shin_Megami_Tensei.GameLoop.Actions.AttackActions;

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

    public static void DealDamageByAffinityRules(Unit attacker, int damage, Unit target, AffinityType affinityType)
    {
        if (affinityType == AffinityType.Repel)
            AttackUtils.ApplyDamage(attacker, damage);
        
        else if (affinityType == AffinityType.Drain)
            AttackUtils.ApplyDrain(target, damage);
        else
            AttackUtils.ApplyDamage(target, damage);
    }

    public static AffinityType GetTargetAffinity(Skill skill, Unit target)
    {
        return target.Affinity.GetAffinity(skill.Type);
    }

    public static bool HasInstantKillSkillMissed(Unit attacker, Skill skill, Unit target, AffinityType targetAffinity)
    {
        if (targetAffinity is AffinityType.Neutral)
            return !(attacker.Stats.Lck + skill.Power >= target.Stats.Lck);
        if (targetAffinity is AffinityType.Resist)
            return !(attacker.Stats.Lck + skill.Power >= 2 * target.Stats.Lck);
        return false;
    }

    public static void ExecuteInstantKillByAffinityRules(Unit attacker, Unit target, AffinityType targetAffinity)
    {
        if (targetAffinity is AffinityType.Neutral or AffinityType.Resist or AffinityType.Weak)
            AttackUtils.ExecuteInstantKill(target);
    }
}



