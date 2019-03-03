using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour {

    private Player player;
    public float damageDealt;
    private GameSceneManager gm;
    private void Awake()
    {
        gm = FindObjectOfType<GameSceneManager>();
    }
    private void Start()
    {
        player = gm.player.GetComponent<Player>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            player.DealDamage(damageDealt);
            player.transform.position = GameSceneManager.instance.lastCheckpointPosition;
        }
    }
}
