using Shin_Megami_Tensei.DataStructures;
using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.GameActions.AttackActions;
using Shin_Megami_Tensei.GameData;

namespace Shin_Megami_Tensei.Affinities;

public class NeutralAffinityHandler : AffinityHandler
{
    public override double ApplyDamageModifier(double baseDamage)
    {
        throw new NotImplementedException();
    }

    public override void UseTurns(TurnState turnState)
    {
        if (turnState.GetBlinkingTurns() > 0)
            turnState.UseBlinkingTurn();
        else
            turnState.UseFullTurn();
    }

    public override string GetAttackResultMessage(CombatRecord combatRecord)
    {
        return $"{combatRecord.Target.Name} recibe {combatRecord.Damage} de daño";
    }

    public override string GetInstantKillResultMessage(CombatRecord combatRecord)
    {
        return $"{combatRecord.Target.Name} ha sido eliminado";
    }

    public override bool HasInstantKillSkillMissed(CombatRecord combatRecord, Skill skill)
    {
        return !(combatRecord.Attacker.Stats.Lck + skill.Power >= combatRecord.Target.Stats.Lck);
    }

    public override void ExecuteInstantKillByAffinityRules(CombatRecord combatRecord)
    {
        AttackUtils.ExecuteInstantKill(combatRecord.Target);
    }
}