using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.Views;

namespace Shin_Megami_Tensei.GameLoop.Actions;

public class SummonAction
{
    private readonly IView _view;
    private readonly GameState _gameState;
    private readonly SelectionUtils _selectionUtils;

    
    public SummonAction(IView view, GameState gameState)
    {
        _view = view;
        _gameState = gameState;
        _selectionUtils = new SelectionUtils(view, gameState);
    }

    internal void ExecuteSummon(Unit summoner)
    {
        var selectionPhrase = "Seleccione un monstruo para invocar";
        _view.DisplayMonsterSelection(_gameState.TurnPlayer.Table.Reserve, selectionPhrase);
        Unit monsterSummon = _selectionUtils.GetTargetMonster(_gameState.TurnPlayer.Table.Reserve);
        summoner.Summon(monsterSummon, _gameState.TurnPlayer.Table, _selectionUtils);
        _view.WriteLine(Params.Separator);
        _view.WriteLine($"{monsterSummon.Name} ha sido invocado");
        _gameState.TurnPlayer.TurnState.PassTurnOrSummonTurn();
    }
}