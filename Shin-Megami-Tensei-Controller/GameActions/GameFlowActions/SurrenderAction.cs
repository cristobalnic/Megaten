using Shin_Megami_Tensei.ErrorHandling;
using Shin_Megami_Tensei.GameData;
using Shin_Megami_Tensei.Views;

namespace Shin_Megami_Tensei.GameActions.GameFlowActions;

public class SurrenderAction
{
    private readonly IView _view;
    private readonly GameState _gameState;
    
    public SurrenderAction(IView view, GameState gameState)
    {
        _view = view;
        _gameState = gameState;
    }

    internal void ExecuteSurrender()
    {
        _view.WriteLine($"{_gameState.TurnPlayer.Samurai.Name} (J{_gameState.TurnPlayer.Id}) se rinde");
        _view.WriteLine(Params.Separator);
        _view.WriteLine($"Ganador: {_gameState.WaitPlayer.Samurai.Name} (J{_gameState.WaitPlayer.Id})");
        throw new EndGameException();
    }
}