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
        if (selectedSkill.Type == SkillType.Phys)
            return "ataca";
        if (selectedSkill.Type == SkillType.Gun)
            return "dispara";
        if (selectedSkill.Type == SkillType.Fire)
            return "lanza fuego";
        if (selectedSkill.Type == SkillType.Ice)
            return "lanza hielo";
        if (selectedSkill.Type == SkillType.Elec)
            return "lanza electricidad";
        if (selectedSkill.Type == SkillType.Force)
            return "lanza viento";
        throw new NotImplementedException("Skill type not implemented for Attack Phrase");
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
        if (skill.Type == SkillType.Phys)
            return Math.Sqrt(attacker.Stats.Str * skill.Power);
        if (skill.Type == SkillType.Gun)
            return Math.Sqrt(attacker.Stats.Skl * skill.Power);
        if (skill.Type is SkillType.Fire or SkillType.Ice or SkillType.Elec or SkillType.Force or SkillType.Almighty)
            return Math.Sqrt(attacker.Stats.Mag * skill.Power);
        throw new NotImplementedException("Skill type not implemented for Damage calculation");
    }
}