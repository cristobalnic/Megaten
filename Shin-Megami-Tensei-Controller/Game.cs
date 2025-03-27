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

    public static Table Table = new();
    
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
        player.ResetAvailableTurns();
        DisplayPlayerAvailableTurns(player);
        var orderedMonsters = GetMonstersOrderedBySpeed(player);
        DisplayPlayerMonstersOrderedBySpeed(orderedMonsters);
        DisplayPlayerActionSelection(player);
        var action = GetPlayerAction(player);
        // ExecutePlayerAction(player, action);
    }
    
    private void DisplayRoundInit(Player player)
    {
        _view.WriteLine(Params.Separator);
        _view.WriteLine($"Ronda de {player.Samurai?.Name} (J{player.Id})");
        DisplayPlayerTables();
    }
    private void DisplayPlayerTables()
    {
        _view.WriteLine(Params.Separator);
        DisplayPlayerTable(Player1);
        DisplayPlayerTable(Player2);
    }
    private void DisplayPlayerTable(Player player)
    {
        var table = player.Table;
        var samurai = player.Table.Samurai;
        _view.WriteLine($"Equipo de {samurai?.Name} (J{player.Id})");
        char label = 'A';
        foreach (var monster in table.Monsters)
        {
            _view.WriteLine($"{label}-{monster.Name} HP:{monster.Stats.Hp}/{monster.Stats.MaxHp} MP:{monster.Stats.Mp}/{monster.Stats.MaxMp}");
            label++;
        }
    }
    private void DisplayPlayerAvailableTurns(Player player)
    {
        _view.WriteLine(Params.Separator);
        _view.WriteLine($"Full Turns: {player.FullTurns}");
        _view.WriteLine($"Blinking Turns: {player.BlinkingTurns}");
    }
    private static List<Unit> GetMonstersOrderedBySpeed(Player player)
    {
        var monsters = player.Table.Monsters;
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
    private void DisplayPlayerActionSelection(Player player)
    {
        _view.WriteLine(Params.Separator);
        _view.WriteLine($"Seleccione una acción para {player.Samurai?.Name}");
        _view.WriteLine("1: Atacar");
        _view.WriteLine("2: Disparar");
        _view.WriteLine("3: Usar Habilidad");
        _view.WriteLine("4: Invocar");
        _view.WriteLine("5: Pasar Turno");
        _view.WriteLine("6: Rendirse");
    }
    private int GetPlayerAction(Player player)
    {
        return int.Parse(_view.ReadLine());
    }

    // private void ExecutePlayerAction(Player player, int action)
    // {
    //     if (action == 1) ExecuteAttack(player);
    //     else if (action == 2) ExecuteShoot(player);
    //     else if (action == 3) ExecuteUseSkill(player);
    //     else if (action == 4) ExecuteSummon(player);
    //     else if (action == 5) ExecutePassTurn(player);
    //     else if (action == 6) ExecuteSurrender(player);
    // }
    
    
}
