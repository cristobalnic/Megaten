using Shin_Megami_Tensei.DataStructures;
using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.Enums;
using Shin_Megami_Tensei.ErrorHandling;
using Shin_Megami_Tensei.GameActions.AttackActions;
using Shin_Megami_Tensei.GameActions.GameFlowActions;
using Shin_Megami_Tensei.GameData;
using Shin_Megami_Tensei.Utils;
using Shin_Megami_Tensei.Views;

namespace Shin_Megami_Tensei.GameActions.SkillActions;

public class UseSkillAction
{
    private readonly IView _view;
    private readonly GameState _gameState;
    private readonly SelectionUtils _selectionUtils;
    private readonly SkillHandlerFactory _skillHandlerFactory;
    
    public UseSkillAction(IView view, GameState gameState)
    {
        _view = view;
        _gameState = gameState;
        _selectionUtils = new SelectionUtils(view, gameState);
        _skillHandlerFactory = new SkillHandlerFactory(_view, _gameState);
    }

    internal void Execute(Unit attacker)
    {
        Skill skill = GetSelectedSkill(attacker);
        _view.WriteLine(Params.Separator);
        var strategy = _skillHandlerFactory.GetSkillStrategy(skill);
        strategy.Execute(attacker, skill);
        attacker.Stats.Mp -= skill.Cost;
        _gameState.TurnPlayer.KSkillsUsed++;
    }

    private Skill GetSelectedSkill(Unit attacker)
    {
        _view.DisplaySkillSelection(attacker);
        var affordableSkills = attacker.Skills.Where(skill => attacker.Stats.Mp >= skill.Cost).ToList();
        var skillSelection = int.Parse(_view.ReadLine());
        if (skillSelection > affordableSkills.Count)
        {
            throw new CancelObjectiveSelectionException();
        }
        return attacker.Skills[skillSelection-1];
    }
}