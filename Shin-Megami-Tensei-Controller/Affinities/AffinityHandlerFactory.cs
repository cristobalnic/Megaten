using Shin_Megami_Tensei.DataStructures;
using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.Enums;
using Shin_Megami_Tensei.GameActions.AttackActions;
using Shin_Megami_Tensei.GameData;

namespace Shin_Megami_Tensei.Affinities;

public static class AffinityHandlerFactory
{
    public static AffinityHandler CreateAffinityHandler(AffinityType affinityType)
    {
        return affinityType switch
        {
            AffinityType.Neutral => new NeutralAffinityHandler(),
            AffinityType.Weak => new WeakAffinityHandler(),
            AffinityType.Resist => new ResistAffinityHandler(),
            AffinityType.Null => new NullAffinityHandler(),
            AffinityType.Repel => new RepelAffinityHandler(),
            AffinityType.Drain => new DrainAffinityHandler(),
            _ => throw new ArgumentOutOfRangeException(nameof(affinityType), affinityType, null)
        };
    }
}

public class NeutralAffinityHandler : AffinityHandler
{
    public override double ApplyDamageModifier(double baseDamage)
    {
        throw new NotImplementedException();
    }

    public override void UseTurns(TurnState turnState)
    {
        if (turnState.GetBlinkingTurns() > 0)
            turnState.UseBlinkingTurn();
        else
            turnState.UseFullTurn();
    }
}

public class WeakAffinityHandler : AffinityHandler
{
    public override double ApplyDamageModifier(double baseDamage)
    {
        throw new NotImplementedException();
    }

    public override void UseTurns(TurnState turnState)
    {
        if (turnState.GetFullTurns() > 0)
        {
            turnState.UseFullTurn();
            turnState.GainBlinkingTurn();
        }
        else
            turnState.UseBlinkingTurn();
    }
}

public class ResistAffinityHandler : AffinityHandler
{
    public override double ApplyDamageModifier(double baseDamage)
    {
        throw new NotImplementedException();
    }

    public override void UseTurns(TurnState turnState)
    {
        if (turnState.GetBlinkingTurns() > 0)
            turnState.UseBlinkingTurn();
        else
            turnState.UseFullTurn();
    }
}

public class NullAffinityHandler : AffinityHandler
{
    public override double ApplyDamageModifier(double baseDamage)
    {
        throw new NotImplementedException();
    }

    public override void UseTurns(TurnState turnState)
    {
        for (int i = 0; i < 2; i++)
        {
            if (turnState.GetBlinkingTurns() > 0)
                turnState.UseBlinkingTurn();
            else if (turnState.GetFullTurns() > 0)
                turnState.UseFullTurn();
        }
    }
}

public class RepelAffinityHandler : AffinityHandler
{
    public override double ApplyDamageModifier(double baseDamage)
    {
        throw new NotImplementedException();
    }

    public override void UseTurns(TurnState turnState)
    {
        turnState.UseFullTurn(turnState.GetFullTurns());
        turnState.UseBlinkingTurn(turnState.GetBlinkingTurns());
    }
    
    public override Unit GetDamagedUnit(CombatRecord combatRecord) => combatRecord.Attacker;

    public override void DealDamageByAffinityRules(CombatRecord combatRecord)
    {
        AttackUtils.ApplyDamage(combatRecord.Attacker, combatRecord.Damage);
    }
}

public class DrainAffinityHandler : AffinityHandler
{
    public override double ApplyDamageModifier(double baseDamage)
    {
        throw new NotImplementedException();
    }

    public override void UseTurns(TurnState turnState)
    {
        turnState.UseFullTurn(turnState.GetFullTurns());
        turnState.UseBlinkingTurn(turnState.GetBlinkingTurns());
    }

    public override void DealDamageByAffinityRules(CombatRecord combatRecord)
    {
        AttackUtils.ApplyDrain(combatRecord.Target, combatRecord.Damage);
    }
}
