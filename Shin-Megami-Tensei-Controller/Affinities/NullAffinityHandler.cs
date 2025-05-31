using Shin_Megami_Tensei.DataStructures;
using Shin_Megami_Tensei.GameData;

namespace Shin_Megami_Tensei.Affinities;

public class NullAffinityHandler : AffinityHandler
{
    public override double ApplyDamageModifier(double baseDamage)
    {
        throw new NotImplementedException();
    }

    public override void UseTurns(TurnState turnState)
    {
        for (int i = 0; i < 2; i++)
        {
            if (turnState.GetBlinkingTurns() > 0)
                turnState.UseBlinkingTurn();
            else if (turnState.GetFullTurns() > 0)
                turnState.UseFullTurn();
        }
    }

    public override double GetDamageByAffinityRules(double baseDamage)
    {
        return baseDamage * Params.NullDamageMultiplier;
    }

    public override string GetAttackResultMessage(CombatRecord combatRecord)
    {
        return $"{combatRecord.Target.Name} bloquea el ataque de {combatRecord.Attacker.Name}";
    }

    public override string GetInstantKillResultMessage(CombatRecord combatRecord)
    {
        return GetAttackResultMessage(combatRecord);
    }
}