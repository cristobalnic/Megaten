using Shin_Megami_Tensei_View;
using Shin_Megami_Tensei.GameLoop;
using Shin_Megami_Tensei.GameSetup;
using Shin_Megami_Tensei.MegatenErrorHandling;

namespace Shin_Megami_Tensei;

public class Game
{
    private readonly View _view;
    private readonly string _teamsFolder;
    private readonly GameState _gameState;
    private readonly TeamLoader _teamLoader;
    private readonly RoundManager _roundManager;
    
    public Game(View view, string teamsFolder)
    {
        _view = view;
        _teamsFolder = teamsFolder;
        _gameState = new GameState();
        _teamLoader = new TeamLoader(_view, _teamsFolder, _gameState.Players);
        _roundManager = new RoundManager(_view, _gameState);
        
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
        TeamValidator.ValidateTeams(_gameState.Players);
        TableSetup.SetupTable(_gameState.Players);
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
