using Shin_Megami_Tensei_View;
using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.MegatenErrorHandling;
using Shin_Megami_Tensei.Utils;

namespace Shin_Megami_Tensei;

public class Game
{
    private readonly View _view;
    private readonly string _teamsFolder;

    private bool IsGameEnded = false;
    private readonly List<Player> _players = [new(1), new(2)];
    private int _round;
    private Player _turnPlayer;
    private Player _waitPlayer;

    private readonly DataLoader _dataLoader;
    public Game(View view, string teamsFolder)
    {
        _view = view;
        _teamsFolder = teamsFolder;
        _dataLoader = new DataLoader();
    }

    public void TryToPlay()
    {
        try
        {
            Play();
        }
        catch (MegatenException exception)
        {
            var message = exception.GetErrorMessage();
            if (message != "ENDGAME") _view.WriteLine(message);
        }
    }

    private void Play()
    {
        GameSetup();
        StartGame();
    }
    
    private void GameSetup()
    {
        DisplayTeamFileSelection();
        LoadTeams();
        ValidateTeams();
        SetupTable();
    }

    private void DisplayTeamFileSelection()
    {
        string[] teamFiles = GetTeamFiles();
        _view.WriteLine("Elige un archivo para cargar los equipos");
        for (var i = 0; i < teamFiles.Length; i++)
            _view.WriteLine($"{i}: {Path.GetFileName(teamFiles[i])}");
    }
    
    private string[] GetTeamFiles() => Directory.GetFiles(_teamsFolder, "*.txt");

    private void LoadTeams()
    {
        var selectedTeamFileLines = GetSelectedTeamFileLines();

        Player currentPlayer = _players[0];

        foreach (var unitRawData in selectedTeamFileLines)
        {
            if (unitRawData.StartsWith("Player 1 Team")) currentPlayer = _players[0];
            else if (unitRawData.StartsWith("Player 2 Team")) currentPlayer = _players[1];
            else if (currentPlayer != null) AddUnitToPlayer(unitRawData, currentPlayer);
        }
    }

    private string[] GetSelectedTeamFileLines()
    {
        var teamFiles = GetTeamFiles();
        var selectedTeamIndex = int.Parse(_view.ReadLine());
        return File.ReadAllLines(teamFiles[selectedTeamIndex]);
    }

    private void AddUnitToPlayer(string unitRawData, Player currentPlayer)
    {
        if (unitRawData.StartsWith("[Samurai]"))
        {
            _dataLoader.LoadSamuraiUnitToPlayer(unitRawData, currentPlayer);
            _dataLoader.LoadSkillsToSamurai(unitRawData, currentPlayer.Samurai);
        }
        else
            _dataLoader.LoadMonsterUnitToPlayer(unitRawData, currentPlayer);
    }

    private void ValidateTeams()
    {
        if (!_players[0].IsTeamValid() || !_players[1].IsTeamValid())
            throw new InvalidTeamException();
    }
    
    private void SetupTable()
    {
        _players[0].Table.SetSamurai(_players[0].Samurai);
        _players[1].Table.SetSamurai(_players[1].Samurai);
        foreach (var monster in _players[0].Units) _players[0].Table.AddMonster(monster);
        foreach (var monster in _players[1].Units) _players[1].Table.AddMonster(monster);
        _players[0].Table.FillEmptySlotsToNull();
        _players[1].Table.FillEmptySlotsToNull();
    }
    
    private void StartGame()
    {
        while (IsGameEnded == false)
        {
            SetPlayersRoles();
            PlayRound();
            _round++;
        }
    }
    
    private void SetPlayersRoles()
    {
        _turnPlayer = _players[_round % 2];
        _waitPlayer = _players[(_round + 1) % 2];
    }

    private void PlayRound()
    {
        DisplayRoundInit();
        _turnPlayer.ResetAvailableTurns();
        var orderedMonsters = GetMonstersOrderedBySpeed();
        while (_turnPlayer.FullTurns > 0)
        {
            PlayTurn(orderedMonsters);
            orderedMonsters = ReorderMonsters(orderedMonsters);
        }
    }

    private void PlayTurn(List<Unit> orderedMonsters)
    {
        DisplayPlayerTables();
        DisplayPlayerAvailableTurns();
        DisplayPlayerMonstersOrderedBySpeed(orderedMonsters);
        TryToExecuteAction(orderedMonsters);
        UpdateTurnState();
        DisplayWinnerIfItExists();
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

    private void DisplayRoundInit()
    {
        _view.WriteLine(Params.Separator);
        _view.WriteLine($"Ronda de {_turnPlayer.Samurai?.Name} (J{_turnPlayer.Id})");
    }
    private void DisplayPlayerTables()
    {
        _view.WriteLine(Params.Separator);
        DisplayPlayerTable(_players[0]);
        DisplayPlayerTable(_players[1]);
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
    
    private void DisplayPlayerAvailableTurns()
    {
        _view.WriteLine(Params.Separator);
        _view.WriteLine($"Full Turns: {_turnPlayer.FullTurns}");
        _view.WriteLine($"Blinking Turns: {_turnPlayer.BlinkingTurns}");
    }
    private List<Unit> GetMonstersOrderedBySpeed()
    {
        var monsters = new List<Unit>();
        foreach (var monster in _turnPlayer.Table.Monsters)
        {
            if (monster == null || !monster.IsAlive()) continue;
            monsters.Add(monster);
        }
        return monsters.OrderByDescending(monster => monster.Stats.Spd).ToList();
    }
    private void DisplayPlayerMonstersOrderedBySpeed(List<Unit> orderedMonsters)
    {
        _view.WriteLine(Params.Separator);
        _view.WriteLine("Orden:");
        int counter = 1;
        foreach (var monster in orderedMonsters)
        {
            _view.WriteLine($"{counter}-{monster.Name}");
            counter++;
        }
    }
    private void DisplayPlayerActionSelectionMenu(Unit monster)
    {
        _view.WriteLine(Params.Separator);
        _view.WriteLine($"Seleccione una acción para {monster.Name}");
        DisplayActionList(monster is Samurai ? Params.SamuraiActions : Params.MonsterActions);
    }
    private void DisplayActionList(string[] actions)
    {
        int counter = 1;
        foreach (var action in actions)
        {
            _view.WriteLine($"{counter}: {action}");
            counter++;
        }
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
        var actionSelection = int.Parse(_view.ReadLine());
        return monster is Samurai ? Params.SamuraiActions[actionSelection-1] : Params.MonsterActions[actionSelection-1];
    }

    private void ExecuteAttack(Unit monster)
    {
        _view.WriteLine($"Seleccione un objetivo para {monster.Name}");
        DisplayTargetSelection(_waitPlayer);
        Unit defenderMonster = GetPlayerObjective();
        int damage = Convert.ToInt32(Math.Floor(Math.Max(0, GetAttackDamage(monster))));
        _view.WriteLine(Params.Separator);
        _view.WriteLine($"{monster.Name} ataca a {defenderMonster.Name}");
        DealDamage(defenderMonster, damage);
    }

    private static double GetAttackDamage(Unit monster)
    {
        return monster.Stats.Str * Params.AttackDamageModifier * Params.AttackAndShootDamageMultiplier;
    }

    private void ExecuteShoot(Unit monster)
    {
        _view.WriteLine($"Seleccione un objetivo para {monster.Name}");
        DisplayTargetSelection(_waitPlayer);
        Unit defenderMonster = GetPlayerObjective();
        int damage = Convert.ToInt32(Math.Floor(Math.Max(0, GetShootDamage(monster))));
        _view.WriteLine(Params.Separator);
        _view.WriteLine($"{monster.Name} dispara a {defenderMonster.Name}");
        DealDamage(defenderMonster, damage);
    }

    private static double GetShootDamage(Unit monster)
    {
        return monster.Stats.Skl * Params.ShootDamageModifier * Params.AttackAndShootDamageMultiplier;
    }

    private void DealDamage(Unit monster, int damage)
    {
        monster.Stats.Hp = Math.Max(0, monster.Stats.Hp - damage);
        _view.WriteLine($"{monster.Name} recibe {damage} de daño");
        _view.WriteLine($"{monster.Name} termina con HP:{monster.Stats.Hp}/{monster.Stats.MaxHp}");
        if (!monster.IsAlive()) _waitPlayer.Table.HandleDeath(monster);
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
        _turnPlayer.FullTurns -= 1;
        _view.WriteLine($"Se han consumido 1 Full Turn(s) y 0 Blinking Turn(s)\nSe han obtenido 0 Blinking Turn(s)");
    }
    
    private List<Unit> ReorderMonsters(List<Unit> orderedMonsters)
    {
        var reorderedMonsters = new List<Unit>();
        reorderedMonsters.AddRange(orderedMonsters.GetRange(1, orderedMonsters.Count - 1));
        reorderedMonsters.Add(orderedMonsters[0]);
        return reorderedMonsters;
    }

    private void DisplayWinnerIfItExists()
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
