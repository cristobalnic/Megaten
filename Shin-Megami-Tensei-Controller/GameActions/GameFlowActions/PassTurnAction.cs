using Shin_Megami_Tensei.GameData;

namespace Shin_Megami_Tensei.GameActions.GameFlowActions;

public class PassTurnAction
{
    private readonly GameState _gameState;
    public PassTurnAction(GameState gameState) => _gameState = gameState;

    internal void Execute()
    {
        _gameState.TurnPlayer.TurnState.UseTurnsForPassOrSummon();
    }
}