using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.Enums;

namespace Shin_Megami_Tensei.Views;

public interface IView
{
    void WriteLine(string message);
    string ReadLine();
    void DisplayHpMessage(Unit target);
    void DisplayAttackMessage(Unit attacker, Skill skill, Unit target);
    void DisplayAttackResultMessage(Unit attacker, int damage, Unit target, AffinityType affinityType);
    void DisplayMonsterSelection(List<Unit> tableMonsters, string displayPhrase, bool onlyDead = false, bool showAll = false);
    void DisplayPlayersTables(List<Player> gameStatePlayers);
    void DisplayRoundInit(Player turnPlayer);
    void DisplaySkillSelection(Unit attacker);
    void DisplayAffinityDetectionMessage(Unit attacker, Unit target, AffinityType affinityType);
    void DisplayInstantKillSkillResultMessage(Unit attacker, Unit target, AffinityType targetAffinity, bool skillHasMissed);
}
