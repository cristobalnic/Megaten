using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.GameLoop.Actions.AttackActions;
using Shin_Megami_Tensei.Views;

namespace Shin_Megami_Tensei.GameLoop.Actions;

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

    internal void ExecuteSummon(Unit summoner)
    {
        var selectionPhrase = "Seleccione un monstruo para invocar";
        _view.DisplayMonsterSelection(_gameState.TurnPlayer.Table.Reserve, selectionPhrase);
        Unit monsterSummon = _selectionUtils.GetTargetMonster(_gameState.TurnPlayer.Table.Reserve);
        summoner.Summon(monsterSummon, _gameState.TurnPlayer.Table, _selectionUtils);
        _view.WriteLine(Params.Separator);
        _view.WriteLine($"{monsterSummon.Name} ha sido invocado");
        _gameState.TurnPlayer.TurnState.PassTurnOrSummonTurn();
    }

    internal void ExecuteSpecialSummon()
    {
        var selectionPhrase = "Seleccione un monstruo para invocar";
        _view.DisplayMonsterSelection(_gameState.TurnPlayer.Table.Reserve, selectionPhrase);
        Unit monsterSummon = _selectionUtils.GetTargetMonster(_gameState.TurnPlayer.Table.Reserve);
        _gameState.TurnPlayer.Samurai.Summon(monsterSummon, _gameState.TurnPlayer.Table, _selectionUtils);
        _view.WriteLine(Params.Separator);
        _view.WriteLine($"{monsterSummon.Name} ha sido invocado");
        _gameState.TurnPlayer.TurnState.UseTurnsForNonOffensiveSkill();
    }
    
    internal void ExecuteHealSummon(Unit attacker, Skill skill)
    {
        var selectionPhrase = "Seleccione un monstruo para invocar";
        _view.DisplayMonsterSelection(_gameState.TurnPlayer.Table.Reserve, selectionPhrase, showAll: true);
        Unit monsterSummon = _selectionUtils.GetTargetMonster(_gameState.TurnPlayer.Table.Reserve, showAll: true);
        _gameState.TurnPlayer.Samurai.Summon(monsterSummon, _gameState.TurnPlayer.Table, _selectionUtils);
        _view.WriteLine(Params.Separator);
        _view.WriteLine($"{monsterSummon.Name} ha sido invocado");

        if (!monsterSummon.IsAlive())
        {
            _view.WriteLine($"{attacker.Name} revive a {monsterSummon.Name}");
            var healAmount = AttackUtils.GetRoundedInt(monsterSummon.Stats.MaxHp * (skill.Power * 0.01));
            int currentHp = monsterSummon.Stats.Hp;
            monsterSummon.Stats.Hp = Math.Min(monsterSummon.Stats.MaxHp, currentHp + healAmount);
            int healed = monsterSummon.Stats.Hp - currentHp;
            _view.WriteLine($"{monsterSummon.Name} recibe {healAmount} de HP"); // AQUÍ HAY UN ERROR EN TESTS
            _view.DisplayHpMessage(monsterSummon);
        }
        
        
        _gameState.TurnPlayer.TurnState.UseTurnsForNonOffensiveSkill();
    }
}