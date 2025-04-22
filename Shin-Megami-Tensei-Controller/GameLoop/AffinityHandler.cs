using Shin_Megami_Tensei_View;
using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.Enums;

namespace Shin_Megami_Tensei.GameLoop;

public class AffinityHandler
{
    private readonly View _view;
    private readonly TurnState _turnState;

    public AffinityHandler(View view, TurnState turnState)
    {
        _view = view;
        _turnState = turnState;
    }

    public void HandleAffinityEffect(Unit attacker, Unit target, double baseDamage, AffinityType affinityType)
    {
        if (affinityType == AffinityType.Neutral)
            HandleNeutral(target, baseDamage);
        else if (affinityType == AffinityType.Weak)
            HandleWeak(attacker, target, baseDamage);
        else if (affinityType == AffinityType.Resist)
            HandleResist(attacker, target, baseDamage);
        else if (affinityType == AffinityType.Null)
            HandleNull(attacker, target);
        else if (affinityType == AffinityType.Repel)
            HandleRepel(attacker, target, baseDamage);
        else if (affinityType == AffinityType.Drain)
            HandleDrain(target, baseDamage);
    }

    private void HandleNeutral(Unit target, double baseDamage)
    {
        int damage = GetRoundedIntDamage(baseDamage);
        ApplyDamage(target, damage);
        DisplayDamageMessage(target, damage);
        _turnState.UseTurnsForNeutralOrResist();
    }
    
    private void HandleWeak(Unit attacker, Unit target, double baseDamage)
    {
        double weakDamage = baseDamage * Params.WeakDamageMultiplier;
        int damage = GetRoundedIntDamage(weakDamage);
        _view.WriteLine($"{target.Name} es débil contra el ataque de {attacker.Name}");
        ApplyDamage(target, damage);
        DisplayDamageMessage(target, damage);
        _turnState.UseTurnsForWeak();
    }
    
    private void HandleResist(Unit attacker, Unit target, double baseDamage)
    {
        double resistDamage = baseDamage * Params.ResistDamageMultiplier;
        int damage = GetRoundedIntDamage(resistDamage);
        _view.WriteLine($"{target.Name} es resistente el ataque de {attacker.Name}");
        ApplyDamage(target, damage);
        DisplayDamageMessage(target, damage);
        _turnState.UseTurnsForNeutralOrResist();
    }

    private void HandleNull(Unit attacker, Unit target)
    {
        _view.WriteLine($"{target.Name} bloquea el ataque de {attacker.Name}");
        _turnState.UseTurnsForNull();
        DisplayHpMessage(target);
    }

    private void HandleRepel(Unit attacker, Unit target, double baseDamage)
    {
        int damage = GetRoundedIntDamage(baseDamage);
        ApplyDamage(attacker, damage);
        DisplayRepeledDamageMessage(target, damage, attacker);
        _turnState.UseTurnsForRepelOrDrain();   
    }

    private void HandleDrain(Unit target, double baseDamage)
    {
        int drainDamage = GetRoundedIntDamage(baseDamage);
        ApplyDrain(target, drainDamage);
        DisplayDrainDamageMessage(target, drainDamage);
        _turnState.UseTurnsForRepelOrDrain();
    }

    private static void ApplyDamage(Unit target, int damage) 
        => target.Stats.Hp = Math.Max(0, target.Stats.Hp - damage);

    private static void ApplyDrain(Unit target, int damage) 
        => target.Stats.Hp = Math.Min(target.Stats.MaxHp, target.Stats.Hp + damage);

    private void DisplayDamageMessage(Unit monster, int damage)
    {
        _view.WriteLine($"{monster.Name} recibe {damage} de daño");
        DisplayHpMessage(monster);
    }

    private void DisplayRepeledDamageMessage(Unit target, int repelDamage, Unit attacker)
    {
        _view.WriteLine($"{target.Name} devuelve {repelDamage} daño a {attacker.Name}");
        DisplayHpMessage(attacker);
    }
    
    private void DisplayDrainDamageMessage(Unit target, int drainDamage)
    {
        _view.WriteLine($"{target.Name} absorbe {drainDamage} daño");
        DisplayHpMessage(target);
    }

    private void DisplayHpMessage(Unit monster) 
        => _view.WriteLine($"{monster.Name} termina con HP:{monster.Stats.Hp}/{monster.Stats.MaxHp}");
    
    private static int GetRoundedIntDamage(double damage)
    {
        return Convert.ToInt32(Math.Floor(Math.Max(0, damage)));
    }
}



