using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.GameActions.AttackActions;
using Shin_Megami_Tensei.GameActions.GameFlowActions;
using Shin_Megami_Tensei.GameActions.SkillActions;
using Shin_Megami_Tensei.GameData;
using Shin_Megami_Tensei.Views;

namespace Shin_Megami_Tensei.GameLoop;

public class ActionManager
{
    private readonly IView _view;

    private readonly PhysicalAttack _physicalAttack;
    private readonly ShootAttack _shootAttack;
    private readonly UseSkillAction _useSkillAction;
    private readonly SummonAction _summonAction;
    private readonly PassTurnAction _passTurnAction;
    private readonly SurrenderAction _surrenderAction;
    
    public ActionManager(IView view, GameState gameState)
    {
        _view = view;
        _physicalAttack = new PhysicalAttack(view, gameState);
        _shootAttack = new ShootAttack(view, gameState);
        _useSkillAction = new UseSkillAction(view, gameState);
        _summonAction = new SummonAction(view, gameState);
        _passTurnAction = new PassTurnAction(gameState);
        _surrenderAction = new SurrenderAction(view, gameState);
    }
    
    public void PlayerActionExecution(Unit monster)
    {
        var action = GetPlayerAction(monster);
        if (action != "Pasar Turno") _view.WriteLine(Params.Separator);
        if (action == "Atacar") _physicalAttack.Execute(monster);
        else if (action == "Disparar") _shootAttack.Execute(monster);
        else if (action == "Usar Habilidad") _useSkillAction.Execute(monster);
        else if (action == "Invocar") _summonAction.Execute(monster);
        else if (action == "Pasar Turno") _passTurnAction.Execute();
        else if (action == "Rendirse") _surrenderAction.Execute();
        
    }
    
    private string GetPlayerAction(Unit monster)
    {
        var actionSelection = int.Parse(_view.ReadLine()) - 1; 
        return monster is Samurai ? Params.SamuraiActions[actionSelection] : Params.MonsterActions[actionSelection];
    }
}