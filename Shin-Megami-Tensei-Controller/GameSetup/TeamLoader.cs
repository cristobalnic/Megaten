﻿using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.ErrorHandling;
using Shin_Megami_Tensei.Utils;
using Shin_Megami_Tensei.Views;

namespace Shin_Megami_Tensei.GameSetup;

public class TeamLoader
{
    private readonly IView _view;
    private readonly DataLoader _dataLoader;
    private readonly string _teamsFolder;
    private readonly List<Player> _players;
    
    public TeamLoader(IView view, string teamsFolder, List<Player> players)
    {
        _view = view;
        _dataLoader = new DataLoader();
        _teamsFolder = teamsFolder;
        _players = players;
    }

    internal void LoadTeams()
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
        var teamFiles = SetupUtils.GetTeamFiles(_teamsFolder);
        var selectedTeamIndex = int.Parse(_view.ReadLine());
        return File.ReadAllLines(teamFiles[selectedTeamIndex]);
    }

    

    private void AddUnitToPlayer(string unitRawData, Player currentPlayer)
    {
        if (unitRawData.StartsWith("[Samurai]"))
        {
            if (!currentPlayer.Samurai.IsEmpty())
                throw new InvalidTeamException();
            _dataLoader.LoadSamuraiUnitToPlayer(unitRawData, currentPlayer);
            _dataLoader.LoadSkillsToSamurai(unitRawData, currentPlayer.Samurai);
        }
        else
            _dataLoader.LoadMonsterUnitToPlayer(unitRawData, currentPlayer);
    }
}