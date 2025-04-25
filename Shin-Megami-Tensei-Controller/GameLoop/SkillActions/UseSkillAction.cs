using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.Enums;
using Shin_Megami_Tensei.GameLoop.Actions.AttackActions;
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
        Skill skill = GetSelectedSkill(attacker);
        _view.WriteLine(Params.Separator);

        if (skill.Type is SkillType.Light or SkillType.Dark)
            UseInstantKillSkill(attacker, skill);
        else if (skill.Type is SkillType.Heal)
            UseHealSkill(attacker, skill);
        else if (skill.Type is SkillType.Special)
            UseSpecialSkill();
        else
            UseAttackSkill(attacker, skill);
        
        attacker.Stats.Mp -= skill.Cost;
        _gameState.TurnPlayer.KSkillsUsed++;
    }

    

    private Skill GetSelectedSkill(Unit attacker)
    {
        _view.DisplaySkillSelection(attacker);
        var affordableSkills = attacker.Skills.Where(skill => attacker.Stats.Mp >= skill.Cost).ToList();
        var skillSelection = int.Parse(_view.ReadLine());
        if (skillSelection > affordableSkills.Count)
        {
            throw new CancelObjectiveSelectionException();
        }
        return attacker.Skills[skillSelection-1];
    }
    
    private void UseAttackSkill(Unit attacker, Skill skill)
    {
        var target = _selectionUtils.GetTarget(attacker);
        _view.WriteLine(Params.Separator);
        
        AffinityType targetAffinity = target.Affinity.GetAffinity(skill.Type);
        
        double baseDamage = GetSkillDamage(attacker, skill);
        var affinityDamage = AffinityUtils.GetDamageByAffinityRules(baseDamage, targetAffinity);
        var damage = AttackUtils.GetRoundedInt(affinityDamage);

        int hitNumber = AttackUtils.GetHits(skill.Hits, _gameState.TurnPlayer);
        for (int i = 0; i < hitNumber; i++)
        {
            AffinityUtils.DealDamageByAffinityRules(attacker, damage, target, targetAffinity);
            
            _view.DisplayAttackMessage(attacker, skill, target);
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
        throw new NotImplementedException($"Skill type {skill.Type} not implemented for Damage calculation");
    }
    
    private void UseInstantKillSkill(Unit attacker, Skill skill)
    {
        var target = _selectionUtils.GetTarget(attacker);
        _view.WriteLine(Params.Separator);
        
        AffinityType targetAffinity = AffinityUtils.GetTargetAffinity(skill, target);
        
        bool hasMissed = AffinityUtils.HasInstantKillSkillMissed(attacker, skill, target, targetAffinity);

        
        if (!hasMissed) AffinityUtils.ExecuteInstantKillByAffinityRules(attacker, target, targetAffinity);
        

        _view.DisplayAttackMessage(attacker, skill, target);
        if (!hasMissed) _view.DisplayAffinityDetectionMessage(attacker, target, targetAffinity);
        _view.DisplayInstantKillSkillResultMessage(attacker, target, targetAffinity, hasMissed);
        
        if (!target.IsAlive()) _gameState.WaitPlayer.Table.HandleDeath(target);
        if (!attacker.IsAlive()) _gameState.TurnPlayer.Table.HandleDeath(attacker);
        
        if (!hasMissed) TurnManager.HandleTurns(_gameState.TurnPlayer, targetAffinity);
        else _gameState.TurnPlayer.TurnState.UseTurnsForNonOffensiveSkill();
        
        _view.DisplayHpMessage(targetAffinity == AffinityType.Repel ? attacker : target);
    }
    
    private void UseHealSkill(Unit attacker, Skill skill)
    {
        if (skill.Effect.Contains("eals"))
        {
            // VERIFICAR SI HAY MUERTOS (a los que no se les puede curar)
            var beneficiary = _selectionUtils.GetAllyTarget(attacker);
            _view.WriteLine(Params.Separator);
            _view.WriteLine($"{attacker.Name} cura a {beneficiary.Name}");
            var healAmount = AttackUtils.GetRoundedInt(beneficiary.Stats.MaxHp * (skill.Power * 0.01));
            int currentHp = beneficiary.Stats.Hp;
            beneficiary.Stats.Hp = Math.Min(beneficiary.Stats.MaxHp, currentHp + healAmount);
            int healed = beneficiary.Stats.Hp - currentHp;
            _view.WriteLine($"{beneficiary.Name} recibe {healAmount} de HP"); // AQUÍ HAY UN ERROR EN TESTS
            _view.DisplayHpMessage(beneficiary);
            _gameState.TurnPlayer.TurnState.UseTurnsForNonOffensiveSkill();
        }
        else if (skill.Effect.Contains("KO"))
        {
            var beneficiary = _selectionUtils.GetDeadAllyTarget(attacker);
            var healAmount = AttackUtils.GetRoundedInt(beneficiary.Stats.MaxHp * (skill.Power * 0.01));
            _view.WriteLine(Params.Separator);
            _view.WriteLine($"{attacker.Name} revive a {beneficiary.Name}");
            int currentHp = beneficiary.Stats.Hp;
            beneficiary.Stats.Hp = Math.Min(beneficiary.Stats.MaxHp, currentHp + healAmount);
            int healed = beneficiary.Stats.Hp - currentHp;
            _view.WriteLine($"{beneficiary.Name} recibe {healAmount} de HP"); // AQUÍ HAY UN ERROR EN TESTS
            _view.DisplayHpMessage(beneficiary);
            _gameState.TurnPlayer.TurnState.UseTurnsForNonOffensiveSkill();
        }
        else
        {
            var summonAction = new SummonAction(_view, _gameState);
            summonAction.ExecuteHealSummon(attacker, skill);
        }
    }
    
    private void UseSpecialSkill()
    {
        var summonAction = new SummonAction(_view, _gameState);
        summonAction.ExecuteSpecialSummon();
    }
}