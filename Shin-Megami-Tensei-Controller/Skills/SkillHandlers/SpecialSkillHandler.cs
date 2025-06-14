﻿using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.GameActions.GameFlowActions;
using Shin_Megami_Tensei.GameData;
using Shin_Megami_Tensei.Views;

namespace Shin_Megami_Tensei.Skills.SkillHandlers;

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
        UseSpecialSkill();
    }

    private void UseSpecialSkill()
    {
        var summonAction = new SummonAction(_view, _gameState);
        summonAction.ExecuteSpecialSummon();
    }
}