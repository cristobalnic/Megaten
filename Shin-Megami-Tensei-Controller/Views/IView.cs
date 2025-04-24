using Shin_Megami_Tensei.Entities;

namespace Shin_Megami_Tensei.Views;

public interface IView
{
    void WriteLine(string message);
    string ReadLine();
    void DisplayHpMessage(Unit target);
    void DisplayAttackMessage(Unit attacker, Skill selectedSkill, Unit target);
    void DisplayDamageMessage(Unit target, int damage);
    void DisplayRepeledDamageMessage(Unit target, int damage, Unit attacker);
    void DisplayDrainDamageMessage(Unit target, int drainDamage);
    void DisplayMonsterSelection(List<Unit> tableMonsters, string displayPhrase);
    void DisplayPlayersTables(List<Player> gameStatePlayers);
    void DisplayRoundInit(Player turnPlayer);
}
