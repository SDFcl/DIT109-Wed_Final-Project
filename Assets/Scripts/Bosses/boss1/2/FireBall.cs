using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float delayBeforeMove = 1f;  // เพิ่มตัวแปรหน่วงเวลาเคลื่อน
    private Rigidbody2D rb;
    private GameObject player;
    private bool isMoving = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");

        StartCoroutine(DelayAndMove());
    }

    IEnumerator DelayAndMove()
    {
        yield return new WaitForSeconds(delayBeforeMove);

        if (player != null)
        {
            Vector3 direction = (player.transform.position - transform.position).normalized;
            rb.velocity = new Vector2(direction.x, direction.y) * speed;
            isMoving = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            DamagalbleScript damagalbleScript = collision.gameObject.GetComponent<DamagalbleScript>();
            damagalbleScript.HIT(1);
        }
        if (collision.gameObject.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}

