using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.Enums;
using Shin_Megami_Tensei.Views;

namespace Shin_Megami_Tensei.GameLoop.Actions;

public class PhysicalAttack : BaseAttack
{
    public PhysicalAttack(IView view, GameState gameState) : base(view, gameState) {}

    protected override string GetActionMessage(Unit attacker, Unit target) =>
        $"{attacker.Name} ataca a {target.Name}";

    protected override double GetBaseDamage(Unit attacker) =>
        attacker.Stats.Str * Params.AttackDamageModifier * Params.AttackAndShootDamageMultiplier;

    protected override AffinityType GetAffinity(Unit target) => target.Affinity.Phys;
}