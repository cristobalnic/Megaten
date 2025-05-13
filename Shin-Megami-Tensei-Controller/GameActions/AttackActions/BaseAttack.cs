using Shin_Megami_Tensei.DataStructures;
using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.Enums;
using Shin_Megami_Tensei.GameData;
using Shin_Megami_Tensei.GameLoop;
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
        TurnManager.HandleTurns(_gameState.TurnPlayer, affinity);
    }

    private void HandleDamage(CombatRecord combatRecord)
    {
        var baseDamage = GetBaseDamage(combatRecord.Attacker);
        var affinityDamage = AffinityUtils.GetDamageByAffinityRules(baseDamage, combatRecord.Affinity);
        combatRecord.Damage = AttackUtils.GetRoundedInt(affinityDamage);
        AffinityUtils.DealDamageByAffinityRules(combatRecord);
        
        _view.WriteLine(GetActionMessage(combatRecord));
        _view.DisplayAffinityDetectionMessage(combatRecord);
        _view.DisplayAttackResultMessage(combatRecord);
        
        if (!combatRecord.Target.IsAlive()) _gameState.WaitPlayer.Table.HandleDeath(combatRecord.Target);
        if (!combatRecord.Attacker.IsAlive()) _gameState.TurnPlayer.Table.HandleDeath(combatRecord.Attacker);
        
        _view.DisplayHpMessage(combatRecord.Affinity == AffinityType.Repel ? combatRecord.Attacker : combatRecord.Target);
    }

    protected abstract string GetActionMessage(CombatRecord combatRecord);
    protected abstract double GetBaseDamage(Unit attacker);
    protected abstract AffinityType GetAffinity(Unit target);
}