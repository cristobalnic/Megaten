using Shin_Megami_Tensei.Enums;

namespace Shin_Megami_Tensei.DataStructures;

public struct Affinity
{
    public required AffinityType Phys { get; set; }
    public required AffinityType Gun { get; set; }
    public required AffinityType Fire { get; set; }
    public required AffinityType Ice { get; set; }
    public required AffinityType Elec { get; set; }
    public required AffinityType Force { get; set; }
    public required AffinityType Light { get; set; }
    public required AffinityType Dark { get; set; }
    public required AffinityType Bind { get; set; }
    public required AffinityType Sleep { get; set; }
    public required AffinityType Sick { get; set; }
    public required AffinityType Panic { get; set; }
    public required AffinityType Poison { get; set; }

    public AffinityType GetAffinity(string selectedSkillType)
    {
        return selectedSkillType switch
        {
            "Phys" => Phys,
            "Gun" => Gun,
            "Fire" => Fire,
            "Ice" => Ice,
            "Elec" => Elec,
            "Force" => Force,
            "Light" => Light,
            "Dark" => Dark,
            "Bind" => Bind,
            "Sleep" => Sleep,
            "Sick" => Sick,
            "Panic" => Panic,
            "Poison" => Poison,
            _ => throw new ArgumentException($"Invalid skill type: {selectedSkillType}")
        };
    }
}