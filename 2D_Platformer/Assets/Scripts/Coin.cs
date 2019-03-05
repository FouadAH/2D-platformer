using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private float upForce = 3f;
    [SerializeField] private float sideForce = 3f;
    [SerializeField] private int value = 1;

    void Start()
    {
        float xForce = Random.Range(sideForce,-sideForce);
        float yForce = Random.Range(upForce/2f, upForce);
        GetComponent<Rigidbody2D>().velocity = new Vector2(xForce, yForce);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Player")
        {
            GameManager.instance.currency += value;
            Destroy(gameObject);
        }
    }

}
