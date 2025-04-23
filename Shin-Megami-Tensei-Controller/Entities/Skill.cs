using Shin_Megami_Tensei.DataStructures;
using Shin_Megami_Tensei.Enums;

namespace Shin_Megami_Tensei.Entities;

public class Skill
{
    public readonly string Name;
    public readonly SkillType Type;
    public readonly int Cost;
    public readonly int Power;
    public readonly string Target;
    public readonly string Hits;
    public readonly string Effect;
    
    public Skill(SkillData skillData)
    {
        Name = skillData.Name;
        Type = skillData.Type;
        Cost = skillData.Cost;
        Power = skillData.Power;
        Target = skillData.Target;
        Hits = skillData.Hits;
        Effect = skillData.Effect;
    }
}