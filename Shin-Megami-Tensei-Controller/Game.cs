﻿using Shin_Megami_Tensei_View;
using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.MegatenErrorHandling;
using Shin_Megami_Tensei.Utils;


namespace Shin_Megami_Tensei;

public class Game
{
    private readonly View _view;
    private readonly string _teamsFolder;

    public static Player Player1 = new(id: 1);
    public static Player Player2 = new(id: 2);

    private bool ENDGAME = false;
    private readonly List<Player> _players = [Player1, Player2];
    private int _round;
    private Player _turnPlayer = Player1;
    private Player _waitPlayer = Player2;

    public Game(View view, string teamsFolder)
    {
        _view = view;
        _teamsFolder = teamsFolder;
    }

    public void TryToPlay()
    {
        try
        {
            Play();
        }
        catch (MegatenException exception)
        {
            string message = exception.GetErrorMessage();
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
        DisplayTeamFiles();
        LoadTeams();
        ValidateTeams();
        SetupTable();
    }
    
    private void DisplayTeamFiles()
    {
        string[] teamFiles = GetTeamFiles();
        _view.WriteLine("Elige un archivo para cargar los equipos");
        for (var i = 0; i < teamFiles.Length; i++)
            _view.WriteLine($"{i}: {Path.GetFileName(teamFiles[i])}");
    }
    
    private string[] GetTeamFiles()
    {
        return Directory.GetFiles(_teamsFolder, "*.txt");
    }

    private void LoadTeams()
    {
        var selectedTeamFileLines = GetSelectedTeamFileLines();

        Player currentPlayer = Player1;

        foreach (var unitRawData in selectedTeamFileLines)
        {
            if (unitRawData.StartsWith("Player 1 Team")) currentPlayer = Player1;
            else if (unitRawData.StartsWith("Player 2 Team")) currentPlayer = Player2;
            else if (currentPlayer != null) AddUnitToPlayer(unitRawData, currentPlayer);
        }
    }

    private string[] GetSelectedTeamFileLines()
    {
        var teamFiles = GetTeamFiles();
        var selectedTeamIndex = int.Parse(_view.ReadLine());
        return File.ReadAllLines(teamFiles[selectedTeamIndex]);
    }

    private static void AddUnitToPlayer(string unitRawData, Player currentPlayer)
    {
        if (unitRawData.StartsWith("[Samurai]"))
        {
            DataLoader.LoadSamuraiUnitToPlayer(unitRawData, currentPlayer);
            DataLoader.LoadSkillsToSamurai(unitRawData, currentPlayer.Samurai ?? throw new InvalidOperationException());
        }
        else
            DataLoader.LoadMonsterUnitToPlayer(unitRawData, currentPlayer);
    }

    private static void ValidateTeams()
    {
        if (!IsTeamValid(Player1) || !IsTeamValid(Player2))
            throw new InvalidTeamException();
    }

    private static bool IsTeamValid(Player player) => player.Samurai != null;


    private void SetupTable()
    {
        Player1.Table.SetSamurai(Player1.Samurai);
        Player2.Table.SetSamurai(Player2.Samurai);
        foreach (var monster in Player1.Units) Player1.Table.AddMonster(monster);
        foreach (var monster in Player2.Units) Player2.Table.AddMonster(monster);
    }
    
    private void StartGame()
    {
        while (ENDGAME == false)
        {
            SetCurrentPlayer();
            PlayRound();
            _round++;
        }
    }
    
    private void SetCurrentPlayer()
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
        DisplayPlayerTable(Player1);
        DisplayPlayerTable(Player2);
    }
    private void DisplayPlayerTable(Player player, bool displayForTargetSelection = false)
    {
        var table = player.Table;
        var samurai = player.Table.Samurai;
        if (!displayForTargetSelection) _view.WriteLine($"Equipo de {samurai?.Name} (J{player.Id})");
        char label = 'A';
        if (displayForTargetSelection) {label = '1';}
        foreach (var monster in table.Monsters)
        {
            if (monster == null) continue;
            if (!monster.IsAlive() && displayForTargetSelection) continue;
            _view.WriteLine($"{label}-{monster.Name} HP:{monster.Stats.Hp}/{monster.Stats.MaxHp} MP:{monster.Stats.Mp}/{monster.Stats.MaxMp}");
            label++;
        }
        if (displayForTargetSelection) { _view.WriteLine($"{label}-Cancelar"); }
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
        else if (action == "Usar Habilidad") ExecuteUseSkill();
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
        DisplayPlayerTable(_waitPlayer, true);
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
        DisplayPlayerTable(_waitPlayer, true);
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
        if (WasCancelSelected(objectiveSelection)) throw new CancelObjectiveSelectionException();
        List<Unit> validMonsters = new List<Unit>();
        foreach (var monster in _waitPlayer.Table.Monsters)
        {
            if (monster == null || !monster.IsAlive()) continue;
            validMonsters.Add(monster);
        }
        return validMonsters[objectiveSelection-1];
    }
    
    private bool WasCancelSelected(int objectiveSelection)
    {
        return objectiveSelection == _waitPlayer.Table.Monsters.Count + 1;
    }

    private void ExecuteUseSkill()
    {
        
        // double partialDamage = Math.Sqrt(monster.Stats.Mag * Params.AttackAndShootDamageMultiplier);
        // double damage = Math.Max(0, partialDamage);
        throw new NotImplementedException();
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
}
