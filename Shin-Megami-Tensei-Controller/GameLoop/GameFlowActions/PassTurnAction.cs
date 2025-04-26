namespace Shin_Megami_Tensei.GameLoop.GameFlowActions;

public class PassTurnAction
{
    private readonly GameState _gameState;
    public PassTurnAction(GameState gameState) => _gameState = gameState;

    internal void ExecutePassTurn()
    {
        _gameState.TurnPlayer.TurnState.PassTurnOrSummonTurn();
    }
}