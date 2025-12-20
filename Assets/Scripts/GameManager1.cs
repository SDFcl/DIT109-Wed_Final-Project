using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameManager1 : MonoBehaviour
{
    [SerializeField] private GameObject Player;
    [SerializeField] private GameObject Boss1;

    private DamagalbleScript damagalbleScriptPlayer;
    private PlayerController playerController;
    private PlayerInput playerInput;
    private DamagalbleScript damagalbleScriptBoss1;
    [SerializeField]private Boss2 bossScript;


    [SerializeField] private GameObject BlackImage;
    [SerializeField] private GameObject ChangeBoss;
    [SerializeField] private GameObject UiWin;
    [SerializeField] private GameObject UiLost;
    [SerializeField] private GameObject HpText;

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

        BlackImage.SetActive(false);
        ChangeBoss.SetActive(false);
        UiWin.SetActive(false);
        UiLost.SetActive(false);
        UiPausegame.SetActive(false);
        playerController.enabled = true;
        playerInput.enabled = true;
        HpText.SetActive(true);

        //Fade Boss1 to Boss2
        timeElapsed = 0f;
        image = ChangeBoss.GetComponent<Image>();
        startColor = image.color;
        Time.timeScale = 1.5f;
    }

    private void Update()
    {
        if (!damagalbleScriptBoss1.IsAlive)
        {
            canESC = false;
            ChangeBoss.SetActive(true);
            bossScript.enabled = false;
            Time.timeScale = 0.5f;
            HpText.SetActive(false);
            FadeLod(true, fadeTime);
        }


        if (!damagalbleScriptBoss1.IsAlive && !IsEnd)
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
        }
        
    }
}
