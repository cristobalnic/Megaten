using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.Enums;

namespace Shin_Megami_Tensei.Skills.SkillActions;

public class SkillUtils
{
    public static int GetHits(string hitsString, Player turnPlayer)
    {
        if (hitsString.Contains('-'))
        {
            var parts = hitsString.Split('-');
            int minHits = int.Parse(parts[0]);
            int maxHits = int.Parse(parts[1]);
            int offset = turnPlayer.KSkillsUsed % (maxHits - minHits + 1);
            return minHits + offset;
        }
        return int.Parse(hitsString);
    }
    
    public static double GetSkillDamage(Unit attacker, Skill skill)
    {
        if (skill.Type == SkillType.Phys)
            return Math.Sqrt(attacker.Stats.Str * skill.Power);
        if (skill.Type == SkillType.Gun)
            return Math.Sqrt(attacker.Stats.Skl * skill.Power);
        if (skill.Type is SkillType.Fire or SkillType.Ice or SkillType.Elec or SkillType.Force or SkillType.Almighty)
            return Math.Sqrt(attacker.Stats.Mag * skill.Power);
        throw new NotImplementedException($"Skill type {skill.Type} not implemented for Damage calculation");
    }
}