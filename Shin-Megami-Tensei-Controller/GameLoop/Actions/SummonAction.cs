using Shin_Megami_Tensei_View;
using Shin_Megami_Tensei.Entities;

namespace Shin_Megami_Tensei.GameLoop.Actions;

public class SummonAction
{
    private readonly View _view;
    private readonly GameState _gameState;
    private readonly SelectionUtils _selectionUtils;

    
    public SummonAction(View view, GameState gameState)
    {
        _view = view;
        _gameState = gameState;
        _selectionUtils = new SelectionUtils(view, gameState);
    }

    internal void ExecuteSummon(Unit summoner)
    {
        _view.WriteLine("Seleccione un monstruo para invocar");
        _selectionUtils.DisplayMonsterSelection(_gameState.TurnPlayer.Table.Reserve);
        Unit monsterSummon = _selectionUtils.GetPlayerObjective(_gameState.TurnPlayer.Table.Reserve);
        summoner.Summon(monsterSummon, _gameState.TurnPlayer.Table, _selectionUtils);
        _view.WriteLine(Params.Separator);
        _view.WriteLine($"{monsterSummon.Name} ha sido invocado");
        _gameState.TurnPlayer.TurnState.PassTurnOrSummonTurn();
    }
}