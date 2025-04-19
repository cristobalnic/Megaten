using Shin_Megami_Tensei_View;
using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.Enums;
using Shin_Megami_Tensei.GameSetup;
using Shin_Megami_Tensei.MegatenErrorHandling;

namespace Shin_Megami_Tensei;

public class Game
{
    private readonly View _view;
    private readonly string _teamsFolder;
    
    private readonly List<Player> _players = [new(1), new(2)];
    private int _round;
    private Player _turnPlayer;
    private Player _waitPlayer;

    private int _fullTurnsUsed;
    private int _blinkingTurnsUsed;
    private int _blinkingTurnsObtained;
    
    private readonly TeamLoader _teamLoader;
    
    public Game(View view, string teamsFolder)
    {
        _view = view;
        _teamsFolder = teamsFolder;
        _turnPlayer = _players[0];
        _waitPlayer = _players[1];
        _teamLoader = new TeamLoader(_view, _teamsFolder, _players);
    }

    public void Play()
    {
        try
        {
            TryToPlay();
        }
        catch (MegatenException exception)
        {
            _view.WriteLine(exception.GetErrorMessage());
        }
    }

    private void TryToPlay()
    {
        SetupGame();
        StartGame();
    }
    
    private void SetupGame()
    {
        SetupView.DisplayTeamFileSelection(_view, _teamsFolder);
        _teamLoader.LoadTeams();
        TeamValidator.ValidateTeams(_players);
        TableSetup.SetupTable(_players);
    }
    
    private void StartGame()
    {
        while (true)
        {
            try
            {
                PlayRound();
            }
            catch (EndGameException)
            {
                break;
            }
        }
    }
    
    private void SetPlayersRoles()
    {
        _turnPlayer = _players[_round % 2];
        _waitPlayer = _players[(_round + 1) % 2];
    }

    private void PlayRound()
    {
        SetPlayersRoles();
        DisplayRoundInit();
        _turnPlayer.ResetRemainingTurns();
        var orderedMonsters = GetAliveMonstersOrderedBySpeed();
        while (_turnPlayer.FullTurns > 0)
        {
            PlayTurn(orderedMonsters);
            orderedMonsters = ReorderMonsters(orderedMonsters);
        }
        _round++;
    }
    
    private void DisplayRoundInit()
    {
        _view.WriteLine(Params.Separator);
        _view.WriteLine($"Ronda de {_turnPlayer.Samurai?.Name} (J{_turnPlayer.Id})");
    }
    
    private List<Unit> GetAliveMonstersOrderedBySpeed()
    {
        var monsters = new List<Unit>();
        foreach (var monster in _turnPlayer.Table.Monsters)
            if (monster != null && monster.IsAlive()) monsters.Add(monster);
        return monsters.OrderByDescending(monster => monster.Stats.Spd).ToList();
    }
    
    private static List<Unit> ReorderMonsters(List<Unit> orderedMonsters)
    {
        var reorderedMonsters = new List<Unit>();
        reorderedMonsters.AddRange(orderedMonsters.GetRange(1, orderedMonsters.Count - 1));
        reorderedMonsters.Add(orderedMonsters[0]);
        return reorderedMonsters;
    }

    private void PlayTurn(List<Unit> orderedMonsters)
    {
        DisplayPlayersTables();
        DisplayPlayerAvailableTurns();
        DisplayPlayerMonstersOrderedBySpeed(orderedMonsters);
        TryToExecuteAction(orderedMonsters);
        UpdateTurnState();
        DisplayWinnerIfExists();
    }
    
    private void DisplayPlayersTables()
    {
        _view.WriteLine(Params.Separator);
        foreach (var player in _players) DisplayPlayerTable(player);
    }
    
    private void DisplayPlayerTable(Player player)
    {
        _view.WriteLine($"Equipo de {player.Table.Samurai?.Name} (J{player.Id})");
        char label = 'A';
        foreach (var monster in player.Table.Monsters)
        {
            if (monster == null)
                _view.WriteLine($"{label}-");
            else
                _view.WriteLine($"{label}-{monster.Name} HP:{monster.Stats.Hp}/{monster.Stats.MaxHp} MP:{monster.Stats.Mp}/{monster.Stats.MaxMp}");
            label++;
        }
    }
    
    private void DisplayPlayerAvailableTurns()
    {
        _view.WriteLine(Params.Separator);
        _view.WriteLine($"Full Turns: {_turnPlayer.FullTurns}");
        _view.WriteLine($"Blinking Turns: {_turnPlayer.BlinkingTurns}");
    }
    
    private void DisplayPlayerMonstersOrderedBySpeed(List<Unit> orderedMonsters)
    {
        _view.WriteLine(Params.Separator);
        _view.WriteLine("Orden:");
        var orderedMonstersNames = orderedMonsters.Select(monster => monster.Name).ToArray(); 
        DisplayItemList(orderedMonstersNames, '1', "-");
    }
    
    private void DisplayItemList(string[] items, char counterLabel, string separator)
    {
        foreach (var item in items)
        {
            _view.WriteLine($"{counterLabel}{separator}{item}");
            counterLabel++;
        }
    }
    
    private void TryToExecuteAction(List<Unit> orderedMonsters)
    {
        try
        {
            DisplayPlayerActionSelectionMenu(orderedMonsters[0]);
            PlayerActionExecution(orderedMonsters[0]);
        }
        catch (CancelObjectiveSelectionException)
        {
            TryToExecuteAction(orderedMonsters);
        }
    }
    
    private void DisplayPlayerActionSelectionMenu(Unit monster)
    {
        _view.WriteLine(Params.Separator);
        _view.WriteLine($"Seleccione una acción para {monster.Name}");
        var actions = monster is Samurai ? Params.SamuraiActions : Params.MonsterActions;
        DisplayItemList(actions, '1', ": ");
    }
    
    private void PlayerActionExecution(Unit monster)
    {
        var action = GetPlayerAction(monster);
        _view.WriteLine(Params.Separator);
        if (action == "Atacar") ExecuteAttack(monster);
        else if (action == "Disparar") ExecuteShoot(monster);
        else if (action == "Usar Habilidad") ExecuteUseSkill(monster);
        else if (action == "Invocar") ExecuteSummon();
        else if (action == "Pasar Turno") ExecutePassTurn();
        else if (action == "Rendirse") ExecuteSurrender();
        
    }
    private string GetPlayerAction(Unit monster)
    {
        var actionSelection = int.Parse(_view.ReadLine()) - 1; 
        return monster is Samurai ? Params.SamuraiActions[actionSelection] : Params.MonsterActions[actionSelection];
    }
    
    private void ExecuteAttack(Unit monster)
    {
        _view.WriteLine($"Seleccione un objetivo para {monster.Name}");
        DisplayTargetSelection(_waitPlayer);
        Unit defenderMonster = GetPlayerObjective();
        int damage = Convert.ToInt32(Math.Floor(Math.Max(0, GetAttackDamage(monster))));
        _view.WriteLine(Params.Separator);
        _view.WriteLine($"{monster.Name} ataca a {defenderMonster.Name}");
        DealDamage(defenderMonster, damage, defenderMonster.Affinity.Phys);
    }

    private static double GetAttackDamage(Unit monster) =>
        monster.Stats.Str * Params.AttackDamageModifier * Params.AttackAndShootDamageMultiplier;

    private void ExecuteShoot(Unit monster)
    {
        _view.WriteLine($"Seleccione un objetivo para {monster.Name}");
        DisplayTargetSelection(_waitPlayer);
        Unit defenderMonster = GetPlayerObjective();
        int damage = Convert.ToInt32(Math.Floor(Math.Max(0, GetShootDamage(monster))));
        _view.WriteLine(Params.Separator);
        _view.WriteLine($"{monster.Name} dispara a {defenderMonster.Name}");
        DealDamage(defenderMonster, damage, defenderMonster.Affinity.Gun);
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

    private Unit GetPlayerObjective()
    {
        var objectiveSelection = int.Parse(_view.ReadLine());
        List<Unit> validMonsters = new List<Unit>();
        foreach (var monster in _waitPlayer.Table.Monsters)
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
    
    private void DealDamage(Unit monster, int damage, AffinityType affinityType)
    {
        // AffinityTypes available: Neutral, Weak, Resist, Null, Repel, Drain

        if (affinityType == AffinityType.Neutral)
        {
            monster.Stats.Hp = Math.Max(0, monster.Stats.Hp - damage);
            _view.WriteLine($"{monster.Name} recibe {damage} de daño");
            _fullTurnsUsed += 1;
        }
        else if (affinityType == AffinityType.Weak)
        {
            monster.Stats.Hp = Convert.ToInt32(Math.Floor(Math.Max(0, monster.Stats.Hp - damage * Params.WeakDamageMultiplier)));
        }
        

        _view.WriteLine($"{monster.Name} termina con HP:{monster.Stats.Hp}/{monster.Stats.MaxHp}");
        if (!monster.IsAlive()) _waitPlayer.Table.HandleDeath(monster);
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

    private void ExecuteSurrender()
    {
        _view.WriteLine($"{_turnPlayer.Samurai?.Name} (J{_turnPlayer.Id}) se rinde");
        _view.WriteLine(Params.Separator);
        _view.WriteLine($"Ganador: {_waitPlayer.Samurai?.Name} (J{_waitPlayer.Id})");
        throw new EndGameException();
    }

    private void UpdateTurnState()
    {
        _view.WriteLine(Params.Separator);
        _turnPlayer.FullTurns -= _fullTurnsUsed;
        _turnPlayer.BlinkingTurns -= _blinkingTurnsUsed;
        _turnPlayer.BlinkingTurns += _blinkingTurnsObtained;
        _view.WriteLine($"Se han consumido {_fullTurnsUsed} Full Turn(s) y {_blinkingTurnsUsed} Blinking Turn(s)");
        _view.WriteLine($"Se han obtenido {_blinkingTurnsObtained} Blinking Turn(s)");
        _fullTurnsUsed = 0;
        _blinkingTurnsUsed = 0;
        _blinkingTurnsObtained = 0;
    }
    
    private void DisplayWinnerIfExists()
    {
        if (_waitPlayer.Table.Monsters.All(monster => monster == null || !monster.IsAlive()))
        {
            _view.WriteLine(Params.Separator);
            _view.WriteLine($"Ganador: {_turnPlayer.Samurai?.Name} (J{_turnPlayer.Id})");
            throw new EndGameException();
        }
        if (_turnPlayer.Table.Monsters.All(monster => monster == null || !monster.IsAlive()))
        {
            _view.WriteLine(Params.Separator);
            _view.WriteLine($"Ganador: {_waitPlayer.Samurai?.Name} (J{_waitPlayer.Id})");
            throw new EndGameException();
        }
    }
}
