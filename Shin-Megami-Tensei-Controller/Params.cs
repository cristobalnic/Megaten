namespace Shin_Megami_Tensei;

public static class Params
{
    public static string SamuraiPath => "data/samurai.json";
    public static string MonsterPath => "data/monsters.json";
    public static string SkillPath => "data/skills.json";
    public const int MaxUnitsAllowed = 7;
    public const int RequiredSamurais = 1;
    public const int MaxSamuraiSkillsAllowed = 8;
    public const int MaxUnitsAllowedInTablePerSide = 4;

    public const string Separator = "----------------------------------------";

    public const int AttackDamageModifier = 54;
    public const int ShootDamageModifier = 80;
    public const double AttackAndShootDamageMultiplier = 0.0114;
    
    public static string[] SamuraiActions { get; } =
        [
            "Atacar",
            "Disparar",
            "Usar Habilidad",
            "Invocar",
            "Pasar Turno",
            "Rendirse"
        ];
    
    public static string[] MonsterActions { get; } =
        [
            "Atacar",
            "Usar Habilidad",
            "Invocar",
            "Pasar Turno"
        ];

    public const double WeakDamageMultiplier = 1.5;
    public const double ResistDamageMultiplier = 0.5;
    public const double NullDamageMultiplier = 0.0;
    
    public const string EmptyUnitName = "Vacío";
}