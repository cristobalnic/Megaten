using Shin_Megami_Tensei.Affinities;
using Shin_Megami_Tensei.DataStructures;
using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.Enums;
using Shin_Megami_Tensei.GameData;
using Shin_Megami_Tensei.Utils;
using Shin_Megami_Tensei.Views;

namespace Shin_Megami_Tensei.GameActions.AttackActions;

public abstract class BaseAttack
{
    private readonly IView _view;
    private readonly GameState _gameState;
    private readonly SelectionUtils _selectionUtils;

    protected BaseAttack(IView view, GameState gameState)
    {
        _view = view;
        _gameState = gameState;
        _selectionUtils = new SelectionUtils(view, gameState);
    }

    internal void Execute(Unit attacker)
    {
        var target = _selectionUtils.GetTarget(attacker);
        AffinityType affinity = GetAffinity(target);
        _view.WriteLine(Params.Separator);
        CombatRecord combatRecord = new CombatRecord(attacker, target, 0, affinity);
        HandleDamage(combatRecord);
        _gameState.TurnPlayer.TurnState.UseTurnsByTargetAffinity(affinity);
    }

    private void HandleDamage(CombatRecord combatRecord)
    {
        combatRecord = DealDamage(combatRecord);
        DisplayDamageMessages(combatRecord);
        HandleDeathsIfAny(combatRecord);
    }

    private CombatRecord DealDamage(CombatRecord combatRecord)
    {
        var baseDamage = GetBaseDamage(combatRecord.Attacker);
        var affinityDamage = AffinityUtils.GetDamageByAffinityRules(baseDamage, combatRecord.Affinity);
        combatRecord.Damage = AttackUtils.GetRoundedInt(affinityDamage);
        AffinityUtils.DealDamageByAffinityRules(combatRecord);
        return combatRecord;
    }
    
    private void DisplayDamageMessages(CombatRecord combatRecord)
    {
        _view.WriteLine(GetActionMessage(combatRecord));
        _view.DisplayAffinityDetectionMessage(combatRecord);
        _view.DisplayAttackResultMessage(combatRecord);
        var affinityHandler = AffinityHandlerFactory.CreateAffinityHandler(combatRecord.Affinity);
        var damagedUnit = affinityHandler.GetDamagedUnit(combatRecord);
        _view.DisplayHpMessage(damagedUnit);
    }
    
    private void HandleDeathsIfAny(CombatRecord combatRecord)
    {
        if (!combatRecord.Target.IsAlive()) _gameState.WaitPlayer.Table.HandleDeath(combatRecord.Target);
        if (!combatRecord.Attacker.IsAlive()) _gameState.TurnPlayer.Table.HandleDeath(combatRecord.Attacker);
    }

    protected abstract AffinityType GetAffinity(Unit target);
    protected abstract double GetBaseDamage(Unit attacker);
    protected abstract string GetActionMessage(CombatRecord combatRecord);
}