using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.Views;

namespace Shin_Megami_Tensei.Skills.SkillEffects;

public class SkillEffectFactory
{
    private readonly IView _view;
    private readonly Unit _attacker;
    private readonly Unit _target;
    private readonly Skill _skill;
    
    public SkillEffectFactory(IView view, Unit attacker, Unit target, Skill skill)
    {
        _view = view;
        _attacker = attacker;
        _target = target;
        _skill = skill;
    }
    
    public ISkillEffect CreateSkillEffect()
    {
        if (_skill.Effect.Contains("Revive"))
        {
            return new ReviveEffect();
        }
        else
        {
            return new BaseEffect();
        }
    }
}