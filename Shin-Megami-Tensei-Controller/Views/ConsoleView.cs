using Shin_Megami_Tensei_View;
using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.Enums;
using Shin_Megami_Tensei.GameLoop;

namespace Shin_Megami_Tensei.Views;

public class ConsoleView : IView
{
    private readonly View _view;

    private ConsoleView(View view) => _view = view;

    public static ConsoleView SetConsoleView(View view) => new(view);

    public void WriteLine(string message) 
        => _view.WriteLine(message);

    public string ReadLine() 
        => _view.ReadLine();

    public void DisplayHpMessage(Unit monster) 
        => _view.WriteLine($"{monster.Name} termina con HP:{monster.Stats.Hp}/{monster.Stats.MaxHp}");
    
    public void DisplayAttackMessage(Unit attacker, Skill selectedSkill, Unit target) 
        => _view.WriteLine($"{attacker.Name} {GetAttackPhrase(selectedSkill.Type)} a {target.Name}");

    public void DisplayDamageMessage(Unit monster, int damage) 
        => _view.WriteLine($"{monster.Name} recibe {damage} de daño");

    public void DisplayRepeledDamageMessage(Unit target, int repelDamage, Unit attacker) 
        => _view.WriteLine($"{target.Name} devuelve {repelDamage} daño a {attacker.Name}");

    public void DisplayDrainDamageMessage(Unit target, int drainDamage) 
        => _view.WriteLine($"{target.Name} absorbe {drainDamage} daño");

    private static string GetAttackPhrase(SkillType attackType)
    {
        return attackType switch
        {
            SkillType.Phys => "ataca",
            SkillType.Gun => "dispara",
            SkillType.Fire => "lanza fuego",
            SkillType.Ice => "lanza hielo",
            SkillType.Elec => "lanza electricidad",
            SkillType.Force => "lanza viento",
            SkillType.Light => "ataca con luz",
            SkillType.Dark => "ataca con oscuridad",
            _ => throw new NotImplementedException("Skill type not implemented for Attack Phrase")
        };
    }
}