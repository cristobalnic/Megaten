using Shin_Megami_Tensei.DataStructures;
using Shin_Megami_Tensei.Entities;

namespace Shin_Megami_Tensei.Views;

public interface IView
{
    void WriteLine(string message);
    string ReadLine();
    void DisplayHpMessage(Unit target);
    void DisplayAttackMessage(CombatRecord combatRecord, Skill skill);
    void DisplayAttackResultMessage(CombatRecord combatRecord);
    void DisplayMonsterSelection(List<Unit> tableMonsters, string displayPhrase);
    void DisplayPlayersTables(List<Player> gameStatePlayers);
    void DisplayRoundInit(Player turnPlayer);
    void DisplaySkillSelection(Unit attacker);
    void DisplayAffinityDetectionMessage(CombatRecord combatRecord);
    void DisplayInstantKillSkillResultMessage(CombatRecord combatRecord, bool skillHasMissed);
}
