using Shin_Megami_Tensei.Entities;

namespace Shin_Megami_Tensei.GameLoop.AttackActions;

public static class AttackUtils
{
    public static int GetRoundedInt(double damage)
    {
        return Convert.ToInt32(Math.Floor(Math.Max(0, damage)));
    }

    public static void ApplyDamage(Unit target, int damage) 
        => target.Stats.Hp = Math.Max(0, target.Stats.Hp - damage);

    public static void ApplyDrain(Unit target, int damage) 
        => target.Stats.Hp = Math.Min(target.Stats.MaxHp, target.Stats.Hp + damage);
    
    public static void ExecuteInstantKill(Unit target) 
        => target.Stats.Hp = 0;
}