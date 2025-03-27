using Shin_Megami_Tensei.DataStructures;
using Shin_Megami_Tensei.Entities;

namespace Shin_Megami_Tensei;

public class Table
{
    public Unit? Samurai;
    private readonly List<Unit> _monsters = [];
    private readonly List<Unit> _reserve = [];


    public void SetSamurai(Unit? samurai)
    {
        Samurai = samurai;
    }

    public void AddMonster(Unit monster)
    {
        if (_monsters.Count < Params.MaxUnitsAllowedInTablePerSide)
        {
            _monsters.Add(monster);
        }
        else
        {
            _reserve.Add(monster);
        }
    }
}