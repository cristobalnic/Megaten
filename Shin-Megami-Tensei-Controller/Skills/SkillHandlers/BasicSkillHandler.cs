using Shin_Megami_Tensei.Affinities;
using Shin_Megami_Tensei.DataStructures;
using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.Enums;
using Shin_Megami_Tensei.GameActions.AttackActions;
using Shin_Megami_Tensei.GameData;
using Shin_Megami_Tensei.Skills.SkillActions;
using Shin_Megami_Tensei.Utils;
using Shin_Megami_Tensei.Views;

namespace Shin_Megami_Tensei.Skills.SkillHandlers;

public class BasicSkillHandler : ISkillHandler
{
    private readonly IView _view;
    private readonly GameState _gameState;
    private readonly SelectionUtils _selectionUtils;

    public BasicSkillHandler(IView view, GameState gameState)
    {
        _view = view;
        _gameState = gameState;
        _selectionUtils = new SelectionUtils(_view, gameState);
    }

    public void Execute(Unit attacker, Skill skill)
    {
        UseBasicSkill(attacker, skill);
    }

    private void UseBasicSkill(Unit attacker, Skill skill)
    {
        var target = _selectionUtils.GetTarget(attacker);
        _view.WriteLine(Params.Separator);
        
        AffinityType targetAffinity = target.Affinity.GetAffinity(skill.Type);
        
        double baseDamage = SkillUtils.GetSkillDamage(attacker, skill);
        var affinityDamage = AffinityUtils.GetDamageByAffinityRules(baseDamage, targetAffinity);
        var damage = AttackUtils.GetRoundedInt(affinityDamage);
        
        CombatRecord combatRecord = new CombatRecord(attacker, target, damage, targetAffinity);
        var affinityHandler = AffinityHandlerFactory.CreateAffinityHandler(combatRecord.Affinity);

        int hitNumber = SkillUtils.GetHits(skill.Hits, _gameState.TurnPlayer);
        for (int i = 0; i < hitNumber; i++)
        {
            affinityHandler.DealDamageByAffinityRules(combatRecord);
            _view.DisplayAttackMessage(combatRecord, skill);
            _view.DisplayAffinityDetectionMessage(combatRecord);
            _view.DisplayAttackResultMessage(combatRecord);
            
            if (!target.IsAlive()) _gameState.WaitPlayer.Table.HandleDeath(target);
            if (!attacker.IsAlive()) _gameState.TurnPlayer.Table.HandleDeath(attacker);
        }
        _gameState.TurnPlayer.TurnState.UseTurnsByTargetAffinity(targetAffinity);
        var damagedUnit = affinityHandler.GetDamagedUnit(combatRecord);
        _view.DisplayHpMessage(damagedUnit);
    }
}