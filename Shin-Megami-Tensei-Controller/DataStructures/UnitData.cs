namespace Shin_Megami_Tensei.DataStructures;

public struct UnitData
{
    public required string Name { get; set; }
    public Stats Stats { get; set; }
    public Affinity Affinity { get; set; }
    public List<string> Skills { get; set; }
}
