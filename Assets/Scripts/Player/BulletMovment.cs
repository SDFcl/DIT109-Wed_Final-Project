using UnityEngine;

public class BulletMovment : MonoBehaviour
{
    private Transform Gun;
    private Transform Player;
    private Rigidbody2D rb2d;
    private Vector2 Direction;
    public float MoveSpeed = 10;

    private float ScalePlayer = 1;

    [Header("Hack Damage")]
    public int Damage = 1;

    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        Gun = GameObject.FindGameObjectWithTag("Gun").transform;
        rb2d = GetComponent<Rigidbody2D>();
        Direction = transform.right;
        if (Player.transform.localScale.x < 0) { transform.localScale *= new Vector2(-1, 1); ScalePlayer = -1; }
    }

    private void Update()
    {
        rb2d.linearVelocity = Direction * MoveSpeed * ScalePlayer;
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Boss"))
        {
            DamagalbleScript damagalbleScript = collision.GetComponent<DamagalbleScript>();
            if (damagalbleScript != null)
            { 
                damagalbleScript.HIT(Damage);
                Destroy(gameObject);
            }
        }
        if (collision.gameObject.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
