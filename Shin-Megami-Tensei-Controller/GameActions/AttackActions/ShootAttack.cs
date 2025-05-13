using Shin_Megami_Tensei.DataStructures;
using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.Enums;
using Shin_Megami_Tensei.GameData;
using Shin_Megami_Tensei.Views;

namespace Shin_Megami_Tensei.GameActions.AttackActions;

public class ShootAttack : BaseAttack
{
    public ShootAttack(IView view, GameState gameState) : base(view, gameState) {}

    protected override string GetActionMessage(CombatRecord combatRecord) =>
        $"{combatRecord.Attacker.Name} dispara a {combatRecord.Target.Name}";

    protected override double GetBaseDamage(Unit attacker) =>
        attacker.Stats.Skl * Params.ShootDamageModifier * Params.AttackAndShootDamageMultiplier;

    protected override AffinityType GetAffinity(Unit target) => target.Affinity.Gun;
}