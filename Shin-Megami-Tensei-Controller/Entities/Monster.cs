using Shin_Megami_Tensei.DataStructures;
using Shin_Megami_Tensei.Utils;

namespace Shin_Megami_Tensei.Entities;

public class Monster : Unit
{
    private readonly DataLoader _dataLoader = new();
    public Monster(UnitData unitData) : base(unitData)
    {
        foreach (var skillName in unitData.Skills)
        {
            SkillData skillData = _dataLoader.GetSkillDataFromDeserializedJson(skillName);
            Skills.Add(new Skill(skillData));
        }
    }
}