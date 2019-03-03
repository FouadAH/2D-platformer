using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitBox : MonoBehaviour
{
    Player player;
    public Vector2 knockback;
    public float damageDealt;
    private GameSceneManager gm;

    private void Awake()
    {
        gm = FindObjectOfType<GameSceneManager>();
    }
    void Start()
    {
        player = gm.player.GetComponent<Player>();
    }

    private void OnTriggerEnter2D (Collider2D collision)
    {
        if (collision.isTrigger != true && collision.tag == "Player")
        {
            player.Knockback(Vector3.Normalize(player.transform.position - transform.position), knockback);
            player.DealDamage(damageDealt);

        }
    }
}
