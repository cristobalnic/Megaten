using System.Text.Json;
using Shin_Megami_Tensei.DataStructures;
using Shin_Megami_Tensei.Entities;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Shin_Megami_Tensei.Utils;

public abstract class DataLoader
{
    private static UnitData LoadUnitData(string path, string unitName)
    {
        var json = File.ReadAllText(path);
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var samurais = JsonSerializer.Deserialize<List<UnitData>>(json, options);
        return samurais!.First(samurai => samurai.Name == unitName);
    }

    public static void LoadSamuraiUnit(string unitRawData, Player currentPlayer)
    {
        var lineInfo = StringFormatter.SplitSamuraiInfo(unitRawData.Replace("[Samurai] ", ""));
        var samuraiName = lineInfo[0];
        var samuraiData = LoadUnitData(Params.SamuraiPath, samuraiName);
        var samurai = new Samurai(samuraiData);
        currentPlayer.AddSamurai(samurai);
        var samuraiSkills = lineInfo[1].Split(",");
        foreach (var skill in samuraiSkills) samurai.EquipSkill(skill);
    }

    public static void LoadMonsterUnit(string line, Player currentPlayer)
    {
        var monsterName = line;
        var monsterData = LoadUnitData(Params.MonsterPath, monsterName);
        var monster = new Monster(monsterData);
        currentPlayer.AddUnit(monster);
    }
}




