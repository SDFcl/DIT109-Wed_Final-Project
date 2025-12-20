using UnityEngine;

public class BossDoDamaga : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            DamagalbleScript damagalbleScript = collision.GetComponent<DamagalbleScript>();
            damagalbleScript.HIT(1);
        }
    }
}
