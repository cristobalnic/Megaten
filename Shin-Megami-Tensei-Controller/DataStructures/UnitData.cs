namespace Shin_Megami_Tensei.DataStructures;

public class UnitData
{
    public required string Name { get; set; }
    public required Stats Stats { get; set; }
    public required Affinity Affinity { get; set; }
    public List<string>? Skills { get; set; }
}
