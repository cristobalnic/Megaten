using Shin_Megami_Tensei_View;
using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.GameLoop.Actions;

namespace Shin_Megami_Tensei.GameLoop;

public class ActionManager
{
    private readonly View _view;
    private readonly TurnManager _turnManager;
    private readonly RoundManager _roundManager;

    private readonly AttackAction _attackAction;
    private readonly ShootAction _shootAction;
    private readonly UseSkillAction _useSkillAction;
    private readonly SummonAction _summonAction;
    private readonly PassTurnAction _passTurnAction;
    private readonly SurrenderAction _surrenderAction;
    
    public ActionManager(View view, TurnManager turnManager, RoundManager roundManager)
    {
        _view = view;
        _turnManager = turnManager;
        _roundManager = roundManager;
        _attackAction = new AttackAction(view, turnManager);
        _shootAction = new ShootAction(view, turnManager);
        _useSkillAction = new UseSkillAction(view, turnManager);
        _summonAction = new SummonAction(view);
        _passTurnAction = new PassTurnAction(turnManager);
        _surrenderAction = new SurrenderAction(view);
    }

    public void DisplayPlayerActionSelectionMenu(Unit monster)
    {
        _view.WriteLine(Params.Separator);
        _view.WriteLine($"Seleccione una acción para {monster.Name}");
        var actions = monster is Samurai ? Params.SamuraiActions : Params.MonsterActions;
        DisplayItemList(actions, '1', ": ");
    }
    
    private void DisplayItemList(string[] items, char counterLabel, string separator)
    {
        foreach (var item in items)
        {
            _view.WriteLine($"{counterLabel}{separator}{item}");
            counterLabel++;
        }
    }

    public void PlayerActionExecution(Unit monster, Player turnPlayer, Player waitPlayer)
    {
        var action = GetPlayerAction(monster);
        _view.WriteLine(Params.Separator);
        if (action == "Atacar") _attackAction.ExecuteAttack(monster, waitPlayer);
        else if (action == "Disparar") _shootAction.ExecuteShoot(monster, waitPlayer);
        else if (action == "Usar Habilidad") _useSkillAction.ExecuteUseSkill(monster);
        else if (action == "Invocar") _summonAction.ExecuteSummon();
        else if (action == "Pasar Turno") _passTurnAction.ExecutePassTurn();
        else if (action == "Rendirse") _surrenderAction.ExecuteSurrender(turnPlayer, waitPlayer);
        
    }
    
    private string GetPlayerAction(Unit monster)
    {
        var actionSelection = int.Parse(_view.ReadLine()) - 1; 
        return monster is Samurai ? Params.SamuraiActions[actionSelection] : Params.MonsterActions[actionSelection];
    }
}