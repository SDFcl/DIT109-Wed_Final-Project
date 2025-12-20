using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet3 : MonoBehaviour
{
    public float growSpeed = 0.5f; // ความเร็วในการขยาย
    public float maxSize = 3f; // ขนาดสูงสุดก่อนหยุดขยาย

    void Update()
    {
        if (transform.localScale.x < maxSize)
        {
            transform.localScale += new Vector3(0.5f, 0.5f, 0.5f) * growSpeed * Time.deltaTime;
        }
    }
}
