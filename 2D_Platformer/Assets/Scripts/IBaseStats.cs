public interface IBaseStats : IDamagable
{
    int MinMeleeDamage { get; set; }
    int MaxMeleeDamage { get; set; }
    float MeleeAttackMod { get; set; }

    float RangedAttackMod { get; set; }
    int BaseRangeDamage { get; set; }
   
    int HitKnockbackAmount { get; set; }
    void KnockbackOnHit(int amount, int dirX, int dirY);
}
