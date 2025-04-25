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
        _view.DisplayAffinityDetectionMessage(attacker, target, affinityType);
        var finalDamage = GetDamageModifiedByAffinity(baseDamage, affinityType);
        var damage = ActionUtils.GetRoundedIntDamage(finalDamage);
        ActuallyDealDamage(attacker, damage, target, affinityType);
        _view.DisplayAttackResultMessage(attacker, damage, target, affinityType);
    }

    private static double GetDamageModifiedByAffinity(double baseDamage, AffinityType affinityType)
    {
        if (affinityType == AffinityType.Weak)
            baseDamage *= Params.WeakDamageMultiplier;
        else if (affinityType == AffinityType.Resist)
            baseDamage *= Params.ResistDamageMultiplier;
        else if (affinityType == AffinityType.Null)
            baseDamage = 0;
        return baseDamage;
    }


    private static void ActuallyDealDamage(Unit attacker, int damage, Unit target, AffinityType affinityType)
    {
        if (affinityType == AffinityType.Repel)
            ActionUtils.ApplyDamage(attacker, damage);
        
        else if (affinityType == AffinityType.Drain)
            ActionUtils.ApplyDrain(target, damage);
        else
            ActionUtils.ApplyDamage(target, damage);
    }
}



