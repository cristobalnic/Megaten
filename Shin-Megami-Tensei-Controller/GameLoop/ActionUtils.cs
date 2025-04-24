using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.Enums;

namespace Shin_Megami_Tensei.GameLoop;

public class ActionUtils
{
    public static int GetRoundedIntDamage(double damage)
    {
        return Convert.ToInt32(Math.Floor(Math.Max(0, damage)));
    }

    public static void ApplyDamage(Unit target, int damage) 
        => target.Stats.Hp = Math.Max(0, target.Stats.Hp - damage);

    public static void ApplyDrain(Unit target, int damage) 
        => target.Stats.Hp = Math.Min(target.Stats.MaxHp, target.Stats.Hp + damage);
    
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