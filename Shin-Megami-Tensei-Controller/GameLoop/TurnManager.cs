using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.ErrorHandling;
using Shin_Megami_Tensei.GameData;
using Shin_Megami_Tensei.Views;

namespace Shin_Megami_Tensei.GameLoop;

public class TurnManager
{
    private readonly IView _view;
    private readonly GameState _gameState;
    private readonly ActionManager _actionManager;
    
    
    public TurnManager(IView view, GameState gameState)
    {
        _view = view;
        _gameState = gameState;
        _actionManager = new ActionManager(view, gameState);
    }


    internal void PlayTurn(List<Unit> orderedMonsters)
    {
        DisplayPlayerAvailableTurns();
        DisplayPlayerMonstersOrderedBySpeed(orderedMonsters);
        TryToExecuteAction(orderedMonsters[0]);
        UpdateTurnState();
        DisplayWinnerIfExists();
    }
    
    private void DisplayPlayerAvailableTurns() 
        => _view.WriteLine(_gameState.TurnPlayer.TurnState.GetAvailableTurnsReport());

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
    
    private void TryToExecuteAction(Unit monster)
    {
        try
        {
            _actionManager.DisplayPlayerActionSelectionMenu(monster);
            _actionManager.PlayerActionExecution(monster);
        }
        catch (CancelObjectiveSelectionException)
        {
            TryToExecuteAction(monster);
        }
    }
    
    private void UpdateTurnState()
    {
        var turnState = _gameState.TurnPlayer.TurnState;
        _view.WriteLine(Params.Separator);
        _view.WriteLine(turnState.GetUsageReport());
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
        return player.Table.ActiveUnits.All(monster => monster.IsEmpty() || !monster.IsAlive());
    }

    private void DeclareWinner(Player winner)
    {
        _view.WriteLine(Params.Separator);
        _view.WriteLine($"Ganador: {winner.Samurai.Name} (J{winner.Id})");
        throw new EndGameException();
    }
}