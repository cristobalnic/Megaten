using Shin_Megami_Tensei.Enums;

namespace Shin_Megami_Tensei.GameData;

public class TurnState
{
    private int _fullTurns;
    private int _blinkingTurns;
    private int _fullTurnsUsed;
    private int _blinkingTurnsUsed;
    private int _blinkingTurnsGained;
    
    public bool AreTurnsAvailable() => _fullTurns > 0 || _blinkingTurns > 0;
    
    private void UseFullTurn(int amount = 1)
    {
        _fullTurns -= amount;
        _fullTurnsUsed += amount;
    }

    private void UseBlinkingTurn(int amount = 1)
    {
        _blinkingTurns -= amount;
        _blinkingTurnsUsed += amount;
    }

    private void GainBlinkingTurn(int amount = 1)
    {
        _blinkingTurns += amount;
        _blinkingTurnsGained += amount;
    }

    public void ResetUsage()
    {
        _fullTurnsUsed = 0;
        _blinkingTurnsUsed = 0;
        _blinkingTurnsGained = 0;
    }

    public string GetAvailableTurnsReport()
    {
        return $"{Params.Separator}\n" +
               $"Full Turns: {_fullTurns}\n" +
               $"Blinking Turns: {_blinkingTurns}";
    }
    
    public string GetUsageReport()
    {
        return $"Se han consumido {_fullTurnsUsed} Full Turn(s) y {_blinkingTurnsUsed} Blinking Turn(s)\n" +
               $"Se han obtenido {_blinkingTurnsGained} Blinking Turn(s)";
    }

    public void ResetRemainingTurns(Table table)
    {
        _fullTurns = table.ActiveUnits.Count(monster => !monster.IsEmpty() && monster.IsAlive());
        _blinkingTurns = 0;
    }

    private void UseTurnsForNeutralOrResist()
    {
        if (_blinkingTurns > 0)
            UseBlinkingTurn();
        else
            UseFullTurn();
    }

    private void UseTurnsForWeak()
    {
        if (_fullTurns > 0)
        {
            UseFullTurn();
            GainBlinkingTurn();
        }
        else
            UseBlinkingTurn();
    }

    private void UseTurnsForNull()
    {
        for (int i = 0; i < 2; i++)
        {
            if (_blinkingTurns > 0)
                UseBlinkingTurn();
            else if (_fullTurns > 0)
                UseFullTurn();
        }
    }

    private void UseTurnsForRepelOrDrain()
    {
        UseFullTurn(_fullTurns);
        UseBlinkingTurn(_blinkingTurns);
    }

    public void UseTurnsForPassOrSummon()
    {
        if (_blinkingTurns > 0)
            UseBlinkingTurn();
        else
        {
            UseFullTurn();
            GainBlinkingTurn();
        }
    }

    public void UseTurnsForNonOffensiveSkill()
    {
        if (_blinkingTurns > 0)
            UseBlinkingTurn();
        else
            UseFullTurn();
    }

    public void UseTurnsByTargetAffinity(AffinityType targetAffinity)
    {
        if (targetAffinity is AffinityType.Neutral or AffinityType.Resist)
            UseTurnsForNeutralOrResist();
        else if (targetAffinity is AffinityType.Weak)
            UseTurnsForWeak();
        else if (targetAffinity is AffinityType.Null)
            UseTurnsForNull();
        else if (targetAffinity is AffinityType.Repel or AffinityType.Drain)
            UseTurnsForRepelOrDrain();
    }
}
