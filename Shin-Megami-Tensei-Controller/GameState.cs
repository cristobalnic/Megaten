using Shin_Megami_Tensei.Entities;

namespace Shin_Megami_Tensei;

public class GameState
{
    public List<Player> Players { get; }
    public Player TurnPlayer { get; set; }
    public Player WaitPlayer { get; set; }

    public int Round = 0;
    
    public GameState()
    {
        var player1 = new Player(1);
        var player2 = new Player(2);

        Players = [player1, player2];
        TurnPlayer = player1;
        WaitPlayer = player2;
    }
}