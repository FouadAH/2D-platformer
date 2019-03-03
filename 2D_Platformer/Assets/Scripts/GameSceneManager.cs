using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager instance { get; private set; }
    public GameObject player;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public float health = 100;
    public float maxHealth = 100;
    public int currency;
    public Vector2 lastCheckpointPosition;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(lastCheckpointPosition, 2);
    }
}
