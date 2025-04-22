using Shin_Megami_Tensei_View;
using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.Enums;

namespace Shin_Megami_Tensei.GameLoop.Actions;

public class AttackAction
{
    
    private readonly View _view;
    private readonly GameState _gameState;
    
    public AttackAction(View view, GameState gameState)
    {
        _view = view;
        _gameState = gameState;
    }

    internal void ExecuteAttack(Unit attacker)
    {
        _view.WriteLine($"Seleccione un objetivo para {attacker.Name}");
        ActionsUtils.DisplayMonsterSelection(_view, _gameState.WaitPlayer.Table.Monsters);
        Unit target = ActionsUtils.GetPlayerObjective(_view, _gameState.WaitPlayer.Table.Monsters);
        _view.WriteLine(Params.Separator);
        _view.WriteLine($"{attacker.Name} ataca a {target.Name}");
        double baseDamage = GetAttackDamage(attacker);
        DealDamage(attacker, target, baseDamage, target.Affinity.Phys);
    }

    private static double GetAttackDamage(Unit monster) =>
        monster.Stats.Str * Params.AttackDamageModifier * Params.AttackAndShootDamageMultiplier;
    
    private void DealDamage(Unit attacker, Unit target, double baseDamage, AffinityType affinityType)
    {
        var affinityHandler = new AffinityHandler(_view, _gameState.TurnPlayer.TurnState);
        affinityHandler.HandleAffinityEffect(attacker, target, baseDamage, affinityType);

        if (!target.IsAlive()) _gameState.WaitPlayer.Table.HandleDeath(target);
        if (!attacker.IsAlive()) _gameState.TurnPlayer.Table.HandleDeath(attacker);
    }
}