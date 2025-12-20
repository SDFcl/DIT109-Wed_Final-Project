using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class boss : MonoBehaviour
{
    [Header("Boss status")]
    [SerializeField] private int bosshp = 50;
    public int currenthp;
    [SerializeField] private float BossSpeed = 1.5f;
    private Rigidbody2D rb;
    [SerializeField] private Transform midscreen;

    private bool movingLeft = true;
    public enum bossstate { Idle, Tackle, ultimate, FireBall, Fly }
    public bossstate Currentstage;
    [SerializeField] private int TouchCount = 0;
    

    [Header("Shootting")]
    [SerializeField] private float ShootRate = 2;
    [SerializeField] private float ShootCountdown = 0f;
    public GameObject Bullet;
    public Transform FirePoint;
    [Header("IdleState")]
    [SerializeField] private float Idlerate = 3f;
    [SerializeField] private float IdleCountdown = 0f;
    [Header("Ultimate")]
    [SerializeField] private int ToUltimate = 0;
    private int Ultimateround = 0;
    public GameObject UltimateBullet;

    [Header("Fly")]
    [SerializeField] private int ToFlydcount = 0;
    public Transform player;
    private int shooted = 0;
    private bool isDiving = false;
    [SerializeField] private int maxDiveCount = 3; // จำนวนครั้งที่ต้องการดิ่ง
    private int currentDiveCount = 0;
    private float bulletAngle = 0f;


    public GameObject bossskin;
    public Animator anim;
    public BoxCollider2D BoxCollider2DAnim;
    private void Start()
    {
        currenthp = bosshp;
        rb = GetComponent<Rigidbody2D>();
        Currentstage = bossstate.Idle;
        anim = bossskin.GetComponent<Animator>();
        Idlerate = 1f;
    }
    private void FixedUpdate()
    {

        ShootCountdown += Time.deltaTime;
        IdleCountdown += Time.deltaTime;
        switch (Currentstage)
        {
            case bossstate.Idle:
                anim.SetTrigger("Idle");
                if (IdleCountdown >= Idlerate) ChangeState(bossstate.Tackle);
                break;

            case bossstate.Tackle:
                anim.SetTrigger("Tackle");
                Idlerate = 1.5f;
                Tackle();
                break;

            case bossstate.FireBall:
                anim.SetTrigger("FireFlame");
                ShootRate = 0.6f;
                FireBall(6,Bullet);
                break;

            case bossstate.ultimate:
                Ultimate();
                break;

            case bossstate.Fly:
              FlyMove();
                break;
                
        }

    }
    void Tackle()
    {
        IdleCountdown = 0;
        float directionMultiplier = movingLeft ? -1 : 1;
        rb.velocity = new Vector2(5 * directionMultiplier, rb.velocity.y) * BossSpeed;


    }
    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Wall") && Currentstage == bossstate.Tackle)
        {
            BoxCollider2DAnim.enabled = false;
            TouchCount += 1;
            movingLeft = !movingLeft;
            Flip(!movingLeft);

            if(TouchCount>= 1)
            {
                ChangeState(bossstate.FireBall);
                ToUltimate++;
                if (ToUltimate >= 3)
                {
                    ChangeState(bossstate.ultimate);
                    ToUltimate = 0;
                    ToFlydcount++;
                    if (ToFlydcount >= 2)
                    {
                        ChangeState(bossstate.Fly);
                        ToFlydcount = 0;
                    }
                }
            }
           
            
        }
        


        

    }
    void Flip(bool toLeft)
    {

        if (toLeft)
        {

            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * -1, transform.localScale.y, transform.localScale.z);
        }
        else
        {

            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }
    void ChangeState(bossstate newState)
    {
      
        Currentstage = newState;
    }
    void FireBall(int MaxShot,GameObject Bullettype)
    {

        rb.velocity = Vector2.zero;

        if (ShootCountdown >= ShootRate)
        {

            GameObject newBullet = Instantiate(Bullettype, FirePoint.position, Quaternion.identity);

            // เช็คว่าบอสหันด้านไหนแล้วส่งค่าทิศทางไป
            Ball1 bulletScript = newBullet.GetComponent<Ball1>();
            bulletScript.SetDirection(movingLeft ? Vector2.left : Vector2.right);

            ShootCountdown = 0;
            shooted += 1;

            if (shooted == MaxShot)
            {
                shooted = 0;
                Idlerate = 5f;
                ChangeState(bossstate.Idle);
            }
        }

    }
    void BulletHell()
    {
        GameObject newBullet = Instantiate(UltimateBullet, FirePoint.position, Quaternion.identity);
        Ball2 bulletScript = newBullet.GetComponent<Ball2>();
        bulletScript.SetDirection(bulletAngle); // ส่งมุมให้กระสุน
        bulletAngle += 15f; // เพิ่มมุมเรื่อย ๆ เพื่อหมุนเป็นวง
    }
    void Ultimate()
    {
        anim.SetTrigger("Tackle");
        Vector2 middlepoint = midscreen.position - transform.position;
        rb.velocity = new Vector2(middlepoint.x,rb.velocity.y)*BossSpeed;
        if (Mathf.Abs(middlepoint.x) < 1f)
        {
            anim.SetTrigger("Ultimate");
            rb.velocity = Vector2.zero;
            ShootRate = 0.2f;
            if(ShootCountdown >= ShootRate)
            {
                BulletHell();
                ShootCountdown = 0;
                Ultimateround++;
                if (Ultimateround >= 30)
                {
                    anim.ResetTrigger("Ultimate");
                    Ultimateround = 0;
                    ToUltimate = 0;
                    Idlerate = 6f;
                    ChangeState(bossstate.Idle);
                }
            }
           
        }

    }

   
    void FlyMove()
    {
       
        Vector2 PlayerLoc = player.position - transform.position;

        if (!isDiving)
        {
            anim.SetTrigger("Fly");

            rb.velocity = new Vector2(0, 4f) * BossSpeed;
            
            if (transform.position.y > 4f)
            {
                Debug.Log("Yes");

                rb.velocity = new Vector2(PlayerLoc.x , rb.velocity.y) * 3f;

                if (Mathf.Abs(PlayerLoc.x) < 1f)
                {
                    
                    isDiving = true;
                    anim.ResetTrigger("Fly");
                    anim.SetTrigger("Eat");
                }
            }
        }
        else
        {
            
            rb.velocity = new Vector2(rb.velocity.x, -10f); // เร่งดิ่งลง
            if (transform.position.y < -2.856298f)
            {
                
                currentDiveCount++;

                if (currentDiveCount >= maxDiveCount)
                {
                    rb.velocity = Vector2.zero;
                    isDiving = false;
                    currentDiveCount = 0; // รีเซ็ต
                    Idlerate = 2f;
                    ChangeState(bossstate.Idle);
                   
                }
                else
                {
                    isDiving = false;
                }
            }
        }
    }
}
