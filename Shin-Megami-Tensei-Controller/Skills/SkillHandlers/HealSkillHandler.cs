using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.GameActions.AttackActions;
using Shin_Megami_Tensei.GameActions.GameFlowActions;
using Shin_Megami_Tensei.GameData;
using Shin_Megami_Tensei.Skills.SkillEffects;
using Shin_Megami_Tensei.Utils;
using Shin_Megami_Tensei.Views;

namespace Shin_Megami_Tensei.Skills.SkillHandlers;

public class HealSkillHandler : ISkillHandler
{
    private readonly IView _view;
    private readonly GameState _gameState;
    private readonly SelectionUtils _selectionUtils;

    public HealSkillHandler(IView view, GameState gameState)
    {
        _view = view;
        _gameState = gameState;
        _selectionUtils = new SelectionUtils(_view, gameState);
    }

    public void Execute(Unit attacker, Skill skill)
    {
        UseHealSkill(attacker, skill);
    }

    private void UseHealSkill(Unit attacker, Skill skill)
    {
        if (skill.Effect.Contains("eals"))
        {
            // VERIFICAR SI HAY MUERTOS (a los que no se les puede curar)
            var beneficiary = _selectionUtils.GetAllyTarget(attacker);
            _view.WriteLine(Params.Separator);
            _view.WriteLine($"{attacker.Name} cura a {beneficiary.Name}");
            var healAmount = AttackUtils.GetRoundedInt(beneficiary.Stats.MaxHp * (skill.Power * 0.01));
            int currentHp = beneficiary.Stats.Hp;
            beneficiary.Stats.Hp = Math.Min(beneficiary.Stats.MaxHp, currentHp + healAmount);
            int healed = beneficiary.Stats.Hp - currentHp;
            _view.WriteLine($"{beneficiary.Name} recibe {healAmount} de HP"); // AQUÍ HAY UN ERROR EN TESTS
            _view.DisplayHpMessage(beneficiary);
            _gameState.TurnPlayer.TurnState.UseTurnsForNonOffensiveSkill();
        }
        else if (skill.Effect.Contains("KO"))
        {
            var beneficiary = _selectionUtils.GetDeadAllyTarget(attacker);
            _view.WriteLine(Params.Separator);

            var skillEffect = new ReviveEffect();
            skillEffect.Apply(_view, attacker, beneficiary, skill);
            
            _gameState.TurnPlayer.TurnState.UseTurnsForNonOffensiveSkill();
        }
        else
        {
            var summonAction = new SummonAction(_view, _gameState);
            summonAction.ExecuteHealSummon(attacker, skill);
        }
    }
}