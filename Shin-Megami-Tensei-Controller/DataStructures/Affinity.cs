﻿using Shin_Megami_Tensei.Enums;

namespace Shin_Megami_Tensei.DataStructures;

public struct Affinity
{
    public required AffinityType Phys { get; set; }
    public required AffinityType Gun { get; set; }
    public required AffinityType Fire { get; set; }
    public required AffinityType Ice { get; set; }
    public required AffinityType Elec { get; set; }
    public required AffinityType Force { get; set; }
    public required AffinityType Light { get; set; }
    public required AffinityType Dark { get; set; }
    public required AffinityType Bind { get; set; }
    public required AffinityType Sleep { get; set; }
    public required AffinityType Sick { get; set; }
    public required AffinityType Panic { get; set; }
    public required AffinityType Poison { get; set; }

    public AffinityType GetAffinity(SkillType skillType)
    {
        return skillType switch
        {
            SkillType.Phys => Phys,
            SkillType.Gun => Gun,
            SkillType.Fire => Fire,
            SkillType.Ice => Ice,
            SkillType.Elec => Elec,
            SkillType.Force => Force,
            SkillType.Light => Light,
            SkillType.Dark => Dark,
            SkillType.Almighty => AffinityType.Neutral,
            _ => throw new ArgumentException($"Skill type has no corresponding affinity: {skillType}")
        };
    }
}