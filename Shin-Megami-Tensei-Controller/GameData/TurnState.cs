namespace Shin_Megami_Tensei.GameData;

public class TurnState
{
    public int FullTurns { get; private set; }
    public int BlinkingTurns { get; private set; }

    private int _fullTurnsUsed;
    private int _blinkingTurnsUsed;
    private int _blinkingTurnsGained;
    
    public bool AreTurnsAvailable() => FullTurns > 0 || BlinkingTurns > 0;
    
    private void UseFullTurn(int amount = 1)
    {
        FullTurns -= amount;
        _fullTurnsUsed += amount;
    }

    private void UseBlinkingTurn(int amount = 1)
    {
        BlinkingTurns -= amount;
        _blinkingTurnsUsed += amount;
    }

    private void GainBlinkingTurn(int amount = 1)
    {
        BlinkingTurns += amount;
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
               $"Full Turns: {FullTurns}\n" +
               $"Blinking Turns: {BlinkingTurns}";
    }
    
    public string GetTurnUsageReport()
    {
        return $"Se han consumido {_fullTurnsUsed} Full Turn(s) y {_blinkingTurnsUsed} Blinking Turn(s)\n" +
               $"Se han obtenido {_blinkingTurnsGained} Blinking Turn(s)";
    }

    public void ResetRemainingTurns(Table table)
    {
        FullTurns = table.ActiveUnits.Count(monster => !monster.IsEmpty() && monster.IsAlive());
        BlinkingTurns = 0;
    }

    public void UseTurnsForNeutralOrResist()
    {
        if (BlinkingTurns > 0)
            UseBlinkingTurn();
        else
            UseFullTurn();
    }

    public void UseTurnsForWeak()
    {
        if (FullTurns > 0)
        {
            UseFullTurn();
            GainBlinkingTurn();
        }
        else
            UseBlinkingTurn();
    }

    public void UseTurnsForNull()
    {
        for (int i = 0; i < 2; i++)
        {
            if (BlinkingTurns > 0)
                UseBlinkingTurn();
            else if (FullTurns > 0)
                UseFullTurn();
        }
    }

    public void UseTurnsForRepelOrDrain()
    {
        UseFullTurn(FullTurns);
        UseBlinkingTurn(BlinkingTurns);
    }

    public void PassTurnOrSummonTurn()
    {
        if (BlinkingTurns > 0)
            UseBlinkingTurn();
        else
        {
            UseFullTurn();
            GainBlinkingTurn();
        }
    }

    public void UseTurnsForNonOffensiveSkill()
    {
        if (BlinkingTurns > 0)
            UseBlinkingTurn();
        else
            UseFullTurn();
    }
}
