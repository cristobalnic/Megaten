using Shin_Megami_Tensei_View;
using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.Enums;

namespace Shin_Megami_Tensei.GameLoop.Actions;

public class AttackAction
{
    private readonly View _view;
    private readonly ActionsUtils _actionsUtils;
    
    public AttackAction(View view, GameState gameState)
    {
        _view = view;
        _actionsUtils = new ActionsUtils(view, gameState);
    }

    internal void ExecuteAttack(Unit attacker)
    {
        var target = _actionsUtils.GetTarget(attacker);
        _view.WriteLine(Params.Separator);
        HandleDamage(attacker, target);
    }
    
    private void HandleDamage(Unit attacker, Unit target)
    {
        _view.WriteLine($"{attacker.Name} ataca a {target.Name}");
        double baseDamage = GetAttackDamage(attacker);
        _actionsUtils.DealDamage(attacker, target, baseDamage, target.Affinity.Phys);
    }

    private static double GetAttackDamage(Unit monster) =>
        monster.Stats.Str * Params.AttackDamageModifier * Params.AttackAndShootDamageMultiplier;
}