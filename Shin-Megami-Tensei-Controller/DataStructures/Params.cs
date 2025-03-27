namespace Shin_Megami_Tensei.DataStructures;

public static class Params
{
    public static string SamuraiPath { get; } = "data/samurai.json";
    public static string MonsterPath { get; } = "data/monsters.json";
    public static string SkillPath { get; } = "data/skills.json";
    public const int MaxUnitsAllowed = 7;
    public const int RequiredSamurais = 1;
    public const int MaxSamuraiSkillsAllowed = 8;
    public const int MaxUnitsAllowedInTablePerSide = 3;

    public const string Separator = "----------------------------------------";
}