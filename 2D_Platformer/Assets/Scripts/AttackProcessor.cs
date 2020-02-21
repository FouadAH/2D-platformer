using UnityEngine;

public class AttackProcessor
{
    public void ProcessMelee(IBaseStats attacker, IDamagable target, int knockbackDirX, int knockbackDirY)
    {
        int amount = CalculateAttackAmountMelee(attacker);
        ProcessAttack(target, amount);
        ProcessKnockbackOnHit(attacker, knockbackDirX, knockbackDirY);
        ProcessKnockbackOnDamage(target, -knockbackDirX,  -knockbackDirY);
    }

    public void ProcessRanged(ILauncher attacker, IDamagable target)
    {
        int amount = CalculateAttackAmountRanged(attacker);
        ProcessAttack(target, amount);
    }
    
    private int CalculateAttackAmountMelee(IBaseStats attacker)
    {
        return (int)(Random.Range(attacker.MinMeleeDamage, attacker.MaxMeleeDamage + 1) * attacker.MeleeAttackMod);
    }

    private int CalculateAttackAmountRanged(ILauncher attacker)
    {
        return (int)(Random.Range(attacker.MinRangeDamage, attacker.MaxRangeDamage + 1) * attacker.RangedAttackMod);
    }

    private void ProcessAttack(IDamagable target, int amount)
    {
        target.ModifyHealth(amount);
    }

    private void ProcessKnockbackOnHit(IBaseStats attacker, int knockbackDirX, int knockbackDirY)
    {
        attacker.KnockbackOnHit(attacker.HitKnockbackAmount, knockbackDirX, knockbackDirY);
    }

    private void ProcessKnockbackOnDamage(IDamagable target, int knockbackDirX, int knockbackDirY)
    {
        target.KnockbackOnDamage(target.knockbackGiven, knockbackDirX, knockbackDirY);
    }
}
