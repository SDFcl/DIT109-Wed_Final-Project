using Unity.VisualScripting;
using UnityEngine;

public class ShootInTutorial : MonoBehaviour
{
    [SerializeField] private GameObject ball;
    private GameObject BulletInst;

    private bool IsShoot;
    private float CDShoot;

    private void Update()
    {
        if (CDShoot <= Time.time)
        {
            CDShoot = Time.time + 1;
            BulletInst = Instantiate(ball, transform.position, transform.rotation, null);
        }
    }
}
