using Shin_Megami_Tensei_View;
using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.MegatenErrorHandling;

namespace Shin_Megami_Tensei.GameLoop;

public class ActionsUtils
{
    private View _view;
    public ActionsUtils(View view)
    {
        _view = view;
    }
    
    public void DisplayMonsterSelection(List<Unit> monsters)
    {
        char label = '1';
        foreach (var monster in monsters)
        {
            if (monster.IsEmpty() || !monster.IsAlive()) continue;
            _view.WriteLine($"{label}-{monster.Name} HP:{monster.Stats.Hp}/{monster.Stats.MaxHp} MP:{monster.Stats.Mp}/{monster.Stats.MaxMp}");
            label++;
        }
        _view.WriteLine($"{label}-Cancelar");
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
}