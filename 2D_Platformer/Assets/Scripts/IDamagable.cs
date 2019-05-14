using UnityEngine;

public interface IDamagable
{
    float Health { get; set; }
    int MaxHealth { get; set; }
    void ModifyHealth(int amount);

    int knockbackGiven { get; set; }
    void KnockbackOnDamage(int amount, int dirX, int dirY);
}
