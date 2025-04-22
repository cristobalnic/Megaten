using Shin_Megami_Tensei_View;
using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.MegatenErrorHandling;

namespace Shin_Megami_Tensei.GameLoop;

public class TurnManager
{
    private readonly View _view;
    private GameState _gameState;
    private ActionManager _actionManager;
    
    
    public TurnManager(View view, GameState gameState)
    {
        _view = view;
        _gameState = gameState;
        _actionManager = new ActionManager(view, gameState);
    }


    internal void PlayTurn(List<Unit> orderedMonsters)
    {
        DisplayPlayerAvailableTurns();
        DisplayPlayerMonstersOrderedBySpeed(orderedMonsters);
        TryToExecuteAction(orderedMonsters);
        UpdateTurnState();
        DisplayWinnerIfExists();
    }
    
    private void DisplayPlayerAvailableTurns()
    {
        _view.WriteLine(Params.Separator);
        _view.WriteLine($"Full Turns: {_gameState.TurnPlayer.TurnState.FullTurns}");
        _view.WriteLine($"Blinking Turns: {_gameState.TurnPlayer.TurnState.BlinkingTurns}");
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
    
    private void TryToExecuteAction(List<Unit> orderedMonsters)
    {
        try
        {
            _actionManager.DisplayPlayerActionSelectionMenu(orderedMonsters[0]);
            _actionManager.PlayerActionExecution(orderedMonsters[0]);
        }
        catch (CancelObjectiveSelectionException)
        {
            TryToExecuteAction(orderedMonsters);
        }
    }
    
    private void UpdateTurnState()
    {
        var turnState = _gameState.TurnPlayer.TurnState;
        _view.WriteLine(Params.Separator);
        _view.WriteLine(turnState.Report());
        turnState.ResetUsage();
    }
    
    private void DisplayWinnerIfExists()
    {
        var turnPlayer = _gameState.TurnPlayer;
        var waitPlayer = _gameState.WaitPlayer;

        if (HasLost(waitPlayer))
        {
            DeclareWinner(turnPlayer);
        }
        else if (HasLost(turnPlayer))
        {
            DeclareWinner(waitPlayer);
        }
    }

    private bool HasLost(Player player)
    {
        return player.Table.Monsters.All(monster => monster == null || !monster.IsAlive());
    }

    private void DeclareWinner(Player winner)
    {
        _view.WriteLine(Params.Separator);
        _view.WriteLine($"Ganador: {winner.Samurai?.Name} (J{winner.Id})");
        throw new EndGameException();
    }
}