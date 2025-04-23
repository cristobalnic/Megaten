using Shin_Megami_Tensei.DataStructures;
using Shin_Megami_Tensei.GameLoop;
using Shin_Megami_Tensei.Utils;

namespace Shin_Megami_Tensei.Entities;

public class Monster : Unit
{
    private readonly DataLoader _dataLoader = new();
    public static readonly Monster EmptySlot = new(new UnitData { Name = "Vacío" , Skills = []});
    
    public Monster(UnitData unitData) : base(unitData)
    {
        foreach (var skillName in unitData.Skills)
        {
            var skillData = _dataLoader.GetSkillDataFromJson(skillName);
            Skills.Add(new Skill(skillData));
        }
    }

    public override void Summon(Unit monsterSummon, Table summonerTable, ActionsUtils actionsUtils) 
        => summonerTable.ReplaceMonster(this, monsterSummon);
}