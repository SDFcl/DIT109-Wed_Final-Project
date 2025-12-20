using UnityEngine;
using UnityEngine.Tilemaps;

public class PlatformEffector2DRotation : MonoBehaviour
{
    private PlatformEffector2D pfe2d;
    [SerializeField] private PlayerController playerController;
    //private TilemapCollider2D bcol2d;
    private BoxCollider2D bcol2d;
    private float veloY;
    private bool isJumpingDown = false;
    private bool XKeyDown;

    private void Start()
    {
        pfe2d = GetComponent<PlatformEffector2D>();
        bcol2d = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        veloY = playerController.getVeloY();
        XKeyDown = playerController.IsJump;

        if (veloY > 0)
        {
            // ขึ้นจากล่าง: เปิด trigger ให้ทะลุ
            pfe2d.rotationalOffset = 0;
            bcol2d.isTrigger = true;
        }
        else if (veloY <= 0 && !XKeyDown)
        {
            // ยืนอยู่บนพื้น: ปิด trigger
            bcol2d.isTrigger = false;
        }

        // ถ้าผู้เล่นกดลง + กระโดดลง
        if (veloY <= 0 && XKeyDown && !isJumpingDown)
        {
            StartCoroutine(FlipPlatform());
        }
    }

    private System.Collections.IEnumerator FlipPlatform()
    {
        isJumpingDown = true;

        pfe2d.rotationalOffset = 180;
        yield return new WaitForSeconds(.5f); // เว้นช่วงให้ตัวละครผ่าน

        pfe2d.rotationalOffset = 0;
        isJumpingDown = false;
    }
}
