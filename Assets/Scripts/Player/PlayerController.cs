using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField]private GameManager gameManager;

    public StatsAll PlayerStats;
    private DamagalbleScript DamagalbleScript;
    private static Rigidbody2D rb2d;
    private CapsuleCollider2D col;
    [SerializeField] private Animator anim;

    [SerializeField] private GameObject Bullet;
    [SerializeField] private Transform BulletSpawnPoint;
    private GameObject BulletInst;

    [SerializeField]  private GameObject GunObj;
    private bool IsShoot;
    private float CDShoot;

    
    
    private Vector2 MoveInput;
    private bool IsMove;
    private bool IsDownKeyDown = false;
    private bool IsUpKeyDown = false;
    private bool _isReduceSize = false;
    public bool IsReduceSize
    {
        get { return _isReduceSize; }
        private set
        {
            _isReduceSize = value;
            if (_isReduceSize)
            {
                col.offset = new Vector2(0, -0.5f);
                col.size = new Vector2(1, 1);
                //ลดขนาด
            }
            else if (!_isReduceSize)
            {
                col.offset = new Vector2(0, -0.23f);
                col.size = new Vector2(0.65f, 1.5f);
            }
        }
    }

    
    [HideInInspector]public bool IsJump;

    
    private bool IsDash;
    private float DossTime;
    private float CDDoss;
    private float StopDoss;

    public bool IsGround = false;
    public ContactFilter2D filter2D;
    private RaycastHit2D[] groundHits = new RaycastHit2D[5];
    private float groundDistance = 0.15f;

    private bool IsLook = false;

    private bool _isFacingRight = true;
    public bool IsFacingRight
    {
        get
        {
            return _isFacingRight;
        }
        private set
        {
            if (_isFacingRight != value) { transform.localScale *= new Vector2(-1, 1); }
            _isFacingRight = value;
        }
    }

    [HideInInspector]public bool IsAlive;

    private int intGunRotation = 0;

    private bool IsDie=false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {

        rb2d = GetComponent<Rigidbody2D>();
        col = GetComponent<CapsuleCollider2D>();
        IsAlive = true;
        DamagalbleScript = GetComponent<DamagalbleScript>();
        //anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //rb2d.bodyType = RigidbodyType2D.Kinematic;
        IsGround = col.Cast(Vector2.down, filter2D, groundHits, groundDistance) > 0;
        if (!IsLook && !IsDownKeyDown)
        {
            rb2d.linearVelocity = new Vector2(MoveInput.x * PlayerStats.MoveSpeed, rb2d.linearVelocity.y);
        }
        else 
        {
            rb2d.linearVelocity = new Vector2(0f, rb2d.linearVelocity.y);
        }
        GunRotation();
        FlipPlayer();
        AnimSetBool();

        if (Time.time > DossTime && IsDash)
        {
            
            StopDoss = Time.time + 0.5f;

        }
        else if (Time.time <= DossTime)
        {
            int Layer = LayerMask.NameToLayer("IgnoreBossHitBox");
            gameObject.layer = Layer;
            rb2d.linearVelocity = new Vector2(PlayerStats.DashPower * transform.localScale.x * 5, 0f);
            
        }
        if (Time.time > DossTime && !IsDash)
        {
            int Layer = LayerMask.NameToLayer("Player");
            gameObject.layer = Layer;

        }
        if (StopDoss >= Time.time)
        {
            IsDash = false;
        }

        if (IsShoot) {Attacking(true); }
        else if (!IsShoot) {Attacking(false); }


        if (!DamagalbleScript.IsAlive && !IsDie) 
        {
            Debug.Log("Your Die");
            anim.SetTrigger("IsAlive");
            IsDie = true;
            //Destroy(this);
        }
    }

    private void AnimSetBool() //เมื่อกด C อยู่
    {
        anim.SetBool("IsMove", IsMove);
        anim.SetBool("CKeyDown", IsLook);
        anim.SetBool("XKeyDown", IsShoot);
        anim.SetBool("IsGround", IsGround);
        //jump อยู่ onjump
        anim.SetFloat("YVelocity", rb2d.linearVelocityY);
        if (intGunRotation < 0) { intGunRotation *= -1; }
        anim.SetFloat("GunRotation", intGunRotation);
        anim.SetBool("DownKeyDown", IsDownKeyDown);

    }

    private void GunRotation()
    {
        if (IsUpKeyDown && !IsMove)
        {
            if (IsFacingRight)
            {
                GunObj.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
                intGunRotation = 90;
            }
            if (!IsFacingRight)
            {
                GunObj.transform.rotation = Quaternion.Euler(0f, 0f, -90f);
                intGunRotation = -90;
            }
        }
        if (IsUpKeyDown && IsMove)
        {

            //Animation 45 Gun
            if (IsFacingRight)
            {
                GunObj.transform.rotation = Quaternion.Euler(0f, 0f, 45f);
                intGunRotation = 45;
            }
            if (!IsFacingRight)
            {
                GunObj.transform.rotation = Quaternion.Euler(0f, 0f, -45f);
                intGunRotation = -45;
            }
        }
        if ((!IsUpKeyDown && IsMove) || (!IsUpKeyDown && !IsMove))
        {
            //Animation normal
            GunObj.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            intGunRotation = 0;
        }
    }

    private void FlipPlayer()
    {
        if (MoveInput.x > 0 && !IsFacingRight) { IsFacingRight = true; }
        else if (MoveInput.x < 0 && IsFacingRight) { IsFacingRight = false; }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started) { IsShoot = true; }
        if (context.canceled) { IsShoot = false; }
    }
    public void Attacking(bool IsStart)
    {
        if (CDShoot <= Time.time && IsStart)
        {
            CDShoot = Time.time + PlayerStats.fiverate;
            BulletInst = Instantiate(Bullet, BulletSpawnPoint.position, GunObj.transform.rotation, null);
        }
    }



    public void OnUp(InputAction.CallbackContext context)
    {
        if (context.started) { IsUpKeyDown = true; }
        if (context.canceled) { IsUpKeyDown = false; }
    }

    public void OnDown(InputAction.CallbackContext context)
    {
        if (context.performed && IsGround) { IsDownKeyDown = true; IsReduceSize = true; }
        if (context.canceled) { IsDownKeyDown = false; IsReduceSize = false; }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!IsDash)
        {
            MoveInput = context.ReadValue<Vector2>();
            IsMove = MoveInput != Vector2.zero;
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        if (context.started) { IsLook = true; }
        if (context.canceled) { IsLook = false; }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.started) 
        {
            if (CDDoss <= Time.time)
            {
                IsDash = true;
                anim.SetTrigger("Dash");
                CDDoss = Time.time + 0.5f;
                DossTime = Time.time + 0.4f;
            }
        }
    }

    public void OnESC(InputAction.CallbackContext context)
    {
        gameManager.OnESC(context);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && IsGround && !IsDownKeyDown && rb2d.linearVelocityY <= 0)
        {
            if (IsDownKeyDown) { IsReduceSize = true; }
            rb2d.linearVelocity = new Vector2(rb2d.linearVelocity.x, PlayerStats.JumpPower);
            anim.SetTrigger("Jump");
            IsGround = false;
        }
        if (context.started && IsGround && IsDownKeyDown) { IsJump = true;  }
        if (context.canceled) { IsJump = false; }
    }
    /*private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            IsGround = true;
            if (IsReduceSize && !IsDownKeyDown) { IsReduceSize = false; }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Ground"))
        {
            IsGround = false;
        }
    }*/

    public float getVeloY()
    {
        return rb2d.linearVelocity.y;
    }
}
