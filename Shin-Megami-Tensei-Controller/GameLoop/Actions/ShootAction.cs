using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.Enums;
using Shin_Megami_Tensei.Views;

namespace Shin_Megami_Tensei.GameLoop.Actions;

public class ShootAction
{
    private readonly IView _view;
    private readonly GameState _gameState;
    private readonly SelectionUtils _selectionUtils;
    
    public ShootAction(IView view, GameState gameState)
    {
        _view = view;
        _gameState = gameState;
        _selectionUtils = new SelectionUtils(view, gameState);
    }

    internal void ExecuteShoot(Unit attacker)
    {
        var target = _selectionUtils.GetTarget(attacker);
        _view.WriteLine(Params.Separator);
        HandleDamage(attacker, target);
    }

    private void HandleDamage(Unit attacker, Unit target)
    {
        _view.WriteLine($"{attacker.Name} dispara a {target.Name}");
        double baseDamage = GetShootDamage(attacker);
        AffinityType targetAffinity = target.Affinity.Gun;
        _selectionUtils.DealDamage(attacker, target, baseDamage, target.Affinity.Gun);
        TurnManager.HandleTurns(_gameState.TurnPlayer, targetAffinity);
        _view.DisplayHpMessage(targetAffinity == AffinityType.Repel ? attacker : target);
    }

    private static double GetShootDamage(Unit attacker) =>
        attacker.Stats.Skl * Params.ShootDamageModifier * Params.AttackAndShootDamageMultiplier;
}