using Shin_Megami_Tensei.Entities;

namespace Shin_Megami_Tensei.Skills.SkillHandlers;

public interface ISkillHandler
{
    void Execute(Unit attacker, Skill skill);
}