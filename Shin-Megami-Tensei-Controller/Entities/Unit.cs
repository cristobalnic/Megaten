﻿using Shin_Megami_Tensei.DataStructures;

namespace Shin_Megami_Tensei.Entities;

public abstract class Unit
{
    public readonly string Name;
    public Stats Stats;
    public Affinity Affinity;
    public readonly List<Skill> Skills = [];

    protected Unit(UnitData unitData)
    {
        Name = unitData.Name;
        Stats = unitData.Stats;
        Stats.MaxHp = unitData.Stats.Hp;
        Stats.MaxMp = unitData.Stats.Mp;
        Affinity= unitData.Affinity;
    }
    
    public bool IsAlive()
    {
        return Stats.Hp > 0;
    }
}