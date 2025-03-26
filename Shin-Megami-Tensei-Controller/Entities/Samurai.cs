using Shin_Megami_Tensei.DataStructures;

namespace Shin_Megami_Tensei.Entities;

public class Samurai : Unit
{
    public Samurai(UnitData data) : base(data) 
    {
        Skills = [];
    }
    
    public void EquipSkill(string skill)
    {
        Skills.Add(skill);
        Console.WriteLine($"Skill equipped! {skill}");
    }
}