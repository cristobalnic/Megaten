﻿using Shin_Megami_Tensei.MegatenErrorHandling;

namespace Shin_Megami_Tensei.Entities;

public class Player
{
    public readonly int Id;
    public Samurai? Samurai { get; private set; }
    public readonly List<Unit> Units = [];
    public readonly Table Table = new();
    public int FullTurns { get; set; }
    public int BlinkingTurns { get; private set; }
    
    public Player(int id)
    {
        Id = id;
    }

    public void SetSamurai(Samurai samurai)
    {
        if (Samurai != null)
        {
            throw new InvalidTeamException();
        }
        Samurai = samurai;
    }

    public void AddUnit(Unit unit)
    {
        if (Units.Count >= Params.MaxUnitsAllowed)
            throw new InvalidTeamException();

        if (Units.Any(existingUnit => existingUnit.Name == unit.Name))
            throw new InvalidTeamException();
        
        Units.Add(unit);
    }
    
    public bool IsTeamValid() => Samurai != null;
    
    public void ResetRemainingTurns()
    {
        FullTurns = Table.Monsters.Count(monster => monster != null && monster.IsAlive());
        BlinkingTurns = 0;
    }
}