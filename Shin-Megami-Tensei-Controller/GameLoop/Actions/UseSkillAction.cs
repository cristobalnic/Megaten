using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.Enums;
using Shin_Megami_Tensei.MegatenErrorHandling;
using Shin_Megami_Tensei.Views;

namespace Shin_Megami_Tensei.GameLoop.Actions;

public class UseSkillAction
{
    private readonly IView _view;
    private readonly GameState _gameState;
    private readonly SelectionUtils _selectionUtils;
    
    public UseSkillAction(IView view, GameState gameState)
    {
        _view = view;
        _gameState = gameState;
        _selectionUtils = new SelectionUtils(view, gameState);
    }

    internal void ExecuteUseSkill(Unit attacker)
    {
        _view.DisplaySkillSelection(attacker);
        Skill skill = GetSelectedSkill(attacker);
        _view.WriteLine(Params.Separator);
        var target = _selectionUtils.GetTarget(attacker);
        _view.WriteLine(Params.Separator);
        UseAttackSkill(attacker, skill, target);
        attacker.Stats.Mp -= skill.Cost;
        _gameState.TurnPlayer.KSkillsUsed++;
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
    
    private void UseAttackSkill(Unit attacker, Skill selectedSkill, Unit target)
    {
        AffinityType targetAffinity = target.Affinity.GetAffinity(selectedSkill.Type);
        
        double baseDamage = GetSkillDamage(attacker, selectedSkill);
        var affinityDamage = AffinityHandler.GetDamageByAffinityRules(baseDamage, targetAffinity);
        var damage = ActionUtils.GetRoundedIntDamage(affinityDamage);

        int hitNumber = ActionUtils.GetHits(selectedSkill.Hits, _gameState.TurnPlayer);
        for (int i = 0; i < hitNumber; i++)
        {
            AffinityHandler.DealDamageByAffinityRules(attacker, damage, target, targetAffinity);
            
            _view.DisplayAttackMessage(attacker, selectedSkill, target);
            _view.DisplayAffinityDetectionMessage(attacker, target, targetAffinity);
            _view.DisplayAttackResultMessage(attacker, damage, target, targetAffinity);
            
            if (!target.IsAlive()) _gameState.WaitPlayer.Table.HandleDeath(target);
            if (!attacker.IsAlive()) _gameState.TurnPlayer.Table.HandleDeath(attacker);
        }
        TurnManager.HandleTurns(_gameState.TurnPlayer, targetAffinity);
        _view.DisplayHpMessage(targetAffinity == AffinityType.Repel ? attacker : target);
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
    
    // private void UseInstantKillSkill(Unit attacker, Skill selectedSkill, Unit target)
    // {
    //     bool skillHasMissed = HasSkillMissed(attacker, selectedSkill, target);
    //     
    //     if (skillHasMissed)
    //     {
    //         _view.WriteLine($"{attacker.Name} ha fallado con el ataque");
    //         _view.DisplayHpMessage(target);
    //     }
    // }
    //
    // private bool HasSkillMissed(Unit attacker, Skill selectedSkill, Unit target)    
    // {
    //     AffinityType targetAffinity = target.Affinity.GetAffinity(selectedSkill.Type);
    // }




}