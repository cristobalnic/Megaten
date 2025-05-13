using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.ErrorHandling;

namespace Shin_Megami_Tensei.GameSetup;

public static class TeamValidator
{
    public static void ValidateTeams(List<Player> players)
    {
        if (!players[0].IsTeamValid() || !players[1].IsTeamValid())
            throw new InvalidTeamException();
    }
}