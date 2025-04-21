using Shin_Megami_Tensei_View;
using Shin_Megami_Tensei.Entities;

namespace Shin_Megami_Tensei.GameLoop;

public class RoundManager
{
    private readonly View _view;
    private readonly List<Player> _players;

    public Player TurnPlayer;
    private Player _waitPlayer;
    
    private int _round;
    
    private readonly TurnManager _turnManager;

    public RoundManager(View view, List<Player> players)
    {
        _view = view;
        _players = players;
        TurnPlayer = _players[0];
        _waitPlayer = _players[1];
        _turnManager = new TurnManager(_view, this);
    }

    public void PlayRound()
    {
        SetPlayersRoles();
        DisplayRoundInit();
        TurnPlayer.ResetRemainingTurns();
        var orderedMonsters = GetAliveMonstersOrderedBySpeed();
        while (TurnPlayer.FullTurns > 0)
        {
            DisplayPlayersTables();
            _turnManager.PlayTurn(orderedMonsters, TurnPlayer, _waitPlayer);
            orderedMonsters = ReorderMonsters(orderedMonsters);
        }
        _round++;
    }
    
    private void SetPlayersRoles()
    {
        TurnPlayer = _players[_round % 2];
        _waitPlayer = _players[(_round + 1) % 2];
    }
    
    private void DisplayRoundInit()
    {
        _view.WriteLine(Params.Separator);
        _view.WriteLine($"Ronda de {TurnPlayer.Samurai?.Name} (J{TurnPlayer.Id})");
    }
    
    private List<Unit> GetAliveMonstersOrderedBySpeed()
    {
        var monsters = new List<Unit>();
        foreach (var monster in TurnPlayer.Table.Monsters)
            if (monster != null && monster.IsAlive()) monsters.Add(monster);
        return monsters.OrderByDescending(monster => monster.Stats.Spd).ToList();
    }
    
    private void DisplayPlayersTables()
    {
        _view.WriteLine(Params.Separator);
        foreach (var player in _players) DisplayPlayerTable(player);
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
    
    private static List<Unit> ReorderMonsters(List<Unit> orderedMonsters)
    {
        var reorderedMonsters = new List<Unit>();
        reorderedMonsters.AddRange(orderedMonsters.GetRange(1, orderedMonsters.Count - 1));
        reorderedMonsters.Add(orderedMonsters[0]);
        return reorderedMonsters;
    }
}