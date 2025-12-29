using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject Player;
    [SerializeField] private GameObject Boss1;
    [SerializeField] private GameObject Boss2;
    private bool IsBossPh2 = false;

    private DamagalbleScript damagalbleScriptPlayer;
    private PlayerController playerController;
    private PlayerInput playerInput;
    private DamagalbleScript damagalbleScriptBoss1;
    private DamagalbleScript damagalbleScriptBoss2;
    private boss2 boss2Script;
    private boss bossScript;


    [SerializeField] private GameObject BlackImage;
    [SerializeField] private GameObject ChangeBoss;
    [SerializeField] private GameObject UiWin;
    [SerializeField] private GameObject UiLost;

    [SerializeField] private GameObject UiPausegame;

    private bool IsEnd;

    public float fadeTime = 2f;
    private float timeElapsed = 0f;
    private Image image;
    private Color startColor;

    private bool canESC = true;

    private void Awake()
    {
        playerController = Player.GetComponent<PlayerController>();
        playerInput = Player.GetComponent<PlayerInput>();
        bossScript = Boss1.GetComponent<boss>();
        boss2Script = Boss2.GetComponent<boss2>();
        Time.timeScale = 1f;
        IsEnd = false;
        if (Player != null)
        {
            damagalbleScriptPlayer = Player.GetComponent<DamagalbleScript>();
        }
        if (Boss1 != null)
        {
            damagalbleScriptBoss1 = Boss1.GetComponent<DamagalbleScript>();
        }
        if (Boss2 != null)
        {
            damagalbleScriptBoss2 = Boss2.GetComponent<DamagalbleScript>();
        }

        BlackImage.SetActive(false);
        ChangeBoss.SetActive(false);
        UiWin.SetActive(false);
        UiLost.SetActive(false);
        UiPausegame.SetActive(false);
        playerController.enabled = true;
        playerInput.enabled = true;
        boss2Script.enabled = false;

        //Fade Boss1 to Boss2
        timeElapsed = 0f;
        image = ChangeBoss.GetComponent<Image>();
        startColor = image.color;
        Time.timeScale = 1.5f;
    }

    private void Update()
    {
        if (!damagalbleScriptBoss1.IsAlive && !IsBossPh2)
        {
            canESC = false;
            ChangeBoss.SetActive(true);
            bossScript.enabled = false;
            Time.timeScale = 0.5f;
            FadeLod(true, fadeTime);
            StartCoroutine(WaitA(fadeTime, 2));
        }


        if (!damagalbleScriptBoss2.IsAlive && !IsEnd)
        {
            BlackImage.SetActive(true);
            UiWin.SetActive(true);
            SlowGameToFinish();
        }

        if (!damagalbleScriptPlayer.IsAlive && !IsEnd)
        {
            BlackImage.SetActive(true);
            UiLost.SetActive(true);
            SlowGameToFinish();
        }

        
    }

    public void FadeLod(bool FadeIn, float fadeTime)
    {
        timeElapsed += Time.deltaTime;
        if (FadeIn)
        { 
            float newAlpha = startColor.a + (timeElapsed / fadeTime);
            image.color = new Color(startColor.r, startColor.g, startColor.b, newAlpha);
        }
        if (!FadeIn)
        {
            float newAlpha = startColor.a * (1 - (timeElapsed / fadeTime)); 
            image.color = new Color(startColor.r, startColor.g, startColor.b, newAlpha);
        }
    }

    public void SlowGameToFinish()
    {
        Time.timeScale = 0.5f;
        StartCoroutine(WaitA(1,1));
    }


    public void OnESC(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            BlackImage.SetActive(playerController.enabled);
            UiPausegame.SetActive(playerController.enabled);
            EnableplayerCon();
            EnableBossScript();
            if (playerController.enabled == false)
            {
                Time.timeScale = 0f;
            }
            else 
            {
                Time.timeScale = 1.5f;
            }
        }
    }

    public void EnableplayerCon()
    {
        playerController.enabled = !playerController.enabled;
        //playerInput.enabled =!playerInput.enabled;
    }
    public void EnableBossScript()
    {
        if (bossScript != null) 
        {
            bossScript.enabled = !bossScript.enabled;
        }
        if (boss2Script != null && IsBossPh2)
        {
            boss2Script.enabled = !boss2Script.enabled;
        }
    }

    private System.Collections.IEnumerator WaitA(float Seconds, int numderwait)
    {
        switch (numderwait)
        {
            case 1:
                yield return new WaitForSeconds(Seconds);
                IsEnd = true;

                EnableplayerCon();
                Time.timeScale = 0f;
                break;
            case 2:
                yield return new WaitForSeconds(Seconds); 
                IsBossPh2 = true;
                boss2Script.enabled = true;
                Boss2.transform.position = new Vector3(6.01187515f, -2.8599999f, -0.0154792015f);
                Boss1.transform.position = new Vector3(22.8741856f, -2.89922571f, 0f);
                timeElapsed = 0f;
                FadeLod(false, 1);
                Destroy(bossScript);
                Time.timeScale = 1.5f;
                canESC = true;
                break;
        }
        
    }

    public void Savedata()
    {
        ApiManager apiManager = FindAnyObjectByType<ApiManager>();
        if (apiManager != null)
        {
            apiManager.SavePlayerData();
        }
    }
}
