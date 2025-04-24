using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.Enums;
using Shin_Megami_Tensei.Views;

namespace Shin_Megami_Tensei.GameLoop;

public class AffinityHandler
{
    private readonly IView _view;

    public AffinityHandler(IView view)
    {
        _view = view;
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
        int damage = ActionUtils.GetRoundedIntDamage(baseDamage);
        ActionUtils.ApplyDamage(target, damage);
        _view.DisplayDamageMessage(target, damage);
    }
    
    private void HandleWeak(Unit attacker, Unit target, double baseDamage)
    {
        double weakDamage = baseDamage * Params.WeakDamageMultiplier;
        int damage = ActionUtils.GetRoundedIntDamage(weakDamage);
        _view.WriteLine($"{target.Name} es débil contra el ataque de {attacker.Name}");
        ActionUtils.ApplyDamage(target, damage);
        _view.DisplayDamageMessage(target, damage);
    }
    
    private void HandleResist(Unit attacker, Unit target, double baseDamage)
    {
        double resistDamage = baseDamage * Params.ResistDamageMultiplier;
        int damage = ActionUtils.GetRoundedIntDamage(resistDamage);
        _view.WriteLine($"{target.Name} es resistente el ataque de {attacker.Name}");
        ActionUtils.ApplyDamage(target, damage);
        _view.DisplayDamageMessage(target, damage);
    }

    private void HandleNull(Unit attacker, Unit target)
    {
        _view.WriteLine($"{target.Name} bloquea el ataque de {attacker.Name}");
    }

    private void HandleRepel(Unit attacker, Unit target, double baseDamage)
    {
        int damage = ActionUtils.GetRoundedIntDamage(baseDamage);
        ActionUtils.ApplyDamage(attacker, damage);
        _view.DisplayRepeledDamageMessage(target, damage, attacker);
    }

    private void HandleDrain(Unit target, double baseDamage)
    {
        int drainDamage = ActionUtils.GetRoundedIntDamage(baseDamage);
        ActionUtils.ApplyDrain(target, drainDamage);
        _view.DisplayDrainDamageMessage(target, drainDamage);
    }

    

    

    
    
    
}



