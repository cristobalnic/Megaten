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

    internal void ExecuteAttack(Unit monster)
    {
        _view.WriteLine($"Seleccione un objetivo para {monster.Name}");
        ActionsUtils.DisplayTargetSelection(_view, _gameState.WaitPlayer);
        Unit defenderMonster = ActionsUtils.GetPlayerObjective(_view, _gameState.WaitPlayer);
        int damage = Convert.ToInt32(Math.Floor(Math.Max(0, GetAttackDamage(monster))));
        _view.WriteLine(Params.Separator);
        _view.WriteLine($"{monster.Name} ataca a {defenderMonster.Name}");
        DealDamage(defenderMonster, damage, defenderMonster.Affinity.Phys);
    }

    private static double GetAttackDamage(Unit monster) =>
        monster.Stats.Str * Params.AttackDamageModifier * Params.AttackAndShootDamageMultiplier;
    
    private void DealDamage(Unit monster, int damage, AffinityType affinityType)
    {
        // AffinityTypes available: Neutral, Weak, Resist, Null, Repel, Drain

        if (affinityType == AffinityType.Neutral)
        {
            monster.Stats.Hp = Math.Max(0, monster.Stats.Hp - damage);
            _view.WriteLine($"{monster.Name} recibe {damage} de daño");
            _gameState.TurnPlayer.TurnState.UseFullTurn();
        }
        else if (affinityType == AffinityType.Weak)
        {
            monster.Stats.Hp = Convert.ToInt32(Math.Floor(Math.Max(0, monster.Stats.Hp - damage * Params.WeakDamageMultiplier)));
        }
        

        _view.WriteLine($"{monster.Name} termina con HP:{monster.Stats.Hp}/{monster.Stats.MaxHp}");
        if (!monster.IsAlive()) _gameState.WaitPlayer.Table.HandleDeath(monster);
    }
}