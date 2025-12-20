using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class Boss2 : MonoBehaviour
{
    [Header("BossStatus")]
    [SerializeField] private int bossHp = 100;
    [SerializeField] private DamagalbleScript damagalbleScript;
    [SerializeField] public int bosscurrentHp;
    [Header("Bulletcooldown")]
    [SerializeField] private float TonextStage = 3;
    [SerializeField] private float Nextstagecoundown = 0;
    public enum BossState { Idle, SlapDown, ChargeClap, Clap, Atomicbreath,ChargeBeam,LowHp,AllForOne }
    public BossState currentState;

    [Header("Gameobj")]
    public GameObject Bullet;

    public Transform Firepoint;

    [Header("Animm")]
    public GameObject bossskin;
    public Animator anim;

    [Header("BulletStatus")]
    [SerializeField] private float Firerate = 2;
    [SerializeField] private float FireCountdown = 0;
    [SerializeField] private int counbullet;
    
    [Header("ChargeUltimate")]
    public GameObject BulletCharge;
    public GameObject BulletChargeBall;
    public Transform[] FirePointChrge;

   
    private void Start()
    {
        bosscurrentHp = bossHp;
        Changestage(BossState.Idle);
        anim = bossskin.GetComponent<Animator>();
        Time.timeScale = 1.5f;
        TonextStage = 1.5f;
       

    }
    [SerializeField]private float countdownAllforone=0;
    private void Update()
    {
        FireCountdown += Time.deltaTime;
        Nextstagecoundown += Time.deltaTime;
        countdownAllforone += Time.deltaTime;
        ResetAllTriggers();
        switch (currentState)
        {
            case BossState.Idle:
                Destroy(currentBigBullet);
                anim.SetTrigger("Idle");

                if (Nextstagecoundown >= TonextStage)
                {
                    if (ToAtomicfate > 1) Changestage(BossState.ChargeBeam);
                    else Changestage(BossState.SlapDown);

                }
                if (damagalbleScript.NowHp < 50) Changestage(BossState.LowHp);

                break;
            case BossState.SlapDown:
                anim.SetTrigger("SlapDown");
                Slapdownstate(60,0.18f);
                break;

            case BossState.ChargeClap:
               
                anim.SetTrigger("Charge");
                ChargeUltimate(0.1f, true,false, 40,BossState.Clap);
                break;
            case BossState.Clap:
              
                anim.SetTrigger("Clap");
                ExplodeBall(0.05f, 60);
                
                break;
            case BossState.ChargeBeam:
               
                anim.SetTrigger("ChargeBeam");
                ChargeUltimate(0.1f, true,false, 50,BossState.Atomicbreath);
                break;

            case BossState.Atomicbreath:
                ClearBullet(chargeBullets);
                anim.SetTrigger("Atomic");
                AtomicBeam(0.01f,130,50);
                break;
            case  BossState.LowHp:
                {
                    TonextStage = 2f;
                    anim.SetTrigger("IdleLowHp");
                   if (Nextstagecoundown >= TonextStage) Changestage(BossState.AllForOne);
                } 
                break;
            case BossState.AllForOne:
                {
                    anim.SetTrigger("AllForOne");
                    if (countdownAllforone >= 0.04f)
                    {
                        FireBullet(Bullet, Firepoint, Firepoint.rotation,12);
                        countdownAllforone = 0;
                    }
                    ChargeUltimate(0.5f, true,false, 10000, BossState.AllForOne);
                   
                }
                break;
        }
   
    }
    void FireBullet(GameObject Bullettype, Transform Firepostion, Quaternion Rotation,float speed=6)
    {
        GameObject newBullet = Instantiate(Bullettype, Firepostion.position, Rotation);
        bullet1 bulletScript = newBullet.GetComponent<bullet1>();
        if (bulletScript != null)
        {
            bulletScript.speed = speed; // กระสุนทั่วไป
        }
    }
     void Changestage(BossState boss)
    {
        currentState = boss;
        Nextstagecoundown = 0;
        FireCountdown = 0;
        counbullet = 0;

    }
    void ShootProjectile(float Rateset)
    {
        Firerate = Rateset;
        if (FireCountdown >= Rateset)
        {
            FireBullet(Bullet, Firepoint,Quaternion.identity);
            FireCountdown = 0;
            counbullet++;
        }
    }
    void Slapdownstate(int reach,float firerate)
    {
        
        ShootProjectile(firerate);

        if (counbullet >= reach)
        {
            Changestage(BossState.ChargeClap);

        }
    }
    private List<GameObject> chargeBullets = new List<GameObject>();
    private List<GameObject> Bloodbullet = new List<GameObject>();

    void ChargeUltimate(float rate,bool HasClapornot,bool firepojectie,int reachbullet,BossState bossstages)
    {
       
        Firerate = rate;
        if (FireCountdown >= Firerate)
        {
            //if (firepojectie) Instantiate(Bullet, Firepoint.position, Firepoint.rotation);
            FireCountdown = 0;
            counbullet++;
            if (counbullet >= reachbullet) Changestage(bossstages);

            for (int i = 0; i < FirePointChrge.Length; i++)
            {
                GameObject newBullet = Instantiate(BulletCharge, FirePointChrge[i].position, FirePointChrge[i].rotation);
                chargeBullets.Add(newBullet);

            }
            if (HasClapornot) ClapBall();
           

        }

    }
    private GameObject currentBigBullet;
    void ClapBall()
    {
        if (currentBigBullet == null)
        {
            currentBigBullet = Instantiate(BulletChargeBall, Firepoint.position, Quaternion.identity);
            currentBigBullet.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        }
    }
    [SerializeField] private int ToAtomicfate=0;
    
    void ExplodeBall(float Rateset, int Round)
    {
       
        Destroy(currentBigBullet);
        ClearBullet(chargeBullets);
        chargeBullets.Clear();

        Firerate = Rateset;
        
        if (FireCountdown >= Rateset)
        {
            GameObject newBullet = Instantiate(Bullet, Firepoint.position, Firepoint.rotation);
            Bloodbullet.Add(newBullet);
            FireCountdown = 0;
            counbullet++;
        }
        if (counbullet >= Round)
        {
            ToAtomicfate++;
            Changestage(BossState.Idle);

        }



    }
    void ClearBullet(List<GameObject> bullets)
    {
        foreach (GameObject bullet in bullets)
        {
            if (bullet != null) Destroy(bullet);
        }
    }

     void AtomicBeam(float rate,int Ballcount, float speed=60)
    {

        GameObject newBullet;
        Destroy(currentBigBullet);
        ClearBullet(chargeBullets);
        ClearBullet(Bloodbullet);
        ToAtomicfate = 0;
        //anim.SetTrigger("Atomic");
        Firerate = rate;
        if (FireCountdown >= Firerate)
        {

            newBullet = Instantiate(Bullet, Firepoint.position, Firepoint.rotation);
            bullet1 bulletScript = newBullet.GetComponent<bullet1>();
           
            if (bulletScript != null)
            {
                bulletScript.speed = speed;
            }
            counbullet++;
            if(counbullet >= Ballcount)
            {

                bulletScript.speed = 6;
                Changestage(BossState.SlapDown);
            }
        }
        

    }

    void ResetAllTriggers()
    {
        anim.ResetTrigger("Idle");
        anim.ResetTrigger("SlapDown");
        anim.ResetTrigger("Charge");
        anim.ResetTrigger("Clap");
        anim.ResetTrigger("Atomic");
        anim.ResetTrigger("ChargeBeam");
        anim.ResetTrigger("AllForOne");
        anim.ResetTrigger("IdleLowHp");
    }
}
