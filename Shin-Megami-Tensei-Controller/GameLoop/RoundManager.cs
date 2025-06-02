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
            if (ShouldSkipUnit(unit, orderedUnits)) continue;
            TryReplaceOrAddUnit(unit, updatedMonsters, activeUnits);
        }

        return updatedMonsters;
    }

    private bool ShouldSkipUnit(Unit unit, List<Unit> orderedUnits)
    {
        return unit.IsEmpty() || !unit.IsAlive() || orderedUnits.Contains(unit);
    }

    private void TryReplaceOrAddUnit(Unit unit, List<Unit> updatedMonsters, List<Unit> activeUnits)
    {
        int index = FindReplaceableIndex(updatedMonsters, activeUnits);
        if (index >= 0)
        {
            updatedMonsters[index] = unit;
            return;
        }

        updatedMonsters.Add(unit);
    }

    private int FindReplaceableIndex(List<Unit> updatedMonsters, List<Unit> activeUnits)
    {
        return updatedMonsters.FindIndex(existing => !activeUnits.Contains(existing));
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