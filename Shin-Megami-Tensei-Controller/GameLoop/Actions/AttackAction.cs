using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.Views;

namespace Shin_Megami_Tensei.GameLoop.Actions;

public class AttackAction
{
    private readonly IView _view;
    private readonly SelectionUtils _selectionUtils;
    
    public AttackAction(IView view, GameState gameState)
    {
        _view = view;
        _selectionUtils = new SelectionUtils(view, gameState);
    }

    internal void ExecuteAttack(Unit attacker)
    {
        var target = _selectionUtils.GetTarget(attacker);
        _view.WriteLine(Params.Separator);
        HandleDamage(attacker, target);
    }
    
    private void HandleDamage(Unit attacker, Unit target)
    {
        _view.WriteLine($"{attacker.Name} ataca a {target.Name}");
        double baseDamage = GetAttackDamage(attacker);
        _selectionUtils.DealDamage(attacker, target, baseDamage, target.Affinity.Phys);
    }

    private static double GetAttackDamage(Unit attacker) =>
        attacker.Stats.Str * Params.AttackDamageModifier * Params.AttackAndShootDamageMultiplier;
}