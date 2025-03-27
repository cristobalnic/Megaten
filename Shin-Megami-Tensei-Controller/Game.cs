using System.Diagnostics.CodeAnalysis;
using Shin_Megami_Tensei_View;
using Shin_Megami_Tensei.DataStructures;
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
    private Player _currentPlayer = Player1;

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
            _view.WriteLine(exception.GetErrorMessage());
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
            PlayRound(_currentPlayer);
            _round++;
        }
    }
    
    private void SetCurrentPlayer() => _currentPlayer = _players[_round % 2];

    private void PlayRound(Player player)
    {
        DisplayRoundInit(player);
        DisplayPlayerTable(player);
    }
    
    private void DisplayRoundInit(Player player)
    {
        _view.WriteLine(Params.Separator);
        _view.WriteLine($"Ronda de {player.Samurai?.Name} (J{player.Id})");
    }

    private void DisplayPlayerTable(Player player)
    {
        var table = player.Table;
        var samurai = player.Table.Samurai;
        _view.WriteLine(Params.Separator);
        _view.WriteLine($"Equipo de {samurai?.Name} (J{player.Id})");
        _view.WriteLine($"A-{samurai?.Name} HP: {samurai?.Stats.Hp} / {samurai?.Stats.MaxHp}");
        char label = 'B';
        foreach (var monster in table.Monsters)
        {
            _view.WriteLine($"{label}-{monster.Name} HP: {monster.Stats.Hp} / {monster.Stats.MaxHp}");
            label++;
        }
    }
    
    
}