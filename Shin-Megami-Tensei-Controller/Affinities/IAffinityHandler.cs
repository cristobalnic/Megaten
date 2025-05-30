using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.GameData;

namespace Shin_Megami_Tensei.Affinities;

public interface IAffinityHandler
{
    public double ApplyDamageModifier(double baseDamage);
    public void UseTurns(TurnState turnState);
}

