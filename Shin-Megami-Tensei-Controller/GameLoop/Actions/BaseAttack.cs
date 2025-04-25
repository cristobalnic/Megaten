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
        _view.WriteLine(Params.Separator);
        HandleDamage(attacker, target);
    }

    protected void HandleDamage(Unit attacker, Unit target)
    {
        _view.WriteLine(GetActionMessage(attacker, target));
        double baseDamage = GetBaseDamage(attacker);
        AffinityType affinity = GetAffinity(target);
        _selectionUtils.DealDamage(attacker, target, baseDamage, affinity);
        TurnManager.HandleTurns(_gameState.TurnPlayer, affinity);
        _view.DisplayHpMessage(affinity == AffinityType.Repel ? attacker : target);
    }

    protected abstract string GetActionMessage(Unit attacker, Unit target);
    protected abstract double GetBaseDamage(Unit attacker);
    protected abstract AffinityType GetAffinity(Unit target);
}