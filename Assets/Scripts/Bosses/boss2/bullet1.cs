using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet1 : MonoBehaviour
{
    private Rigidbody2D rb;
     public float speed=10f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.velocity = -transform.right*speed;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            DamagalbleScript damagalbleScript = collision.GetComponent<DamagalbleScript>();
            if (damagalbleScript != null)
            {
                damagalbleScript.HIT(1);
                Destroy(gameObject);
            }
        }
        if (collision.gameObject.CompareTag("Wall")) Destroy(gameObject);
    }
}
