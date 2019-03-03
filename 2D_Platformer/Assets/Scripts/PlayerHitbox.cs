using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitbox : MonoBehaviour
{
    Player player;
    private void Start()
    {
        player = GetComponentInParent<Player>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            collision.GetComponent<BaseEnemy>().TakeDamage(player.damage);
            player.Knockback(player.transform.localScale*-1, player.swordKnockback);

        }
    }
}
