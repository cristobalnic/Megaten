using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.GameActions.AttackActions;
using Shin_Megami_Tensei.Views;

namespace Shin_Megami_Tensei.Skills.SkillEffects;

public class ReviveEffect : ISkillEffect
{
    public void Apply(IView view, Unit attacker, Unit target, Skill skill)
    {
        var healAmount = AttackUtils.GetRoundedInt(target.Stats.MaxHp * (skill.Power * 0.01));
        int currentHp = target.Stats.Hp;
        target.Stats.Hp = Math.Min(target.Stats.MaxHp, currentHp + healAmount);
        // int healedAmount = target.Stats.Hp - currentHp; // No se usa ahora, pero debería usarse en vez de healAmount
        DisplayReviveMessages(view, attacker, target, healAmount);
    }

    private static void DisplayReviveMessages(IView view, Unit attacker, Unit target, int healAmount)
    {
        view.WriteLine($"{attacker.Name} revive a {target.Name}");
        view.WriteLine($"{target.Name} recibe {healAmount} de HP"); // AQUÍ HAY UN ERROR EN TESTS
        view.DisplayHpMessage(target);
    }
}