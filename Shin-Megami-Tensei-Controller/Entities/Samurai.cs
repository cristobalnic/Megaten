using Shin_Megami_Tensei.DataStructures;
using Shin_Megami_Tensei.MegatenErrorHandling;

namespace Shin_Megami_Tensei.Entities;

public class Samurai(UnitData unitData) : Unit(unitData)
{
    public void EquipSkill(SkillData skillData)
    {
        if (Skills.Count >= Params.MaxSamuraiSkillsAllowed)
            throw new InvalidTeamException();

        if (Skills.Any(existingSkill => existingSkill.Name == skillData.Name))
            throw new InvalidTeamException();
        
        Skills.Add(new Skill(skillData));
    }

    public override void Summon(Unit summoner, Unit monsterSummon, Table summonerTable)
    {
        throw new NotImplementedException();
    }
}