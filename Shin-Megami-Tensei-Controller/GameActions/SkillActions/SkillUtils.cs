using Shin_Megami_Tensei.Entities;

namespace Shin_Megami_Tensei.GameActions.SkillActions;

public class SkillUtils
{
    public static int GetHits(string hitsString, Player turnPlayer)
    {
        if (hitsString.Contains('-'))
        {
            var parts = hitsString.Split('-');
            int minHits = int.Parse(parts[0]);
            int maxHits = int.Parse(parts[1]);
            int offset = turnPlayer.KSkillsUsed % (maxHits - minHits + 1);
            return minHits + offset;
        }
        return int.Parse(hitsString);
    }
}