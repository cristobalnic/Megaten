using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.GameData;
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
        var orderedUnits = GetAliveMonstersOrderedBySpeed();
        while (_gameState.TurnPlayer.HasRemainingTurns())
        {
            _view.DisplayPlayersTables(_gameState.Players);
            _turnManager.PlayTurn(orderedUnits);
            orderedUnits = UpdateUnitOrder(orderedUnits);
        }
    }
    private List<Unit> GetAliveMonstersOrderedBySpeed()
    {
        var monsters = new List<Unit>();
        foreach (var monster in _gameState.TurnPlayer.Table.ActiveUnits)
            if (!monster.IsEmpty() && monster.IsAlive()) monsters.Add(monster);
        return monsters.OrderByDescending(monster => monster.Stats.Spd).ToList();
    }
    
    private List<Unit> UpdateUnitOrder(List<Unit> currentOrder)
    {
        currentOrder = UpdateOrderedUnitsWithSummons(currentOrder, _gameState.TurnPlayer.Table.ActiveUnits);
        currentOrder = UpdateOrderedUnitsWithDeaths(currentOrder);
        return GetNewUnitOrder(currentOrder);
    }

    private static List<Unit> GetNewUnitOrder(List<Unit> orderedUnits)
    {
        var reorderedUnits = new List<Unit>();
        reorderedUnits.AddRange(orderedUnits.GetRange(1, orderedUnits.Count - 1));
        reorderedUnits.Add(orderedUnits[0]);
        return reorderedUnits;
    }
    private List<Unit> UpdateOrderedUnitsWithSummons(List<Unit> orderedUnits, List<Unit> activeUnits)
    {
        var updatedMonsters = new List<Unit>(orderedUnits);
        foreach (var unit in activeUnits)
        {
            if (unit.IsEmpty() || !unit.IsAlive()) continue; // Skip empty slots
            if (orderedUnits.Contains(unit)) continue; // Already in the order
            bool replaced = false;
            for (var i = 0; i < updatedMonsters.Count; i++)
            {
                if (!activeUnits.Contains(updatedMonsters[i])) // Already in the order
                {
                    updatedMonsters[i] = unit;
                    replaced = true;
                    break;
                }
            }
            if (!replaced) updatedMonsters.Add(unit);
        }
        return updatedMonsters;
    }
    private List<Unit> UpdateOrderedUnitsWithDeaths(List<Unit> orderedUnits)
    {
        var updatedMonsters = new List<Unit>(orderedUnits);
        foreach (var monster in orderedUnits)
        {
            if (monster.IsAlive()) continue;
            updatedMonsters.Remove(monster);
        }
        return updatedMonsters;
    }
}