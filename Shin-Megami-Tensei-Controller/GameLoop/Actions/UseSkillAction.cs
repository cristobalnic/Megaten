using Shin_Megami_Tensei_View;
using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.MegatenErrorHandling;

namespace Shin_Megami_Tensei.GameLoop.Actions;

public class UseSkillAction
{
    private readonly View _view;
    private readonly GameState _gameState;
    
    public UseSkillAction(View view, GameState gameState)
    {
        _view = view;
        _gameState = gameState;
    }

    internal void ExecuteUseSkill(Unit monster)
    {
        _view.WriteLine($"Seleccione una habilidad para que {monster.Name} use");
        int label = 1;
        foreach (var skill in monster.Skills)
        {
            if (monster.Stats.Mp < skill.Cost)
                continue;
            _view.WriteLine($"{label}-{skill.Name} MP:{skill.Cost}");
            label++;
        }
        _view.WriteLine($"{label}-Cancelar");
        Skill selectedSkill = GetSelectedSkill(monster);
        int damage = Convert.ToInt32(Math.Floor(Math.Max(0, GetSkillDamage(monster, selectedSkill))));
    }

    private Skill GetSelectedSkill(Unit monster)
    {
        var affordableSkills = monster.Skills.Where(skill => monster.Stats.Mp >= skill.Cost).ToList();
        var skillSelection = int.Parse(_view.ReadLine());
        if (skillSelection > affordableSkills.Count)
        {
            throw new CancelObjectiveSelectionException();
        }
        return monster.Skills[skillSelection-1];
    }
    
    private double GetSkillDamage(Unit monster, Skill skill)
    {
        return Math.Sqrt(monster.Stats.Mag * skill.Power);
    }
}