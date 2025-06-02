using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.GameData;
using Shin_Megami_Tensei.Skills.SkillEffects;
using Shin_Megami_Tensei.Utils;
using Shin_Megami_Tensei.Views;

namespace Shin_Megami_Tensei.GameActions.GameFlowActions;

public class SummonAction
{
    private readonly IView _view;
    private readonly GameState _gameState;
    private readonly SelectionUtils _selectionUtils;

    
    public SummonAction(IView view, GameState gameState)
    {
        _view = view;
        _gameState = gameState;
        _selectionUtils = new SelectionUtils(view, gameState);
    }

    internal void Execute(Unit summoner)
    {
        var monsterSummon = GetMonsterToSummon();
        summoner.Summon(monsterSummon, _gameState.TurnPlayer.Table, _selectionUtils);
        _view.WriteLine(Params.Separator);
        _view.WriteLine($"{monsterSummon.Name} ha sido invocado");
        _gameState.TurnPlayer.TurnState.UseTurnsForPassOrSummon();
    }

    internal void ExecuteSpecialSummon()
    {
        var monsterSummon = GetMonsterToSummon();
        _gameState.TurnPlayer.Samurai.Summon(monsterSummon, _gameState.TurnPlayer.Table, _selectionUtils);
        _view.WriteLine(Params.Separator);
        _view.WriteLine($"{monsterSummon.Name} ha sido invocado");
        _gameState.TurnPlayer.TurnState.UseTurnsForNonOffensiveSkill();
    }

    private Unit GetMonsterToSummon()
    {
        var selectionPhrase = "Seleccione un monstruo para invocar";
        var possibleTargets = _selectionUtils.FilterAliveAndNotEmptyMonsters(_gameState.TurnPlayer.Table.Reserve);
        _view.DisplayMonsterSelection(possibleTargets, selectionPhrase);
        Unit monsterSummon = _selectionUtils.GetTargetMonster(possibleTargets);
        return monsterSummon;
    }

    internal void ExecuteHealSummon(Unit attacker, Skill skill)
    {
        var selectionPhrase = "Seleccione un monstruo para invocar";
        _view.DisplayMonsterSelection(_gameState.TurnPlayer.Table.Reserve, selectionPhrase);
        Unit monsterSummon = _selectionUtils.GetTargetMonster(_gameState.TurnPlayer.Table.Reserve);
        _gameState.TurnPlayer.Samurai.Summon(monsterSummon, _gameState.TurnPlayer.Table, _selectionUtils);
        _view.WriteLine(Params.Separator);
        _view.WriteLine($"{monsterSummon.Name} ha sido invocado");

        if (!monsterSummon.IsAlive())
        {
            var skillEffect = new ReviveEffect();
            skillEffect.Apply(_view, attacker, monsterSummon, skill);
        }
        _gameState.TurnPlayer.TurnState.UseTurnsForNonOffensiveSkill();
    }
}