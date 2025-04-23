using Shin_Megami_Tensei_View;
using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.Enums;

namespace Shin_Megami_Tensei.GameLoop.Actions;

public class ShootAction
{
    private readonly View _view;
    private readonly GameState _gameState;
    private readonly ActionsUtils _actionsUtils;
    
    public ShootAction(View view, GameState gameState)
    {
        _view = view;
        _gameState = gameState;
        _actionsUtils = new ActionsUtils(view);
    }

    internal void ExecuteShoot(Unit attacker)
    {
        _view.WriteLine($"Seleccione un objetivo para {attacker.Name}");
        _actionsUtils.DisplayMonsterSelection(_gameState.WaitPlayer.Table.Monsters);
        Unit target = _actionsUtils.GetPlayerObjective(_gameState.WaitPlayer.Table.Monsters);
        _view.WriteLine(Params.Separator);
        _view.WriteLine($"{attacker.Name} dispara a {target.Name}");
        int baseDamage = Convert.ToInt32(Math.Floor(Math.Max(0, GetShootDamage(attacker))));
        DealDamage(attacker, target, baseDamage, target.Affinity.Gun);
    }

    private static double GetShootDamage(Unit monster) =>
        monster.Stats.Skl * Params.ShootDamageModifier * Params.AttackAndShootDamageMultiplier;
    
    private void DealDamage(Unit attacker, Unit target, int baseDamage, AffinityType affinityType)
    {
        var affinityHandler = new AffinityHandler(_view, _gameState.TurnPlayer.TurnState);
        affinityHandler.HandleAffinityEffect(attacker, target, baseDamage, affinityType);

        if (!target.IsAlive()) _gameState.WaitPlayer.Table.HandleDeath(target);
        if (!attacker.IsAlive()) _gameState.TurnPlayer.Table.HandleDeath(attacker);
    }
}