namespace Shin_Megami_Tensei.GameLoop.Actions;

public class PassTurnAction
{
    private readonly GameState _gameState;
    public PassTurnAction(GameState gameState) => _gameState = gameState;

    internal void ExecutePassTurn()
    {
        _gameState.TurnPlayer.TurnState.PassTurn();
        _gameState.TurnPlayer.TurnState.Report();
    }
}