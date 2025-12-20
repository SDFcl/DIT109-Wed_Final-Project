using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Ball2 : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private float speed = 3f;
    private Vector2 direction;

    public void SetDirection(float angleDeg)
    {
        float angleRad = angleDeg * Mathf.Deg2Rad;
        direction = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad)).normalized;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = direction * speed;
    }
   
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
        
    }
}
