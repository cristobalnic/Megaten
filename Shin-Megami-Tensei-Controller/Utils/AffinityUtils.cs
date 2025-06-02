using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.Enums;

namespace Shin_Megami_Tensei.Utils;

public static class AffinityUtils
{
    public static AffinityType GetTargetAffinity(Skill skill, Unit target)
    {
        return target.Affinity.GetAffinity(skill.Type);
    }
}