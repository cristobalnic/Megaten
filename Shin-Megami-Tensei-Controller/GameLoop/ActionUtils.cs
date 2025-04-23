using Shin_Megami_Tensei.Enums;

namespace Shin_Megami_Tensei.GameLoop;

public class ActionUtils
{
    public static string GetAttackPhrase(SkillType attackType)
    {
        return attackType switch
        {
            SkillType.Phys => "ataca",
            SkillType.Gun => "dispara",
            SkillType.Fire => "lanza fuego",
            SkillType.Ice => "lanza hielo",
            SkillType.Elec => "lanza electricidad",
            SkillType.Force => "lanza viento",
            _ => throw new NotImplementedException("Skill type not implemented for Attack Phrase")
        };
    }
}