using Shin_Megami_Tensei.DataStructures;
using Shin_Megami_Tensei.Entities;

namespace Shin_Megami_Tensei;

public class Table
{
    public Samurai? Samurai;
    public readonly List<Unit> Monsters = [];
    private readonly List<Unit> _reserve = [];


    public void SetSamurai(Samurai? samurai)
    {
        Samurai = samurai;
        if (Samurai != null) AddMonster(Samurai);
    }

    public void AddMonster(Unit monster)
    {
        if (Monsters.Count < Params.MaxUnitsAllowedInTablePerSide)
        {
            Monsters.Add(monster);
        }
        else
        {
            _reserve.Add(monster);
        }
    }
}