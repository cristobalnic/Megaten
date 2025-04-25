using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.Enums;
using Shin_Megami_Tensei.Views;

namespace Shin_Megami_Tensei.GameLoop.Actions.AttackActions;

public abstract class BaseAttack
{
    private readonly IView _view;
    private readonly GameState _gameState;
    private readonly SelectionUtils _selectionUtils;

    protected BaseAttack(IView view, GameState gameState)
    {
        _view = view;
        _gameState = gameState;
        _selectionUtils = new SelectionUtils(view, gameState);
    }

    internal void Execute(Unit attacker)
    {
        var target = _selectionUtils.GetTarget(attacker);
        AffinityType affinity = GetAffinity(target);
        _view.WriteLine(Params.Separator);
        HandleDamage(attacker, target, affinity);
        TurnManager.HandleTurns(_gameState.TurnPlayer, affinity);
    }

    private void HandleDamage(Unit attacker, Unit target, AffinityType targetAffinity)
    {
        var baseDamage = GetBaseDamage(attacker);
        var affinityDamage = AffinityUtils.GetDamageByAffinityRules(baseDamage, targetAffinity);
        var damage = AttackUtils.GetRoundedInt(affinityDamage);
        AffinityUtils.DealDamageByAffinityRules(attacker, damage, target, targetAffinity);
        
        _view.WriteLine(GetActionMessage(attacker, target));
        _view.DisplayAffinityDetectionMessage(attacker, target, targetAffinity);
        _view.DisplayAttackResultMessage(attacker, damage, target, targetAffinity);
        
        if (!target.IsAlive()) _gameState.WaitPlayer.Table.HandleDeath(target);
        if (!attacker.IsAlive()) _gameState.TurnPlayer.Table.HandleDeath(attacker);
        
        _view.DisplayHpMessage(targetAffinity == AffinityType.Repel ? attacker : target);
    }

    protected abstract string GetActionMessage(Unit attacker, Unit target);
    protected abstract double GetBaseDamage(Unit attacker);
    protected abstract AffinityType GetAffinity(Unit target);
}