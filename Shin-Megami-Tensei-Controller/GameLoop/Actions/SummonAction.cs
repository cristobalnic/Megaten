using Shin_Megami_Tensei_View;
using Shin_Megami_Tensei.Entities;

namespace Shin_Megami_Tensei.GameLoop.Actions;

public class SummonAction
{
    private readonly View _view;
    private readonly GameState _gameState;
    
    public SummonAction(View view, GameState gameState)
    {
        _view = view;
        _gameState = gameState;
    }

    internal void ExecuteSummon(Unit summoner)
    {
        _view.WriteLine("Seleccione un monstruo para invocar");
        ActionsUtils.DisplayMonsterSelection(_view, _gameState.TurnPlayer.Table.Reserve);
        Unit monsterSummon = ActionsUtils.GetPlayerObjective(_view, _gameState.TurnPlayer.Table.Reserve);
        summoner.Summon(summoner, monsterSummon, _gameState.TurnPlayer.Table);
    }
}