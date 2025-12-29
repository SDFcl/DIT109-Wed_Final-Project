using TMPro;
using UnityEngine;

public class TextFollowPlayer : MonoBehaviour
{
    [SerializeField]private GameObject player;
    [SerializeField]private TextMeshProUGUI textMeshPro;
    [SerializeField]private DamagalbleScript PlayerdamagalbleScript;
    [SerializeField] private float PussZ = 300f;

    private void Awake()
    {
        if (PlayerdamagalbleScript == null)
        {
            ApiManager apiManager = FindAnyObjectByType<ApiManager>();
            textMeshPro.text = apiManager.name;
        }
    }
    // Update is called once per frame
    void Update()
    {
        Vector2 playerOnCamara = Camera.main.WorldToScreenPoint(player.transform.position);
        transform.position = new Vector2(playerOnCamara.x, playerOnCamara.y + PussZ);
        if (PlayerdamagalbleScript != null)
        {
            textMeshPro.text = PlayerdamagalbleScript.getHp();
        }

    }

}
