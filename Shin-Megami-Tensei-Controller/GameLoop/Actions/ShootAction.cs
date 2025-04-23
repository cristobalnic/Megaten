using Shin_Megami_Tensei_View;
using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.Enums;

namespace Shin_Megami_Tensei.GameLoop.Actions;

public class ShootAction
{
    private readonly View _view;
    private readonly ActionsUtils _actionsUtils;
    
    public ShootAction(View view, GameState gameState)
    {
        _view = view;
        _actionsUtils = new ActionsUtils(view, gameState);
    }

    internal void ExecuteShoot(Unit attacker)
    {
        var target = _actionsUtils.GetTarget(attacker);
        _view.WriteLine(Params.Separator);
        HandleDamage(attacker, target);
    }

    private void HandleDamage(Unit attacker, Unit target)
    {
        _view.WriteLine($"{attacker.Name} dispara a {target.Name}");
        double baseDamage = GetShootDamage(attacker);
        _actionsUtils.DealDamage(attacker, target, baseDamage, target.Affinity.Gun);
    }

    private static double GetShootDamage(Unit monster) =>
        monster.Stats.Skl * Params.ShootDamageModifier * Params.AttackAndShootDamageMultiplier;
}