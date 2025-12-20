using UnityEngine;

public class StatsAll : MonoBehaviour
{
    [Header("All")]
    public int HP = 3;

    [Header("Player")]
    public float FiveRate = 100;
    [HideInInspector]public float fiverate;
    public float MoveSpeed = 2f;
    public float JumpPower = 10f;
    public float DashPower = 10f;

    private void Update()
    {
        fiverate = 0.1f / (FiveRate/100);
    }
}
