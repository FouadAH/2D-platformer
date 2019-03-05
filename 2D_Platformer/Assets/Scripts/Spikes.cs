using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour {

    private Player player;
    public float damageDealt;
    private GameManager gm;
    private void Awake()
    {
        gm = FindObjectOfType<GameManager>();
    }
    private void Start()
    {
        player = gm.player.GetComponent<Player>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            player.DealDamage(damageDealt, Vector3.Normalize(player.transform.position - transform.position));
            player.transform.position = GameManager.instance.lastCheckpointPos;
        }
    }
}
