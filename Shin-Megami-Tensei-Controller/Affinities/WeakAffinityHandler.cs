using Shin_Megami_Tensei.DataStructures;
using Shin_Megami_Tensei.GameActions.AttackActions;
using Shin_Megami_Tensei.GameData;

namespace Shin_Megami_Tensei.Affinities;

public class WeakAffinityHandler : AffinityHandler
{
    public override double ApplyDamageModifier(double baseDamage)
    {
        throw new NotImplementedException();
    }

    public override void UseTurns(TurnState turnState)
    {
        if (turnState.GetFullTurns() > 0)
        {
            turnState.UseFullTurn();
            turnState.GainBlinkingTurn();
        }
        else
            turnState.UseBlinkingTurn();
    }

    public override double GetDamageByAffinityRules(double baseDamage)
    {
        return baseDamage * Params.WeakDamageMultiplier;
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
        return $"{combatRecord.Target.Name} es débil contra el ataque de {combatRecord.Attacker.Name}";
    }

    public override void ExecuteInstantKillByAffinityRules(CombatRecord combatRecord)
    {
        AttackUtils.ExecuteInstantKill(combatRecord.Target);
    }
}