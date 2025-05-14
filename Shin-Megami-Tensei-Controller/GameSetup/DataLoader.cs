using System.Text.Json;
using System.Text.Json.Serialization;
using Shin_Megami_Tensei.DataStructures;
using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.Utils;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Shin_Megami_Tensei.GameSetup;

public class DataLoader
{
    private readonly JsonSerializerOptions _options;
    private readonly List<UnitData> _samurais;
    private readonly List<UnitData> _monsters;
    private readonly List<SkillData> _skills;

    public DataLoader()
    {
        _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };
        _samurais = DeserializeUnits(Params.SamuraiPath);
        _monsters = DeserializeUnits(Params.MonsterPath);
        _skills = DeserializeSkills(Params.SkillPath);
    }

    private  List<UnitData> DeserializeUnits(string path)
    {
        var rawJson = File.ReadAllText(path);
        var json = StringFormatter.NormalizeAffinityValues(rawJson);
        return JsonSerializer.Deserialize<List<UnitData>>(json, _options) ?? [];
    }

    private  List<SkillData> DeserializeSkills(string path)
    {
        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<List<SkillData>>(json, _options) ?? [];
    }
    
    public  void LoadSamuraiUnitToPlayer(string samuraiRawData, Player currentPlayer)
    {
        var samuraiName = StringFormatter.GetSamuraiName(samuraiRawData);
        var samuraiData = _samurais.First(samurai => samurai.Name == samuraiName);
        var samurai = new Samurai(samuraiData);
        currentPlayer.SetSamurai(samurai);
    }

    public  void LoadSkillsToSamurai(string samuraiRawData, Samurai samurai)
    {
        if (!samuraiRawData.Contains('(')) return;
        var samuraiSkillsNames = StringFormatter.GetSamuraiSkills(samuraiRawData);
        foreach (var skillName in samuraiSkillsNames)
        {
            var skillData = GetSkillDataFromJson(skillName);
            samurai.EquipSkill(skillData);
        }
    }

    public  SkillData GetSkillDataFromJson(string skillName)
    {
        var skillData = _skills.First(skill => skill.Name == skillName);
        return skillData;
    }

    public  void LoadMonsterUnitToPlayer(string monsterName, Player currentPlayer)
    {
        var monsterData = _monsters.First(monster => monster.Name == monsterName);
        var monster = new Monster(monsterData);
        currentPlayer.AddUnit(monster);
    }
}