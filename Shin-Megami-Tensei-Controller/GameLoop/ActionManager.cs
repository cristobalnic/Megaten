using Shin_Megami_Tensei_View;
using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.Enums;
using Shin_Megami_Tensei.MegatenErrorHandling;

namespace Shin_Megami_Tensei.GameLoop;

public class ActionManager
{
    private readonly View _view;
    private readonly TurnManager _turnManager;
    
    public ActionManager(View view, TurnManager turnManager)
    {
        _view = view;
        _turnManager = turnManager;
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
        if (action == "Atacar") ExecuteAttack(monster, waitPlayer);
        else if (action == "Disparar") ExecuteShoot(monster, waitPlayer);
        else if (action == "Usar Habilidad") ExecuteUseSkill(monster);
        else if (action == "Invocar") ExecuteSummon();
        else if (action == "Pasar Turno") ExecutePassTurn();
        else if (action == "Rendirse") ExecuteSurrender(turnPlayer, waitPlayer);
        
    }
    
    private string GetPlayerAction(Unit monster)
    {
        var actionSelection = int.Parse(_view.ReadLine()) - 1; 
        return monster is Samurai ? Params.SamuraiActions[actionSelection] : Params.MonsterActions[actionSelection];
    }
    
    private void ExecuteAttack(Unit monster, Player waitPlayer)
    {
        _view.WriteLine($"Seleccione un objetivo para {monster.Name}");
        DisplayTargetSelection(waitPlayer);
        Unit defenderMonster = GetPlayerObjective(waitPlayer);
        int damage = Convert.ToInt32(Math.Floor(Math.Max(0, GetAttackDamage(monster))));
        _view.WriteLine(Params.Separator);
        _view.WriteLine($"{monster.Name} ataca a {defenderMonster.Name}");
        DealDamage(defenderMonster, damage, defenderMonster.Affinity.Phys, waitPlayer);
    }

    private static double GetAttackDamage(Unit monster) =>
        monster.Stats.Str * Params.AttackDamageModifier * Params.AttackAndShootDamageMultiplier;

    private void ExecuteShoot(Unit monster, Player waitPlayer)
    {
        _view.WriteLine($"Seleccione un objetivo para {monster.Name}");
        DisplayTargetSelection(waitPlayer);
        Unit defenderMonster = GetPlayerObjective(waitPlayer);
        int damage = Convert.ToInt32(Math.Floor(Math.Max(0, GetShootDamage(monster))));
        _view.WriteLine(Params.Separator);
        _view.WriteLine($"{monster.Name} dispara a {defenderMonster.Name}");
        DealDamage(defenderMonster, damage, defenderMonster.Affinity.Gun, waitPlayer);
    }

    private static double GetShootDamage(Unit monster) =>
        monster.Stats.Skl * Params.ShootDamageModifier * Params.AttackAndShootDamageMultiplier;

    private void DisplayTargetSelection(Player player)
    {
        char label = '1';
        foreach (var monster in player.Table.Monsters)
        {
            if (monster == null || !monster.IsAlive()) continue;
            _view.WriteLine($"{label}-{monster.Name} HP:{monster.Stats.Hp}/{monster.Stats.MaxHp} MP:{monster.Stats.Mp}/{monster.Stats.MaxMp}");
            label++;
        }
        _view.WriteLine($"{label}-Cancelar");
    }

    private Unit GetPlayerObjective(Player waitPlayer)
    {
        var objectiveSelection = int.Parse(_view.ReadLine());
        List<Unit> validMonsters = new List<Unit>();
        foreach (var monster in waitPlayer.Table.Monsters)
        {
            if (monster == null || !monster.IsAlive()) continue;
            validMonsters.Add(monster);
        }
        if (objectiveSelection > validMonsters.Count)
        {
            throw new CancelObjectiveSelectionException();
        }
        return validMonsters[objectiveSelection-1];
    }
    
    private void DealDamage(Unit monster, int damage, AffinityType affinityType, Player waitPlayer)
    {
        // AffinityTypes available: Neutral, Weak, Resist, Null, Repel, Drain

        if (affinityType == AffinityType.Neutral)
        {
            monster.Stats.Hp = Math.Max(0, monster.Stats.Hp - damage);
            _view.WriteLine($"{monster.Name} recibe {damage} de daño");
            _turnManager.FullTurnsUsed += 1;
        }
        else if (affinityType == AffinityType.Weak)
        {
            monster.Stats.Hp = Convert.ToInt32(Math.Floor(Math.Max(0, monster.Stats.Hp - damage * Params.WeakDamageMultiplier)));
        }
        

        _view.WriteLine($"{monster.Name} termina con HP:{monster.Stats.Hp}/{monster.Stats.MaxHp}");
        if (!monster.IsAlive()) waitPlayer.Table.HandleDeath(monster);
    }
    
    private void ExecuteUseSkill(Unit monster)
    {
        _view.WriteLine($"Seleccione una habilidad para que {monster.Name} use");
        int label = 1;
        foreach (var skill in monster.Skills)
        {
            if (monster.Stats.Mp < skill.Cost)
                continue;
            _view.WriteLine($"{label}-{skill.Name} MP:{skill.Cost}");
            label++;
        }
        _view.WriteLine($"{label}-Cancelar");
        Skill selectedSkill = GetSelectedSkill(monster);
        int damage = Convert.ToInt32(Math.Floor(Math.Max(0, GetSkillDamage(monster, selectedSkill))));
    }

    private Skill GetSelectedSkill(Unit monster)
    {
        var affordableSkills = monster.Skills.Where(skill => monster.Stats.Mp >= skill.Cost).ToList();
        var skillSelection = int.Parse(_view.ReadLine());
        if (skillSelection > affordableSkills.Count)
        {
            throw new CancelObjectiveSelectionException();
        }
        return monster.Skills[skillSelection-1];
    }
    
    private double GetSkillDamage(Unit monster, Skill skill)
    {
        return Math.Sqrt(monster.Stats.Mag * skill.Power);
    }

    private void ExecuteSummon()
    {
        throw new NotImplementedException();
    }

    private void ExecutePassTurn()
    {
        throw new NotImplementedException();
    }

    private void ExecuteSurrender(Player turnPlayer, Player waitPlayer)
    {
        _view.WriteLine($"{turnPlayer.Samurai?.Name} (J{turnPlayer.Id}) se rinde");
        _view.WriteLine(Params.Separator);
        _view.WriteLine($"Ganador: {waitPlayer.Samurai?.Name} (J{waitPlayer.Id})");
        throw new EndGameException();
    }
}