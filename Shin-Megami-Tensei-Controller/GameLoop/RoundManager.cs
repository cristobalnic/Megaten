using Shin_Megami_Tensei_View;
using Shin_Megami_Tensei.Entities;

namespace Shin_Megami_Tensei.GameLoop;

public class RoundManager
{
    private readonly View _view;
    private readonly GameState _gameState;
    private readonly TurnManager _turnManager;

    public RoundManager(View view, GameState gameState)
    {
        _view = view;
        _gameState = gameState;
        _turnManager = new TurnManager(_view, _gameState);
    }

    public void PlayRound()
    {
        SetPlayersRoles();
        DisplayRoundInit();
        _gameState.TurnPlayer.TurnState.ResetRemainingTurns(_gameState.TurnPlayer.Table);
        var orderedMonsters = GetAliveMonstersOrderedBySpeed();
        while (_gameState.TurnPlayer.TurnState.FullTurns > 0 || _gameState.TurnPlayer.TurnState.BlinkingTurns > 0)
        {
            DisplayPlayersTables();
            _turnManager.PlayTurn(orderedMonsters);
            orderedMonsters = UpdateOrderedMonstersWithSummons(orderedMonsters, _gameState.TurnPlayer.Table.Monsters);
            orderedMonsters = UpdateOrderedMonstersWithDeaths(orderedMonsters);
            orderedMonsters = GetNewMonsterOrder(orderedMonsters);
        }
        _gameState.Round++;
    }
    
    private List<Unit> UpdateOrderedMonstersWithSummons(List<Unit> orderedMonsters, List<Unit> tableMonsters)
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
    
    private List<Unit> UpdateOrderedMonstersWithDeaths(List<Unit> orderedMonsters)
    {
        var updatedMonsters = new List<Unit>(orderedMonsters);
        foreach (var monster in orderedMonsters)
        {
            if (monster.IsAlive()) continue;
            updatedMonsters.Remove(monster);
        }
        return updatedMonsters;
    }

    private void SetPlayersRoles()
    {
        _gameState.TurnPlayer = _gameState.Players[_gameState.Round % 2];
        _gameState.WaitPlayer = _gameState.Players[(_gameState.Round + 1) % 2];
    }
    
    private void DisplayRoundInit()
    {
        _view.WriteLine(Params.Separator);
        _view.WriteLine($"Ronda de {_gameState.TurnPlayer.Samurai?.Name} (J{_gameState.TurnPlayer.Id})");
    }
    
    private List<Unit> GetAliveMonstersOrderedBySpeed()
    {
        var monsters = new List<Unit>();
        foreach (var monster in _gameState.TurnPlayer.Table.Monsters)
            if (!monster.IsEmpty() && monster.IsAlive()) monsters.Add(monster);
        return monsters.OrderByDescending(monster => monster.Stats.Spd).ToList();
    }
    
    private void DisplayPlayersTables()
    {
        _view.WriteLine(Params.Separator);
        foreach (var player in _gameState.Players) DisplayPlayerTable(player);
    }
    
    private void DisplayPlayerTable(Player player)
    {
        _view.WriteLine($"Equipo de {player.Table.Samurai?.Name} (J{player.Id})");
        char label = 'A';
        foreach (var monster in player.Table.Monsters)
        {
            if (monster.IsEmpty())
                _view.WriteLine($"{label}-");
            else
                _view.WriteLine($"{label}-{monster.Name} HP:{monster.Stats.Hp}/{monster.Stats.MaxHp} MP:{monster.Stats.Mp}/{monster.Stats.MaxMp}");
            label++;
        }
    }
    
    private static List<Unit> GetNewMonsterOrder(List<Unit> orderedMonsters)
    {
        var reorderedMonsters = new List<Unit>();
        reorderedMonsters.AddRange(orderedMonsters.GetRange(1, orderedMonsters.Count - 1));
        reorderedMonsters.Add(orderedMonsters[0]);
        return reorderedMonsters;
    }
}