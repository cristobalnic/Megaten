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

    private readonly Player _player1 = new(id: 1);
    private readonly Player _player2 = new(id: 2);

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
    }



    private void GameSetup()
    {
        var teamFiles = GetTeamFiles();
        DisplayTeamFiles(teamFiles);
        LoadTeams(teamFiles);
        ValidateTeams();
    }

    private string[] GetTeamFiles()
    {
        return Directory.GetFiles(_teamsFolder, "*.txt");
    }

    private void DisplayTeamFiles(string[] teamFiles)
    {
        _view.WriteLine("Elige un archivo para cargar los equipos");
        for (var i = 0; i < teamFiles.Length; i++)
            _view.WriteLine($"{i}: {Path.GetFileName(teamFiles[i])}");
    }

    private void LoadTeams(string[] teamFiles)
    {
        var selectedTeamFileLines = GetSelectedTeamFileLines(teamFiles);

        Player currentPlayer = null!;

        foreach (var line in selectedTeamFileLines)
        {
            if (line.StartsWith("Player 1 Team")) currentPlayer = _player1;
            else if (line.StartsWith("Player 2 Team")) currentPlayer = _player2;
            else if (currentPlayer != null) AddUnitToPlayer(line, currentPlayer);
        }
    }

    private string[] GetSelectedTeamFileLines(string[] teamFiles)
    {
        var selectedTeamIndex = int.Parse(_view.ReadLine());
        return File.ReadAllLines(teamFiles[selectedTeamIndex]);
    }

    private static void AddUnitToPlayer(string unitRawData, Player currentPlayer)
    {
        if (unitRawData.StartsWith("[Samurai]"))
            DataLoader.LoadSamuraiUnit(unitRawData, currentPlayer);
        else
            DataLoader.LoadMonsterUnit(unitRawData, currentPlayer);
    }

    private void ValidateTeams()
    {
        if (!IsTeamValid(_player1) || !IsTeamValid(_player2))
        {
            throw new InvalidTeamException();
        }
    }

    private bool IsTeamValid(Player player)
    {
        if (player.Samurais.Count != Params.RequiredSamurais)
            return false;

        if (player.Units.Count + player.Samurais.Count > Params.MaxUnitsAllowed)
            return false;

        if (player.Units.GroupBy(unit => unit.Name).Any(group => group.Count() > 1))
            return false;

        if (player.Samurais.First().Skills.Count > Params.MaxSamuraiSkillsAllowed)
            return false;

        return !player.Samurais.First().Skills.GroupBy(skill => skill).Any(group => group.Count() > 1);
    }
}