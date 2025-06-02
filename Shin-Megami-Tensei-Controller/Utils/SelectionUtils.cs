using System.Reflection.Metadata;
using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.ErrorHandling;
using Shin_Megami_Tensei.GameData;
using Shin_Megami_Tensei.Views;

namespace Shin_Megami_Tensei.Utils;

public class SelectionUtils
{
    private readonly IView _view;
    private readonly GameState _gameState;
    public SelectionUtils(IView view, GameState gameState)
    {
        _view = view;
        _gameState = gameState;
    }
    
    public void DisplaySummonWithdrawSelection(List<Unit> monsters)
    {
        _view.WriteLine(Params.Separator);
        _view.WriteLine("Seleccione una posición para invocar");
        int label = 1;
        foreach (var monster in monsters)
        {
            if (monster is Samurai) continue;
            _view.WriteLine(monster.IsEmpty()
                ? $"{label}-{monster.Name} (Puesto {label + 1})"
                : $"{label}-{monster.Name} HP:{monster.Stats.Hp}/{monster.Stats.MaxHp} MP:{monster.Stats.Mp}/{monster.Stats.MaxMp} (Puesto {label + 1})");
            label++;
        }
        _view.WriteLine($"{label}-Cancelar");
    }
    
    public Unit GetTargetMonster(List<Unit> monsters)
    {
        var objectiveSelection = int.Parse(_view.ReadLine());
        List<Unit> validMonsters = new List<Unit>();
        foreach (var monster in monsters)
        {
            if (monster.IsEmpty() || !monster.IsAlive()) continue;
            validMonsters.Add(monster);
        }
        HandleCancelSelection(objectiveSelection, validMonsters);
        return validMonsters[objectiveSelection-1];
    }
    
    public Unit GetDeadTargetMonster(List<Unit> monsters)
    {
        var objectiveSelection = int.Parse(_view.ReadLine());
        List<Unit> validMonsters = new List<Unit>();
        foreach (var monster in monsters)
        {
            if (monster.IsAlive()) continue;
            validMonsters.Add(monster);
        }
        HandleCancelSelection(objectiveSelection, validMonsters);
        return validMonsters[objectiveSelection-1];
    }

    public Unit GetAnyReserveTargetMonster(List<Unit> reserveMonsters)
    {
        var objectiveSelection = int.Parse(_view.ReadLine());
        HandleCancelSelection(objectiveSelection, reserveMonsters);
        return reserveMonsters[objectiveSelection-1];
    }
    
    private static void HandleCancelSelection(int objectiveSelection, List<Unit> monsters)
    {
        if (objectiveSelection > monsters.Count)
        {
            throw new CancelObjectiveSelectionException();
        }
    }
    
    public Unit GetSummonWithdrawSelection(List<Unit> monsters)
    {
        var summonSelection = int.Parse(_view.ReadLine());
        List<Unit> validMonsters = new List<Unit>();
        foreach (var monster in monsters)
        {
            if (monster is Samurai) continue;
            validMonsters.Add(monster);
        }
        if (summonSelection > validMonsters.Count)
        {
            throw new CancelObjectiveSelectionException();
        }
        return validMonsters[summonSelection-1];
    }

    public Unit GetTarget(Unit attacker)
    {
        var player = _gameState.WaitPlayer;
        var selectionPhrase = $"Seleccione un objetivo para {attacker.Name}";
        var possibleTargets = FilterAliveAndNotEmptyMonsters(player.Table.ActiveUnits);
        _view.DisplayMonsterSelection(possibleTargets, selectionPhrase);
        Unit target = GetTargetMonster(player.Table.ActiveUnits);
        return target;
    }

    public List<Unit> FilterAliveAndNotEmptyMonsters(List<Unit> tableActiveUnits)
    {
        List<Unit> validMonsters = new List<Unit>();
        foreach (var monster in tableActiveUnits)
        {
            if (monster.IsEmpty() || !monster.IsAlive()) continue;
            validMonsters.Add(monster);
        }
        return validMonsters;
    }

    public Unit GetAllyTarget(Unit attacker)
    {
        var player = _gameState.TurnPlayer;
        var selectionPhrase = $"Seleccione un objetivo para {attacker.Name}";
        var possibleTargets = FilterAliveAndNotEmptyMonsters(player.Table.ActiveUnits);
        _view.DisplayMonsterSelection(possibleTargets, selectionPhrase);
        Unit target = GetTargetMonster(player.Table.ActiveUnits);
        return target;
    }

    public Unit GetDeadAllyTarget(Unit attacker)
    {
        var selectionPhrase = $"Seleccione un objetivo para {attacker.Name}";
        var deadMonsters = FilterDeadMonsters(_gameState.TurnPlayer.GetAllUnits());
        _view.DisplayMonsterSelection(deadMonsters, selectionPhrase);
        Unit target = GetDeadTargetMonster(_gameState.TurnPlayer.GetAllUnits());
        return target;
    }

    private List<Unit> FilterDeadMonsters(List<Unit> getAllUnits)
    {
        List<Unit> deadMonsters = new List<Unit>();
        foreach (var monster in getAllUnits)
        {
            if (monster.IsAlive()) continue;
            deadMonsters.Add(monster);
        }
        return deadMonsters;
    }

    public void DisplayPlayerActionSelectionMenu(Unit monster)
    {
        _view.WriteLine(Params.Separator);
        _view.WriteLine($"Seleccione una acción para {monster.Name}");
        var actions = monster is Samurai ? Params.SamuraiActions : Params.MonsterActions;
        DisplayItemList(actions, '1', ": ");
    }
    
    private void DisplayItemList(string[] items, char counterLabel, string separator)
    {
        foreach (var item in items)
        {
            _view.WriteLine($"{counterLabel}{separator}{item}");
            counterLabel++;
        }
    }
}