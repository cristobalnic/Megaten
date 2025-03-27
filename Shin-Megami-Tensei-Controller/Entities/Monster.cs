using Shin_Megami_Tensei.DataStructures;
using Shin_Megami_Tensei.Utils;

namespace Shin_Megami_Tensei.Entities;

public class Monster : Unit
{
    public Monster(UnitData unitData) : base(unitData)
    {
        foreach (var skillName in unitData.Skills!)
        {
            SkillData skillData = DataLoader.GetSkillDataFromDeserializedJson(skillName);
            Skills.Add(new Skill(skillData));
        }
    }
}