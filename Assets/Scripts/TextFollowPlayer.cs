using TMPro;
using UnityEngine;

public class TextFollowPlayer : MonoBehaviour
{
    [SerializeField]private GameObject player;
    [SerializeField]private TextMeshProUGUI textMeshPro;
    [SerializeField]private DamagalbleScript PlayerdamagalbleScript;
    private void Awake()
    {
   
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 playerOnCamara = Camera.main.WorldToScreenPoint(player.transform.position);
        transform.position = new Vector2(playerOnCamara.x, playerOnCamara.y + 150f);

        textMeshPro.text = PlayerdamagalbleScript.getHp();
    }
}
