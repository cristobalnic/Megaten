using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.Views;

namespace Shin_Megami_Tensei.Skills.SkillEffects;

public class BaseEffect : ISkillEffect
{
    public void Apply(IView view, Unit attacker, Unit target, Skill skill)
    {
        // This is a base effect that does nothing.
    }
}