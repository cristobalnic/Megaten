using Shin_Megami_Tensei.DataStructures;
using Shin_Megami_Tensei.ErrorHandling;
using Shin_Megami_Tensei.GameData;
using Shin_Megami_Tensei.GameLoop;

namespace Shin_Megami_Tensei.Entities;

public class Samurai(UnitData unitData) : Unit(unitData)
{
    internal static readonly Samurai Empty = new(new UnitData { Name = Params.EmptyUnitName });
    
    public void EquipSkill(SkillData skillData)
    {
        if (Skills.Count >= Params.MaxSamuraiSkillsAllowed)
            throw new InvalidTeamException();

        if (Skills.Any(existingSkill => existingSkill.Name == skillData.Name))
            throw new InvalidTeamException();
        
        Skills.Add(new Skill(skillData));
    }

    public override void Summon(Unit monsterSummon, Table summonerTable, SelectionUtils selectionUtils)
    {
        selectionUtils.DisplaySummonWithdrawSelection(summonerTable.ActiveUnits);
        Unit monsterWithdraw = selectionUtils.GetSummonWithdrawSelection(summonerTable.ActiveUnits);
        summonerTable.ReplaceMonster(monsterWithdraw, monsterSummon);
    }
}