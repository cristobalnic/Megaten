using Shin_Megami_Tensei_View;
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
        _view.WriteLine($"Ronda de {turnPlayer.Samurai?.Name} (J{turnPlayer.Id})");
    }

    public void DisplayPlayersTables(List<Player> players)
    {
        _view.WriteLine(Params.Separator);
        foreach (var player in players)
        {
            var phrase = $"Equipo de {player.Table.Samurai?.Name} (J{player.Id})";
            DisplayPlayerTable(player.Table.Monsters, phrase);
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
    
    public void DisplayMonsterSelection(List<Unit> monsters, string displayPhrase)
    {
        _view.WriteLine(displayPhrase);
        char label = '1';
        foreach (var monster in monsters)
        {
            if (monster.IsEmpty() || !monster.IsAlive()) continue;
            _view.WriteLine($"{label}-{monster.Name} HP:{monster.Stats.Hp}/{monster.Stats.MaxHp} MP:{monster.Stats.Mp}/{monster.Stats.MaxMp}");
            label++;
        }
        _view.WriteLine($"{label}-Cancelar");
    }
    
    public void DisplayHpMessage(Unit monster) 
        => _view.WriteLine($"{monster.Name} termina con HP:{monster.Stats.Hp}/{monster.Stats.MaxHp}");
    
    public void DisplayAttackMessage(Unit attacker, Skill skill, Unit target) 
        => _view.WriteLine($"{attacker.Name} {GetAttackPhrase(skill.Type)} a {target.Name}");

    
    public void DisplayAffinityDetectionMessage(Unit attacker, Unit target, AffinityType affinityType)
    {
        if (affinityType == AffinityType.Weak)
            _view.WriteLine($"{target.Name} es débil contra el ataque de {attacker.Name}");
        else if (affinityType == AffinityType.Resist)
            _view.WriteLine($"{target.Name} es resistente el ataque de {attacker.Name}");
    }

    public void DisplayInstantKillSkillResultMessage(Unit attacker, Unit target, AffinityType targetAffinity, bool skillHasMissed)
    {
        if (skillHasMissed)
        {
            _view.WriteLine($"{attacker.Name} ha fallado el ataque");
        }
        else
        {
            if (targetAffinity is AffinityType.Neutral or AffinityType.Resist or AffinityType.Weak)
                _view.WriteLine($"{target.Name} ha sido eliminado");
            else if (targetAffinity is AffinityType.Null)
                _view.WriteLine($"{target.Name} bloquea el ataque de {attacker.Name}");
            else
                throw new NotImplementedException($"Affinity type {targetAffinity} not implemented for Instant Kill Skill Result Message");
        }
    }

    public void DisplayAttackResultMessage(Unit attacker, int damage, Unit target, AffinityType affinityType)
    {
        var message = affinityType switch
        {
            AffinityType.Neutral or AffinityType.Weak or AffinityType.Resist => $"{target.Name} recibe {damage} de daño",
            AffinityType.Null => $"{target.Name} bloquea el ataque de {attacker.Name}",
            AffinityType.Repel => $"{target.Name} devuelve {damage} daño a {attacker.Name}",
            AffinityType.Drain => $"{target.Name} absorbe {damage} daño",
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