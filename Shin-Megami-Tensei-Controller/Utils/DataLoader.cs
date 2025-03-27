using System.Text.Json;
using Shin_Megami_Tensei.DataStructures;
using Shin_Megami_Tensei.Entities;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Shin_Megami_Tensei.Utils;

public abstract class DataLoader
{
    private static readonly JsonSerializerOptions Options = new() { PropertyNameCaseInsensitive = true };
    private static readonly List<UnitData> Samurais = DeserializeUnits(Params.SamuraiPath);
    private static readonly List<UnitData> Monsters = DeserializeUnits(Params.MonsterPath);
    private static readonly List<SkillData> Skills = DeserializeSkills(Params.SkillPath);

    private static List<UnitData> DeserializeUnits(string path) 
    {
        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<List<UnitData>>(json, Options) ?? [];
    }

    private static List<SkillData> DeserializeSkills(string path)
    {
        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<List<SkillData>>(json, Options) ?? [];
    }

    public static void LoadSamuraiUnitToPlayer(string samuraiRawData, Player currentPlayer)
    {
        var samuraiName = StringFormatter.GetSamuraiName(samuraiRawData);
        var samuraiData = Samurais.First(samurai => samurai.Name == samuraiName);
        var samurai = new Samurai(samuraiData);
        currentPlayer.SetSamurai(samurai);
    }

    public static void LoadSkillsToSamurai(string samuraiRawData, Samurai samurai)
    {
        if (!samuraiRawData.Contains('(')) return;
        var samuraiSkillsNames = StringFormatter.GetSamuraiSkills(samuraiRawData);
        foreach (var skillName in samuraiSkillsNames)
        {
            var skillData = GetSkillDataFromDeserializedJson(skillName);
            samurai.EquipSkill(skillData);
        }
    }

    public static SkillData GetSkillDataFromDeserializedJson(string skillName)
    {
        var skillData = Skills.First(skill => skill.Name == skillName);
        return skillData;
    }

    public static void LoadMonsterUnitToPlayer(string monsterName, Player currentPlayer)
    {
        var monsterData = Monsters.First(monster => monster.Name == monsterName);
        var monster = new Monster(monsterData);
        currentPlayer.AddUnit(monster);
    }
}