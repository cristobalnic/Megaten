using Shin_Megami_Tensei_View;
using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.MegatenErrorHandling;

namespace Shin_Megami_Tensei.GameLoop;

public static class ActionsUtils
{
    public static void DisplayTargetSelection(View view, Player player)
    {
        char label = '1';
        foreach (var monster in player.Table.Monsters)
        {
            if (monster == null || !monster.IsAlive()) continue;
            view.WriteLine($"{label}-{monster.Name} HP:{monster.Stats.Hp}/{monster.Stats.MaxHp} MP:{monster.Stats.Mp}/{monster.Stats.MaxMp}");
            label++;
        }
        view.WriteLine($"{label}-Cancelar");
    }

    public static Unit GetPlayerObjective(View view, Player waitPlayer)
    {
        var objectiveSelection = int.Parse(view.ReadLine());
        List<Unit> validMonsters = new List<Unit>();
        foreach (var monster in waitPlayer.Table.Monsters)
        {
            if (monster == null || !monster.IsAlive()) continue;
            validMonsters.Add(monster);
        }
        if (objectiveSelection > validMonsters.Count)
        {
            throw new CancelObjectiveSelectionException();
        }
        return validMonsters[objectiveSelection-1];
    }
}