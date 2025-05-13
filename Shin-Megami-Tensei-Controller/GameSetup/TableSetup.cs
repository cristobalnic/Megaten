using Shin_Megami_Tensei.Entities;

namespace Shin_Megami_Tensei.GameSetup;

public static class TableSetup
{
    public static void SetupTable(List<Player> players)
    {
        foreach (var player in players)
        {
            player.Table.AddSamuraiToTable(player.Samurai);
            foreach (var monster in player.Units) player.Table.AddMonster(monster);
            player.Table.FillEmptySlots();
        }
    }
}