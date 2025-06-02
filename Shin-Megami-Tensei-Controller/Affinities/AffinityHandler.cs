using Shin_Megami_Tensei.DataStructures;
using Shin_Megami_Tensei.Entities;
using Shin_Megami_Tensei.GameActions.AttackActions;
using Shin_Megami_Tensei.GameData;

namespace Shin_Megami_Tensei.Affinities;

public abstract class AffinityHandler
{
    public abstract double ApplyDamageModifier(double baseDamage);
    
    public abstract void UseTurns(TurnState turnState);
    
    public virtual Unit GetDamagedUnit(CombatRecord combatRecord) 
        => combatRecord.Target;
    
    public virtual void DealDamageByAffinityRules(CombatRecord combatRecord) 
        => AttackUtils.ApplyDamage(combatRecord.Target, combatRecord.Damage);
    
    public virtual double GetDamageByAffinityRules(double baseDamage) 
        => baseDamage;
    
    public abstract string GetAttackResultMessage(CombatRecord combatRecord);
    
    public abstract string GetInstantKillResultMessage(CombatRecord combatRecord);
    
    public virtual string GetAffinityDetectionMessage(CombatRecord combatRecord)
    {
        throw new NotImplementedException($"{combatRecord.Affinity} affinity does not have a detection message");
    }

    public virtual bool HasInstantKillSkillMissed(CombatRecord combatRecord, Skill skill)
    {
        return false;
    }

    public virtual void ExecuteInstantKillByAffinityRules(CombatRecord combatRecord)
    {
    }
}

