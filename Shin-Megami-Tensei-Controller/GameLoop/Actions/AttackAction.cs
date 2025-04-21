using Shin_Megami_Tensei_View;
using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.Enums;

namespace Shin_Megami_Tensei.GameLoop.Actions;

public class AttackAction
{
    
    private readonly View _view;
    private readonly TurnManager _turnManager;
    
    public AttackAction(View view, TurnManager turnManager)
    {
        _view = view;
        _turnManager = turnManager;
    }

    internal void ExecuteAttack(Unit monster, Player waitPlayer)
    {
        _view.WriteLine($"Seleccione un objetivo para {monster.Name}");
        ActionsUtils.DisplayTargetSelection(_view, waitPlayer);
        Unit defenderMonster = ActionsUtils.GetPlayerObjective(_view, waitPlayer);
        int damage = Convert.ToInt32(Math.Floor(Math.Max(0, GetAttackDamage(monster))));
        _view.WriteLine(Params.Separator);
        _view.WriteLine($"{monster.Name} ataca a {defenderMonster.Name}");
        DealDamage(defenderMonster, damage, defenderMonster.Affinity.Phys, waitPlayer);
    }

    private static double GetAttackDamage(Unit monster) =>
        monster.Stats.Str * Params.AttackDamageModifier * Params.AttackAndShootDamageMultiplier;
    
    private void DealDamage(Unit monster, int damage, AffinityType affinityType, Player waitPlayer)
    {
        // AffinityTypes available: Neutral, Weak, Resist, Null, Repel, Drain

        if (affinityType == AffinityType.Neutral)
        {
            monster.Stats.Hp = Math.Max(0, monster.Stats.Hp - damage);
            _view.WriteLine($"{monster.Name} recibe {damage} de daño");
            _turnManager.FullTurnsUsed += 1;
        }
        else if (affinityType == AffinityType.Weak)
        {
            monster.Stats.Hp = Convert.ToInt32(Math.Floor(Math.Max(0, monster.Stats.Hp - damage * Params.WeakDamageMultiplier)));
        }
        

        _view.WriteLine($"{monster.Name} termina con HP:{monster.Stats.Hp}/{monster.Stats.MaxHp}");
        if (!monster.IsAlive()) waitPlayer.Table.HandleDeath(monster);
    }
}