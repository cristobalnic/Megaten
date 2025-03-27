using Shin_Megami_Tensei_View;
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
    private Player _attacker = Player1;
    private Player _defender = Player2;

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
            PlayRound();
            _round++;
        }
    }
    
    private void SetCurrentPlayer()
    {
        _attacker = _players[_round % 2];
        _defender = _players[(_round + 1) % 2];
    }

    private void PlayRound()
    {
        DisplayRoundInit();
        _attacker.ResetAvailableTurns();
        DisplayPlayerAvailableTurns();
        var orderedMonsters = GetMonstersOrderedBySpeed();
        DisplayPlayerMonstersOrderedBySpeed(orderedMonsters);
        DisplayPlayerActionSelectionMenu(orderedMonsters[0]);
        ExecutePlayerAction(orderedMonsters[0]);
    }
    
    private void DisplayRoundInit()
    {
        _view.WriteLine(Params.Separator);
        _view.WriteLine($"Ronda de {_attacker.Samurai?.Name} (J{_attacker.Id})");
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
    private void DisplayPlayerAvailableTurns()
    {
        _view.WriteLine(Params.Separator);
        _view.WriteLine($"Full Turns: {_attacker.FullTurns}");
        _view.WriteLine($"Blinking Turns: {_attacker.BlinkingTurns}");
    }
    private List<Unit> GetMonstersOrderedBySpeed()
    {
        var monsters = _attacker.Table.Monsters;
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
        _view.WriteLine($"Seleccione una acción para {_attacker.Samurai?.Name}");
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

    private void ExecutePlayerAction(Unit monster)
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
        double partialDamage = monster.Stats.Str * Params.AttackDamageModifier * Params.AttackAndShootDamageMultiplier;
        double damage = Math.Min(0, partialDamage);
    }

    private void ExecuteShoot(Unit monster)
    {
        //_view.WriteLine($"Seleccione un objetivo para {monster.Name}"); // SEGUIR ACA!!!!!!!!!
        double partialDamage = monster.Stats.Skl * Params.ShootDamageModifier * Params.AttackAndShootDamageMultiplier;
        double damage = Math.Min(0, partialDamage);
    }

    private void ExecuteUseSkill(Unit monster)
    {
        
        double partialDamage = Math.Sqrt(monster.Stats.Mag * Params.AttackAndShootDamageMultiplier);
        double damage = Math.Min(0, partialDamage);
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
        throw new NotImplementedException();
    }
}
