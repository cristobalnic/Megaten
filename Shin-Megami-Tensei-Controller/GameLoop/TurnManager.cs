using Shin_Megami_Tensei_View;
using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.MegatenErrorHandling;

namespace Shin_Megami_Tensei.GameLoop;

public class TurnManager
{
    private readonly View _view;

    public int FullTurnsUsed;
    public int BlinkingTurnsUsed;
    public int BlinkingTurnsObtained;

    private RoundManager _roundManager;
    private ActionManager _actionManager;
    
    
    public TurnManager(View view, RoundManager roundManager)
    {
        _view = view;
        _roundManager = roundManager;
        _actionManager = new ActionManager(view,this, roundManager);
    }


    internal void PlayTurn(List<Unit> orderedMonsters, Player turnPlayer, Player waitPlayer)
    {
        DisplayPlayerAvailableTurns(turnPlayer);
        DisplayPlayerMonstersOrderedBySpeed(orderedMonsters);
        TryToExecuteAction(orderedMonsters, turnPlayer, waitPlayer);
        UpdateTurnState(turnPlayer);
        DisplayWinnerIfExists(turnPlayer, waitPlayer);
    }
    
    private void DisplayPlayerAvailableTurns(Player turnPlayer)
    {
        _view.WriteLine(Params.Separator);
        _view.WriteLine($"Full Turns: {_roundManager.TurnPlayer.FullTurns}");
        _view.WriteLine($"Blinking Turns: {turnPlayer.BlinkingTurns}");
    }
    
    private void DisplayPlayerMonstersOrderedBySpeed(List<Unit> orderedMonsters)
    {
        _view.WriteLine(Params.Separator);
        _view.WriteLine("Orden:");
        var orderedMonstersNames = orderedMonsters.Select(monster => monster.Name).ToArray(); 
        DisplayItemList(orderedMonstersNames, '1', "-");
    }
    
    private void DisplayItemList(string[] items, char counterLabel, string separator)
    {
        foreach (var item in items)
        {
            _view.WriteLine($"{counterLabel}{separator}{item}");
            counterLabel++;
        }
    }
    
    private void TryToExecuteAction(List<Unit> orderedMonsters, Player turnPlayer, Player waitPlayer)
    {
        try
        {
            _actionManager.DisplayPlayerActionSelectionMenu(orderedMonsters[0]);
            _actionManager.PlayerActionExecution(orderedMonsters[0], turnPlayer, waitPlayer);
        }
        catch (CancelObjectiveSelectionException)
        {
            TryToExecuteAction(orderedMonsters, turnPlayer, waitPlayer);
        }
    }
    
    private void UpdateTurnState(Player turnPlayer)
    {
        _view.WriteLine(Params.Separator);
        turnPlayer.FullTurns -= FullTurnsUsed;
        turnPlayer.BlinkingTurns -= BlinkingTurnsUsed;
        turnPlayer.BlinkingTurns += BlinkingTurnsObtained;
        _view.WriteLine($"Se han consumido {FullTurnsUsed} Full Turn(s) y {BlinkingTurnsUsed} Blinking Turn(s)");
        _view.WriteLine($"Se han obtenido {BlinkingTurnsObtained} Blinking Turn(s)");
        FullTurnsUsed = 0;
        BlinkingTurnsUsed = 0;
        BlinkingTurnsObtained = 0;
    }
    
    private void DisplayWinnerIfExists(Player turnPlayer, Player waitPlayer)
    {
        if (waitPlayer.Table.Monsters.All(monster => monster == null || !monster.IsAlive()))
        {
            _view.WriteLine(Params.Separator);
            _view.WriteLine($"Ganador: {turnPlayer.Samurai?.Name} (J{turnPlayer.Id})");
            throw new EndGameException();
        }
        if (turnPlayer.Table.Monsters.All(monster => monster == null || !monster.IsAlive()))
        {
            _view.WriteLine(Params.Separator);
            _view.WriteLine($"Ganador: {waitPlayer.Samurai?.Name} (J{waitPlayer.Id})");
            throw new EndGameException();
        }
    }
}