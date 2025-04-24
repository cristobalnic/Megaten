using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.GameLoop.Actions;
using Shin_Megami_Tensei.Views;

namespace Shin_Megami_Tensei.GameLoop;

public class ActionManager
{
    private readonly IView _view;

    private readonly AttackAction _attackAction;
    private readonly ShootAction _shootAction;
    private readonly UseSkillAction _useSkillAction;
    private readonly SummonAction _summonAction;
    private readonly PassTurnAction _passTurnAction;
    private readonly SurrenderAction _surrenderAction;
    
    public ActionManager(IView view, GameState gameState)
    {
        _view = view;
        _attackAction = new AttackAction(view, gameState);
        _shootAction = new ShootAction(view, gameState);
        _useSkillAction = new UseSkillAction(view, gameState);
        _summonAction = new SummonAction(view, gameState);
        _passTurnAction = new PassTurnAction(gameState);
        _surrenderAction = new SurrenderAction(view, gameState);
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

    public void PlayerActionExecution(Unit monster)
    {
        var action = GetPlayerAction(monster);
        if (action != "Pasar Turno") _view.WriteLine(Params.Separator);
        if (action == "Atacar") _attackAction.ExecuteAttack(monster);
        else if (action == "Disparar") _shootAction.ExecuteShoot(monster);
        else if (action == "Usar Habilidad") _useSkillAction.ExecuteUseSkill(monster);
        else if (action == "Invocar") _summonAction.ExecuteSummon(monster);
        else if (action == "Pasar Turno") _passTurnAction.ExecutePassTurn();
        else if (action == "Rendirse") _surrenderAction.ExecuteSurrender();
        
    }
    
    private string GetPlayerAction(Unit monster)
    {
        var actionSelection = int.Parse(_view.ReadLine()) - 1; 
        return monster is Samurai ? Params.SamuraiActions[actionSelection] : Params.MonsterActions[actionSelection];
    }
}