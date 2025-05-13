using Shin_Megami_Tensei.MegatenErrorHandling;

namespace Shin_Megami_Tensei.Entities;

public class Player
{
    public readonly int Id;
    public Samurai Samurai { get; private set; } = Samurai.Empty;
    public readonly List<Unit> Units = [];
    public readonly Table Table;
    public readonly TurnState TurnState = new();
    public int KSkillsUsed = 0;
    
    public Player(int id)
    {
        Id = id;
        Table = new Table(this);
    }

    public void SetSamurai(Samurai samurai) => Samurai = samurai;

    public void AddUnit(Unit unit)
    {
        if (Units.Count >= Params.MaxUnitsAllowed)
            throw new InvalidTeamException();

        if (Units.Any(existingUnit => existingUnit.Name == unit.Name))
            throw new InvalidTeamException();
        
        Units.Add(unit);
    }

    public bool IsTeamValid() => Samurai != Samurai.Empty;

    public List<Unit> GetAllUnits()
    {
        var allUnits = new List<Unit>();
        allUnits.Add(Samurai);
        allUnits.AddRange(Units);
        return allUnits;
    }
}