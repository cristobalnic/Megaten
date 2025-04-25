using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.Enums;
using Shin_Megami_Tensei.Views;

namespace Shin_Megami_Tensei.GameLoop.Actions;

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
        HandleDamage(attacker, target, affinity);
        TurnManager.HandleTurns(_gameState.TurnPlayer, affinity);
    }

    private void HandleDamage(Unit attacker, Unit target, AffinityType affinity)
    {
        _view.WriteLine(Params.Separator);
        _view.WriteLine(GetActionMessage(attacker, target));
        double baseDamage = GetBaseDamage(attacker);
        DealDamage(attacker, target, baseDamage, affinity);
        _view.DisplayHpMessage(affinity == AffinityType.Repel ? attacker : target);
    }

    private void DealDamage(Unit attacker, Unit target, double baseDamage, AffinityType affinityType)
    {
        var affinityHandler = new AffinityHandler(_view);
        affinityHandler.HandleAffinityEffect(attacker, target, baseDamage, affinityType);

        if (!target.IsAlive()) _gameState.WaitPlayer.Table.HandleDeath(target);
        if (!attacker.IsAlive()) _gameState.TurnPlayer.Table.HandleDeath(attacker);
    }

    protected abstract string GetActionMessage(Unit attacker, Unit target);
    protected abstract double GetBaseDamage(Unit attacker);
    protected abstract AffinityType GetAffinity(Unit target);
}