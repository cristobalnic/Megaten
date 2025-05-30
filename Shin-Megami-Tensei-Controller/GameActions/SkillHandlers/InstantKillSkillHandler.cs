using Shin_Megami_Tensei.DataStructures;
using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.Enums;
using Shin_Megami_Tensei.GameData;
using Shin_Megami_Tensei.Utils;
using Shin_Megami_Tensei.Views;

namespace Shin_Megami_Tensei.GameActions.SkillHandlers;

public class InstantKillSkillHandler : ISkillHandler
{
    private readonly IView _view;
    private readonly GameState _gameState;
    private readonly SelectionUtils _selectionUtils;

    public InstantKillSkillHandler(IView view, GameState gameState)
    {
        _view = view;
        _gameState = gameState;
        _selectionUtils = new SelectionUtils(_view, gameState);
    }

    public void Execute(Unit attacker, Skill skill)
    {
        UseInstantKillSkill(attacker, skill);
    }

    private void UseInstantKillSkill(Unit attacker, Skill skill)
    {
        var target = _selectionUtils.GetTarget(attacker);
        _view.WriteLine(Params.Separator);
        AffinityType targetAffinity = AffinityUtils.GetTargetAffinity(skill, target);
        CombatRecord combatRecord = new CombatRecord(attacker, target, 0, targetAffinity);
        bool hasMissed = AffinityUtils.HasInstantKillSkillMissed(combatRecord, skill);
        if (!hasMissed) AffinityUtils.ExecuteInstantKillByAffinityRules(combatRecord);
        _view.DisplayAttackMessage(combatRecord, skill);
        if (!hasMissed) _view.DisplayAffinityDetectionMessage(combatRecord);
        _view.DisplayInstantKillSkillResultMessage(combatRecord, hasMissed);
        if (!target.IsAlive()) _gameState.WaitPlayer.Table.HandleDeath(target);
        if (!attacker.IsAlive()) _gameState.TurnPlayer.Table.HandleDeath(attacker);
        if (!hasMissed) _gameState.TurnPlayer.TurnState.UseTurnsByTargetAffinity(targetAffinity);
        else _gameState.TurnPlayer.TurnState.UseTurnsForNonOffensiveSkill();
        _view.DisplayHpMessage(targetAffinity == AffinityType.Repel ? attacker : target);
    }
}
