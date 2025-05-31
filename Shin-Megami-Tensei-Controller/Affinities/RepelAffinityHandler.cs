using Shin_Megami_Tensei.DataStructures;
using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.GameActions.AttackActions;
using Shin_Megami_Tensei.GameData;

namespace Shin_Megami_Tensei.Affinities;

public class RepelAffinityHandler : AffinityHandler
{
    public override double ApplyDamageModifier(double baseDamage)
    {
        throw new NotImplementedException();
    }

    public override void UseTurns(TurnState turnState)
    {
        turnState.UseFullTurn(turnState.GetFullTurns());
        turnState.UseBlinkingTurn(turnState.GetBlinkingTurns());
    }
    
    public override Unit GetDamagedUnit(CombatRecord combatRecord) => combatRecord.Attacker;

    public override void DealDamageByAffinityRules(CombatRecord combatRecord)
    {
        AttackUtils.ApplyDamage(combatRecord.Attacker, combatRecord.Damage);
    }

    public override string GetAttackResultMessage(CombatRecord combatRecord)
    {
        return $"{combatRecord.Target.Name} devuelve {combatRecord.Damage} daño a {combatRecord.Attacker.Name}";
    }

    public override string GetInstantKillResultMessage(CombatRecord combatRecord)
    {
        throw new NotImplementedException($"Affinity type {combatRecord.Affinity} not implemented for Instant Kill Skill Result Message");
    }
}