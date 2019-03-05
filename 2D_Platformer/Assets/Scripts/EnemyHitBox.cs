using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitBox : MonoBehaviour
{
    Player player;
    public Vector2 knockback;
    public float damageDealt;
    private GameManager gm;

    private void Awake()
    {
        gm = FindObjectOfType<GameManager>();
    }
    void Start()
    {
        player = gm.player.GetComponent<Player>();
    }

    private void OnTriggerEnter2D (Collider2D collision)
    {
        if (collision.isTrigger != true && collision.tag == "Player")
        {
            player.DealDamage(damageDealt, Vector3.Normalize(player.transform.position - transform.position));
        }
    }
}
