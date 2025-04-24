using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.Enums;
using Shin_Megami_Tensei.MegatenErrorHandling;
using Shin_Megami_Tensei.Views;

namespace Shin_Megami_Tensei.GameLoop;

public class SelectionUtils
{
    private IView _view;
    private GameState _gameState;
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
    
    public Unit GetPlayerObjective(List<Unit> monsters)
    {
        var objectiveSelection = int.Parse(_view.ReadLine());
        List<Unit> validMonsters = new List<Unit>();
        foreach (var monster in monsters)
        {
            if (monster.IsEmpty() || !monster.IsAlive()) continue;
            validMonsters.Add(monster);
        }
        if (objectiveSelection > validMonsters.Count)
        {
            throw new CancelObjectiveSelectionException();
        }
        return validMonsters[objectiveSelection-1];
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

    public void DealDamage(Unit attacker, Unit target, double baseDamage, AffinityType affinityType)
    {
        var affinityHandler = new AffinityHandler(_view);
        affinityHandler.HandleAffinityEffect(attacker, target, baseDamage, affinityType);

        if (!target.IsAlive()) _gameState.WaitPlayer.Table.HandleDeath(target);
        if (!attacker.IsAlive()) _gameState.TurnPlayer.Table.HandleDeath(attacker);
    }

    public Unit GetTarget(Unit attacker)
    {
        var selectionPhrase = $"Seleccione un objetivo para {attacker.Name}";
        _view.DisplayMonsterSelection(_gameState.WaitPlayer.Table.Monsters, selectionPhrase);
        Unit target = GetPlayerObjective(_gameState.WaitPlayer.Table.Monsters);
        return target;
    }
}