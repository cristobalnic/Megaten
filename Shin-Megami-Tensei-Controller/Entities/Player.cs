namespace Shin_Megami_Tensei.Entities;

public class Player
{
    public int Id;
    public readonly List<Unit> Samurais = [];
    public readonly List<Unit> Units = [];
    
    
    public Player(int id)
    {
        Id = id;
    }

    public void AddSamurai(Unit unit)
    {
        Samurais.Add(unit);
    }

    public void AddUnit(Unit unit)
    {
        Units.Add(unit);
    }
}