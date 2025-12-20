using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet2 : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private float speed = 5f;
    public Transform CenterofVoid;
    Vector2 direction;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        CenterofVoid = GameObject.Find("Main").transform;
        Vector2 direction = ((Vector2)CenterofVoid.position - (Vector2)transform.position).normalized;
        rb.velocity = direction * speed;
    }

    private void Update()
    {
        transform.localScale -= new Vector3(0.6f,0.6f, 0f)*Time.deltaTime;

        if (Vector2.Distance(transform.position, CenterofVoid.position) < 0.5f)
        {
            Destroy(gameObject);
        }
    }
    public void biggerball()
    {
        transform.localScale = new Vector3(1,1,1);
    }
    
}
