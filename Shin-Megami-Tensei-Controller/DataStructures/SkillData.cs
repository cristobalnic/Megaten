﻿using Shin_Megami_Tensei.Enums;

namespace Shin_Megami_Tensei.DataStructures;

public struct SkillData
{
    public required string Name { get; set; }
    public required SkillType Type { get; set; }
    public int Cost { get; set; }
    public int Power { get; set; }
    public required string Target { get; set; }
    public required string Hits { get; set; }
    public required string Effect { get; set; }
}