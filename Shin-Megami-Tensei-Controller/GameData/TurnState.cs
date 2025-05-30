using Shin_Megami_Tensei.Affinities;
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

    public void UseFullTurn(int amount = 1)
    {
        _fullTurns -= amount;
        _fullTurnsUsed += amount;
    }

    public void UseBlinkingTurn(int amount = 1)
    {
        _blinkingTurns -= amount;
        _blinkingTurnsUsed += amount;
    }

    public void GainBlinkingTurn(int amount = 1)
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
        AffinityHandler affinityHandler = AffinityHandlerFactory.CreateAffinityHandler(targetAffinity);
        affinityHandler.UseTurns(this);
    }

    public int GetBlinkingTurns()
    {
        return _blinkingTurns;
    }

    public int GetFullTurns()
    {
        return _fullTurns;
    }
}
