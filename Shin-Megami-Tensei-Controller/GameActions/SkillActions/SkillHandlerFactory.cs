using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.Enums;
using Shin_Megami_Tensei.GameData;
using Shin_Megami_Tensei.Views;

namespace Shin_Megami_Tensei.GameActions.SkillActions;

public class SkillHandlerFactory
{
    private readonly IView _view;
    private readonly GameState _gameState;

    public SkillHandlerFactory(IView view, GameState gameState)
    {
        _view = view;
        _gameState = gameState;
    }

    public ISkillHandler GetSkillStrategy(Skill skill)
    {
        return skill.Type switch
        {
            SkillType.Heal => new HealSkillHandler(_view, _gameState),
            SkillType.Light or SkillType.Dark => new InstantKillSkillHandler(_view, _gameState),
            SkillType.Special => new SpecialSkillHandler(_view, _gameState),
            _ => new AttackSkillHandler(_view, _gameState),
        };
    }
}