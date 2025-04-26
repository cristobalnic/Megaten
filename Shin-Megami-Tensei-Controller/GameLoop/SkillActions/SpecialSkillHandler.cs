using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.Views;

namespace Shin_Megami_Tensei.GameLoop.SkillActions;

public class SpecialSkillHandler : ISkillHandler
{
    private readonly IView _view;
    private readonly GameState _gameState;

    public SpecialSkillHandler(IView view, GameState gameState)
    {
        _view = view;
        _gameState = gameState;
    }

    public void Execute(Unit attacker, Skill skill)
    {
        new UseSkillAction(_view, _gameState).UseSpecialSkill();
    }
}