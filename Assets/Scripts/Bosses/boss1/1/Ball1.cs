using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball1 : MonoBehaviour
{
    [SerializeField] private float speed;
    public int damage = 20;
    private Rigidbody2D rb;

    private Vector2 moveDirection = Vector2.left; // ค่า default

    public void SetDirection(Vector2 direction)
    {
        moveDirection = direction.normalized;

        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        rb.velocity = moveDirection * speed;

    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            // ให้เด้งขึ้น โดยคงทิศทางแนวนอนเดิมและความเร็วคงที่
            rb.velocity = new Vector2(moveDirection.x * speed, 6f);
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }

}
