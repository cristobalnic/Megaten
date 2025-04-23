using Shin_Megami_Tensei_View;
using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.Enums;
using Shin_Megami_Tensei.MegatenErrorHandling;

namespace Shin_Megami_Tensei.GameLoop.Actions;

public class UseSkillAction
{
    private readonly View _view;
    private readonly GameState _gameState;
    private readonly ActionsUtils _actionsUtils;
    
    public UseSkillAction(View view, GameState gameState)
    {
        _view = view;
        _gameState = gameState;
        _actionsUtils = new ActionsUtils(view, gameState);
    }

    internal void ExecuteUseSkill(Unit attacker)
    {
        DisplaySkillSelection(attacker);
        Skill selectedSkill = GetSelectedSkill(attacker);
        _view.WriteLine(Params.Separator);
        var target = _actionsUtils.GetTarget(attacker);
        _view.WriteLine(Params.Separator);
        string attackPhrase = GetAttackPhrase(selectedSkill);
        AffinityType targetAffinity = target.Affinity.GetAffinity(selectedSkill.Type);
        double baseDamage = GetSkillDamage(attacker, selectedSkill);
        
        // IMPLEMENT HITS
        _view.WriteLine($"{attacker.Name} {attackPhrase} a {target.Name}");
        _actionsUtils.DealDamage(attacker, target, baseDamage, targetAffinity);
        // WORK HERE!!!
        
        attacker.Stats.Mp -= selectedSkill.Cost;
    }

    private string GetAttackPhrase(Skill selectedSkill)
    {
        if (selectedSkill.Type == "Phys")
            return "ataca";
        if (selectedSkill.Type == "Gun")
            return "dispara";
        if (selectedSkill.Type is "Fire")
            return "lanza fuego";
        if (selectedSkill.Type is "Ice")
            return "lanza hielo";
        if (selectedSkill.Type is "Elec")
            return "lanza electricidad";
        if (selectedSkill.Type is "Force")
            return "lanza viento";
        throw new NotImplementedException("Skill type not implemented PHRASE");
    }

    private void DisplaySkillSelection(Unit attacker)
    {
        _view.WriteLine($"Seleccione una habilidad para que {attacker.Name} use");
        int label = 1;
        foreach (var skill in attacker.Skills)
        {
            if (attacker.Stats.Mp < skill.Cost)
                continue;
            _view.WriteLine($"{label}-{skill.Name} MP:{skill.Cost}");
            label++;
        }
        _view.WriteLine($"{label}-Cancelar");
    }

    private Skill GetSelectedSkill(Unit attacker)
    {
        var affordableSkills = attacker.Skills.Where(skill => attacker.Stats.Mp >= skill.Cost).ToList();
        var skillSelection = int.Parse(_view.ReadLine());
        if (skillSelection > affordableSkills.Count)
        {
            throw new CancelObjectiveSelectionException();
        }
        return attacker.Skills[skillSelection-1];
    }
    
    private double GetSkillDamage(Unit attacker, Skill skill)
    {
        if (skill.Type == "Phys")
            return Math.Sqrt(attacker.Stats.Str * skill.Power);
        if (skill.Type == "Gun")
            return Math.Sqrt(attacker.Stats.Skl * skill.Power);
        if (skill.Type is "Fire" or "Ice" or "Elec" or "Force" or "Almighty")
            return Math.Sqrt(attacker.Stats.Mag * skill.Power);
        throw new NotImplementedException("Skill type not implemented DAMAGE");
    }
}