using Shin_Megami_Tensei_View;
using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.MegatenErrorHandling;

namespace Shin_Megami_Tensei.GameLoop.Actions;

public class SurrenderAction
{
    private readonly View _view;
    private readonly GameState _gameState;
    
    public SurrenderAction(View view, GameState gameState)
    {
        _view = view;
        _gameState = gameState;
    }

    internal void ExecuteSurrender()
    {
        _view.WriteLine($"{_gameState.TurnPlayer.Samurai?.Name} (J{_gameState.TurnPlayer.Id}) se rinde");
        _view.WriteLine(Params.Separator);
        _view.WriteLine($"Ganador: {_gameState.WaitPlayer.Samurai?.Name} (J{_gameState.WaitPlayer.Id})");
        throw new EndGameException();
    }
}