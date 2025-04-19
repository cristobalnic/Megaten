// using Shin_Megami_Tensei.Entities;
//
// namespace Shin_Megami_Tensei.GameLoop;
//
// public class RoundLogic
// {
//     private List<Player> _players;
//     
//     private Player _turnPlayer;
//     private Player _waitPlayer;
//     private int _round;
//
//     public RoundLogic(List<Player> players)
//     {
//         _players = players;
//     }
//         
//     private void PlayRound()
//     {
//         SetPlayersRoles();
//         DisplayRoundInit();
//         _turnPlayer.ResetRemainingTurns();
//         var orderedMonsters = GetAliveMonstersOrderedBySpeed();
//         while (_turnPlayer.FullTurns > 0)
//         {
//             PlayTurn(orderedMonsters);
//             orderedMonsters = ReorderMonsters(orderedMonsters);
//         }
//         _round++;
//     }
//     
//     private void SetPlayersRoles()
//     {
//         _turnPlayer = _players[_round % 2];
//         _waitPlayer = _players[(_round + 1) % 2];
//     }
// }