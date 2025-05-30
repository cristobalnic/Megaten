using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.Enums;
using Shin_Megami_Tensei.GameData;

namespace Shin_Megami_Tensei.Affinities;

public static class AffinityHandlerFactory
{
    public static IAffinityHandler CreateAffinityHandler(AffinityType affinityType)
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

public class NeutralAffinityHandler : IAffinityHandler
{
    public double ApplyDamageModifier(double baseDamage)
    {
        throw new NotImplementedException();
    }

    public void UseTurns(TurnState turnState)
    {
        if (turnState.GetBlinkingTurns() > 0)
            turnState.UseBlinkingTurn();
        else
            turnState.UseFullTurn();
    }
}

public class WeakAffinityHandler : IAffinityHandler
{
    public double ApplyDamageModifier(double baseDamage)
    {
        throw new NotImplementedException();
    }

    public void UseTurns(TurnState turnState)
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

public class ResistAffinityHandler : IAffinityHandler
{
    public double ApplyDamageModifier(double baseDamage)
    {
        throw new NotImplementedException();
    }

    public void UseTurns(TurnState turnState)
    {
        if (turnState.GetBlinkingTurns() > 0)
            turnState.UseBlinkingTurn();
        else
            turnState.UseFullTurn();
    }
}

public class NullAffinityHandler : IAffinityHandler
{
    public double ApplyDamageModifier(double baseDamage)
    {
        throw new NotImplementedException();
    }

    public void UseTurns(TurnState turnState)
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

public class RepelAffinityHandler : IAffinityHandler
{
    public double ApplyDamageModifier(double baseDamage)
    {
        throw new NotImplementedException();
    }

    public void UseTurns(TurnState turnState)
    {
        turnState.UseFullTurn(turnState.GetFullTurns());
        turnState.UseBlinkingTurn(turnState.GetBlinkingTurns());
    }
}

public class DrainAffinityHandler : IAffinityHandler
{
    public double ApplyDamageModifier(double baseDamage)
    {
        throw new NotImplementedException();
    }

    public void UseTurns(TurnState turnState)
    {
        turnState.UseFullTurn(turnState.GetFullTurns());
        turnState.UseBlinkingTurn(turnState.GetBlinkingTurns());
    }
}