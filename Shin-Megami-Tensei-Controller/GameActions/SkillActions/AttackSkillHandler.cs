﻿using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.GameData;
using Shin_Megami_Tensei.Views;

namespace Shin_Megami_Tensei.GameActions.SkillActions;

public class AttackSkillHandler : ISkillHandler
{
    private readonly IView _view;
    private readonly GameState _gameState;

    public AttackSkillHandler(IView view, GameState gameState)
    {
        _view = view;
        _gameState = gameState;
    }

    public void Execute(Unit attacker, Skill skill)
    {
        new UseSkillAction(_view, _gameState).UseAttackSkill(attacker, skill);
    }
}