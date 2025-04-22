namespace Shin_Megami_Tensei;

public class TurnState
{
    public int FullTurns { get; private set; }
    public int BlinkingTurns { get; private set; }

    private int _fullTurnsUsed;
    private int _blinkingTurnsUsed;
    private int _blinkingTurnsGained;

    public void UseFullTurn(int amount = 1)
    {
        FullTurns -= amount;
        _fullTurnsUsed += amount;
    }

    public void UseBlinkingTurn(int amount = 1)
    {
        BlinkingTurns -= amount;
        _blinkingTurnsUsed += amount;
    }

    public void GainBlinkingTurn(int amount = 1)
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

    public string Report()
    {
        return $"Se han consumido {_fullTurnsUsed} Full Turn(s) y {_blinkingTurnsUsed} Blinking Turn(s)\n" +
               $"Se han obtenido {_blinkingTurnsGained} Blinking Turn(s)";
    }

    public void ResetRemainingTurns(Table table)
    {
        FullTurns = table.Monsters.Count(monster => monster != null && monster.IsAlive());
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
        if (BlinkingTurns >= 2)
            UseBlinkingTurn(2);
        else if (BlinkingTurns == 1)
        {
            UseBlinkingTurn();
            UseFullTurn();
        }
        else
            UseFullTurn(2);
    }

    public void UseTurnsForRepelOrDrain()
    {
        UseFullTurn(FullTurns);
        UseBlinkingTurn(BlinkingTurns);
    }
    
    public void UseTurnsForMiss()
    {
        if (BlinkingTurns > 0)
            UseBlinkingTurn();
        else
            UseFullTurn();
    }

    public void PassTurn()
    {
        if (BlinkingTurns > 0)
            UseBlinkingTurn();
        else
        {
            UseFullTurn();
            GainBlinkingTurn();
        }
    }
}
