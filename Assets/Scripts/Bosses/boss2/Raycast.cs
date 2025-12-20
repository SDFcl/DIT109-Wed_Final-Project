using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raycast : MonoBehaviour
{
    Vector2 origin;
    Vector2 direction;
    float distance = 10f;
    LayerMask layerMask;

    RaycastHit2D hit;

    private void Start()
    {
        
    }

    private void FixedUpdate()
    {
        origin = transform.position;
        direction = transform.right;
        layerMask = LayerMask.GetMask("Boss");
        hit = Physics2D.Raycast(origin, direction, distance, layerMask);
        Debug.DrawLine(origin, origin + direction * distance, Color.red, 0.1f);
        if (hit.collider != null)
        {
            Debug.Log("Hit " + hit.collider.name);
        }
    }

    // ใช้ OnDrawGizmosSelected เพื่อให้เห็นเส้นเมื่อเลือกวัตถุใน Editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(origin, origin + direction * distance); // Draw the Raycast line
    }
}
