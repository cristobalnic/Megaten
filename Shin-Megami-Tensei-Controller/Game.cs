using Shin_Megami_Tensei_View;
using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.GameLoop;
using Shin_Megami_Tensei.GameSetup;
using Shin_Megami_Tensei.MegatenErrorHandling;

namespace Shin_Megami_Tensei;

public class Game
{
    private readonly View _view;
    private readonly string _teamsFolder;
    
    private readonly List<Player> _players = [new(1), new(2)];
    
    private readonly TeamLoader _teamLoader;
    private readonly RoundManager _roundManager;
    
    public Game(View view, string teamsFolder)
    {
        _view = view;
        _teamsFolder = teamsFolder;
        _teamLoader = new TeamLoader(_view, _teamsFolder, _players);
        _roundManager = new RoundManager(_view, _players);
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
                _roundManager.PlayRound();
            }
            catch (EndGameException)
            {
                break;
            }
        }
    }
}
