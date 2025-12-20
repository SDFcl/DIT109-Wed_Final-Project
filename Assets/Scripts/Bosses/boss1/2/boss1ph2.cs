using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using static boss2;

public class boss2 : MonoBehaviour
{
    [Header("Boss status")]
    [SerializeField] private int bosshp = 50;
    public int currenthp;
    [SerializeField] private float BossSpeed = 1.5f;
    private Rigidbody2D rb;


    private bool movingLeft = true;
    public enum bossstate { Idle, Tackle, ultimate, FireBall, TailSwipe }
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
    [SerializeField] private int ToUltimate=0;
    public GameObject BulletUltimate;
    public Transform Player;

   
    private int shooted = 0;

    public GameObject bossskin;

    public Animator anim;
    private DamagalbleScript damagalblescript;
    private void Start()
    {
        damagalblescript = GetComponent<DamagalbleScript>();
        currenthp = bosshp;
        rb = GetComponent<Rigidbody2D>();
        Currentstage = bossstate.Idle;
        anim = bossskin.GetComponent<Animator>();
        Idlerate = 1.5f;
        
    }
    private void FixedUpdate()
    {
        if (!damagalblescript.IsAlive) 
        {
            Destroy(this);
        }
        ShootCountdown += Time.deltaTime;
        IdleCountdown+= Time.deltaTime;
        switch (Currentstage)
        {
            case bossstate.Idle:
                anim.SetTrigger("Idle");
                if (IdleCountdown >= Idlerate) ChangeState(bossstate.Tackle);
                break;
          
            case bossstate.Tackle:
                Idlerate = 1.5f;
                anim.SetTrigger("Tackle");
                Tackle();
                break;
           
            case bossstate.FireBall:

                anim.SetTrigger("FirePro");
                ShootRate = 1.5f;
                FireBall(bossstate.Idle, 10, Bullet,0.1f);
                break;
           
            case bossstate.ultimate:
                Ultimate();
                break;
           
            case bossstate.TailSwipe:
                TailSwipe();
               
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
       
        if (collision.gameObject.CompareTag("Wall")&& Currentstage == bossstate.Tackle)
        {
            
            TouchCount+=1;
            movingLeft = !movingLeft;
            Flip(!movingLeft);

            if (TouchCount == 5)
            {
                TouchCount = 0;
                ChangeState(bossstate.FireBall);
                ToUltimate++;

                if (ToUltimate == 3)
                {
                    ChangeState(bossstate.ultimate);
                }
                else if (ToUltimate == 4)
                {
                    ChangeState(bossstate.TailSwipe);
                    ToUltimate = 0;
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
    void FireBall(bossstate boss,int MaxShot,GameObject BulletType,float shootrate=0.2f)
    {
        rb.velocity = Vector2.zero;
        ShootRate = shootrate;
        
            if (ShootCountdown >= ShootRate)
        {
            Instantiate(BulletType, FirePoint.position, Quaternion.identity);
            ShootCountdown = 0;
            shooted += 1;

            if (shooted == MaxShot)
            {
                ChangeState(boss);
                shooted = 0;
               
            }
        }

    }
    void Ultimate()
    {
        anim.SetTrigger("Ultimate");
        ShootRate = 0.2f;
        Idlerate = 5f;
        FireBall(bossstate.Idle,5,BulletUltimate);
       
    }
   
    void TailSwipe()
    { 
        Vector2 Playerlocate = Player.position - transform.position;
        float directionMultiplier = movingLeft ? -1 : 1;
        rb.velocity = new Vector2(5 * directionMultiplier, rb.velocity.y) * BossSpeed;
        
        //rb.velocity = new Vector2(Playerlocate.x, rb.velocity.y)*BossSpeed;


        float distanceToPlayer = Vector2.Distance(transform.position, Player.position);
        
        if (distanceToPlayer <= 2.8f)
        {
            rb.velocity = Vector2.zero; // หยุดวิ่ง
            anim.SetTrigger("TailSwipe");

            rb.velocity = Vector2.zero;
            Idlerate = 2.2f;
            if(IdleCountdown >= Idlerate) ChangeState(bossstate.Idle);



        }
      
    }
}
