using Shin_Megami_Tensei.DataStructures;

namespace Shin_Megami_Tensei.Entities;

public class Skill
{
    public readonly string Name;
    public readonly string Type;
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