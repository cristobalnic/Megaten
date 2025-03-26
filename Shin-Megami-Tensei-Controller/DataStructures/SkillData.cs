namespace Shin_Megami_Tensei.DataStructures;

public class SkillData
{
    public required string Name { get; set; }
    public required string Type { get; set; }
    public int Cost { get; set; }
    public int Power { get; set; }
    public required string Target { get; set; }
    public required string Hits { get; set; }
    public required string Effect { get; set; }
}