using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.Views;

namespace Shin_Megami_Tensei.Skills.SkillEffects;

public interface ISkillEffect
{
    public void Apply(IView view, Unit attacker, Unit target, Skill skill);
}