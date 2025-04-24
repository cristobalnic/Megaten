using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.Views;

namespace Shin_Megami_Tensei.GameLoop;

public class RoundManager
{
    private readonly IView _view;
    private readonly GameState _gameState;
    private readonly TurnManager _turnManager;

    public RoundManager(IView view, GameState gameState)
    {
        _view = view;
        _gameState = gameState;
        _turnManager = new TurnManager(_view, _gameState);
    }

    public void PlayRound()
    {
        PrepareRound();
        ExecuteTurns();
        _gameState.Round++;
    }

    private void PrepareRound()
    {
        SetPlayersRoles();
        _view.DisplayRoundInit(_gameState.TurnPlayer);
        ResetPlayerTurnState();
    }
    private void SetPlayersRoles()
    {
        _gameState.TurnPlayer = _gameState.Players[_gameState.Round % 2];
        _gameState.WaitPlayer = _gameState.Players[(_gameState.Round + 1) % 2];
    }
    private void ResetPlayerTurnState()
    {
        _gameState.TurnPlayer.TurnState.ResetRemainingTurns(_gameState.TurnPlayer.Table);
    }

    private void ExecuteTurns()
    {
        var orderedMonsters = GetAliveMonstersOrderedBySpeed();
        while (HasRemainingTurns())
        {
            _view.DisplayPlayersTables(_gameState.Players);
            _turnManager.PlayTurn(orderedMonsters);
            orderedMonsters = UpdateUnitOrder(orderedMonsters);
        }
    }
    private List<Unit> GetAliveMonstersOrderedBySpeed()
    {
        var monsters = new List<Unit>();
        foreach (var monster in _gameState.TurnPlayer.Table.Monsters)
            if (!monster.IsEmpty() && monster.IsAlive()) monsters.Add(monster);
        return monsters.OrderByDescending(monster => monster.Stats.Spd).ToList();
    }
    private bool HasRemainingTurns()
    {
        var turnState = _gameState.TurnPlayer.TurnState;
        return turnState.FullTurns > 0 || turnState.BlinkingTurns > 0;
    }
    
    private List<Unit> UpdateUnitOrder(List<Unit> currentOrder)
    {
        currentOrder = UpdateOrderedUnitsWithSummons(currentOrder, _gameState.TurnPlayer.Table.Monsters);
        currentOrder = UpdateOrderedUnitsWithDeaths(currentOrder);
        return GetNewUnitOrder(currentOrder);
    }

    private static List<Unit> GetNewUnitOrder(List<Unit> orderedMonsters)
    {
        var reorderedMonsters = new List<Unit>();
        reorderedMonsters.AddRange(orderedMonsters.GetRange(1, orderedMonsters.Count - 1));
        reorderedMonsters.Add(orderedMonsters[0]);
        return reorderedMonsters;
    }
    private List<Unit> UpdateOrderedUnitsWithSummons(List<Unit> orderedMonsters, List<Unit> tableMonsters)
    {
        var updatedMonsters = new List<Unit>(orderedMonsters);
        foreach (var tableMonster in tableMonsters)
        {
            if (tableMonster.IsEmpty() || orderedMonsters.Contains(tableMonster)) continue;
            bool replaced = false;
            for (var i = 0; i < updatedMonsters.Count; i++)
            {
                if (tableMonsters.Contains(updatedMonsters[i])) continue;
                updatedMonsters[i] = tableMonster;
                replaced = true;
                break;
            }
            if (!replaced) updatedMonsters.Add(tableMonster);
        }
        return updatedMonsters;
    }
    private List<Unit> UpdateOrderedUnitsWithDeaths(List<Unit> orderedMonsters)
    {
        var updatedMonsters = new List<Unit>(orderedMonsters);
        foreach (var monster in orderedMonsters)
        {
            if (monster.IsAlive()) continue;
            updatedMonsters.Remove(monster);
        }
        return updatedMonsters;
    }
}