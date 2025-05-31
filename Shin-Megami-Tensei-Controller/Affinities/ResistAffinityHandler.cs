using Shin_Megami_Tensei.DataStructures;
using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.GameActions.AttackActions;
using Shin_Megami_Tensei.GameData;

namespace Shin_Megami_Tensei.Affinities;

public class ResistAffinityHandler : AffinityHandler
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

    public override double GetDamageByAffinityRules(double baseDamage)
    {
        return baseDamage * Params.ResistDamageMultiplier;
    }

    public override string GetAttackResultMessage(CombatRecord combatRecord)
    {
        return $"{combatRecord.Target.Name} recibe {combatRecord.Damage} de daño";
    }

    public override string GetInstantKillResultMessage(CombatRecord combatRecord)
    {
        return $"{combatRecord.Target.Name} ha sido eliminado";
    }

    public override string GetAffinityDetectionMessage(CombatRecord combatRecord)
    {
        return $"{combatRecord.Target.Name} es resistente el ataque de {combatRecord.Attacker.Name}";
    }

    public override bool HasInstantKillSkillMissed(CombatRecord combatRecord, Skill skill)
    {
        return !(combatRecord.Attacker.Stats.Lck + skill.Power >= 2 * combatRecord.Target.Stats.Lck);
    }
    
    public override void ExecuteInstantKillByAffinityRules(CombatRecord combatRecord)
    {
        AttackUtils.ExecuteInstantKill(combatRecord.Target);
    }
}