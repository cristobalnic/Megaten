using Shin_Megami_Tensei_View.ConsoleLib;
using Shin_Megami_Tensei.DataStructures;
using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.Enums;

namespace Shin_Megami_Tensei.Views;

public class ConsoleView : IView
{
    private readonly View _view;

    private ConsoleView(View view) => _view = view;

    public static ConsoleView SetConsoleView(View view) => new(view);

    public void WriteLine(string message) 
        => _view.WriteLine(message);

    public string ReadLine() 
        => _view.ReadLine();
    
    
    public void DisplayRoundInit(Player turnPlayer)
    {
        _view.WriteLine(Params.Separator);
        _view.WriteLine($"Ronda de {turnPlayer.Samurai.Name} (J{turnPlayer.Id})");
    }

    public void DisplayPlayersTables(List<Player> players)
    {
        _view.WriteLine(Params.Separator);
        foreach (var player in players)
        {
            var phrase = $"Equipo de {player.Samurai.Name} (J{player.Id})";
            DisplayPlayerTable(player.Table.ActiveUnits, phrase);
        }
    }
    
    private void DisplayPlayerTable(List<Unit> monsters, string displayPhrase)
    {
        _view.WriteLine(displayPhrase);
        char label = 'A';
        foreach (var monster in monsters)
        {
            if (monster.IsEmpty())
                _view.WriteLine($"{label}-");
            else
                _view.WriteLine($"{label}-{monster.Name} HP:{monster.Stats.Hp}/{monster.Stats.MaxHp} MP:{monster.Stats.Mp}/{monster.Stats.MaxMp}");
            label++;
        }
    }
    
    public void DisplayMonsterSelection(List<Unit> monsters, string displayPhrase, bool showOnlyDead = false, bool showAll = false)
    {
        _view.WriteLine(displayPhrase);
        char label = '1';
        foreach (var monster in monsters)
        {
            if (!showAll)
            {
                if ((monster.IsEmpty() || !monster.IsAlive()) && !showOnlyDead) continue;
                if (showOnlyDead && monster.IsAlive()) continue;
            }
            _view.WriteLine($"{label}-{monster.Name} HP:{monster.Stats.Hp}/{monster.Stats.MaxHp} MP:{monster.Stats.Mp}/{monster.Stats.MaxMp}");
            label++;
        }
        _view.WriteLine($"{label}-Cancelar");
    }
    
    public void DisplayHpMessage(Unit monster) 
        => _view.WriteLine($"{monster.Name} termina con HP:{monster.Stats.Hp}/{monster.Stats.MaxHp}");
    
    public void DisplayAttackMessage(CombatRecord combatRecord, Skill skill) 
        => _view.WriteLine($"{combatRecord.Attacker.Name} {GetAttackPhrase(skill.Type)} a {combatRecord.Target.Name}");

    
    public void DisplayAffinityDetectionMessage(CombatRecord combatRecord)
    {
        if (combatRecord.Affinity == AffinityType.Weak)
            _view.WriteLine($"{combatRecord.Target.Name} es débil contra el ataque de {combatRecord.Attacker.Name}");
        else if (combatRecord.Affinity == AffinityType.Resist)
            _view.WriteLine($"{combatRecord.Target.Name} es resistente el ataque de {combatRecord.Attacker.Name}");
    }

    public void DisplayInstantKillSkillResultMessage(CombatRecord combatRecord, bool skillHasMissed)
    {
        if (skillHasMissed)
        {
            _view.WriteLine($"{combatRecord.Attacker.Name} ha fallado el ataque");
        }
        else
        {
            if (combatRecord.Affinity is AffinityType.Neutral or AffinityType.Resist or AffinityType.Weak)
                _view.WriteLine($"{combatRecord.Target.Name} ha sido eliminado");
            else if (combatRecord.Affinity is AffinityType.Null)
                _view.WriteLine($"{combatRecord.Target.Name} bloquea el ataque de {combatRecord.Attacker.Name}");
            else
                throw new NotImplementedException($"Affinity type {combatRecord.Affinity} not implemented for Instant Kill Skill Result Message");
        }
    }

    public void DisplayAttackResultMessage(CombatRecord combatRecord)
    {
        var message = combatRecord.Affinity switch
        {
            AffinityType.Neutral or AffinityType.Weak or AffinityType.Resist => $"{combatRecord.Target.Name} recibe {combatRecord.Damage} de daño",
            AffinityType.Null => $"{combatRecord.Target.Name} bloquea el ataque de {combatRecord.Attacker.Name}",
            AffinityType.Repel => $"{combatRecord.Target.Name} devuelve {combatRecord.Damage} daño a {combatRecord.Attacker.Name}",
            AffinityType.Drain => $"{combatRecord.Target.Name} absorbe {combatRecord.Damage} daño",
            _ => throw new NotImplementedException("Affinity type not implemented for Attack Result Message")
        };
        _view.WriteLine(message);
    }

    private static string GetAttackPhrase(SkillType attackType)
    {
        return attackType switch
        {
            SkillType.Phys => "ataca",
            SkillType.Gun => "dispara",
            SkillType.Fire => "lanza fuego",
            SkillType.Ice => "lanza hielo",
            SkillType.Elec => "lanza electricidad",
            SkillType.Force => "lanza viento",
            SkillType.Light => "ataca con luz",
            SkillType.Dark => "ataca con oscuridad",
            _ => throw new NotImplementedException("Skill type not implemented for Attack Phrase")
        };
    }
    
    public void DisplaySkillSelection(Unit attacker)
    {
        _view.WriteLine($"Seleccione una habilidad para que {attacker.Name} use");
        int label = 1;
        foreach (var skill in attacker.Skills)
        {
            if (attacker.Stats.Mp < skill.Cost)
                continue;
            _view.WriteLine($"{label}-{skill.Name} MP:{skill.Cost}");
            label++;
        }
        _view.WriteLine($"{label}-Cancelar");
    }
}