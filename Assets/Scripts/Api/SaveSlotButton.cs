using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SaveSlotButton : MonoBehaviour
{
    [SerializeField] private int slotId = 1;
    [SerializeField] private TextMeshProUGUI slotText;
    [SerializeField] private Button slotButton;

    [SerializeField] private string sceneForLevel1 = "Level1";
    [SerializeField] private string sceneForLevel2 = "Level2";

    private const string BASE_URL = "http://localhost:8000";

    [System.Serializable]
    public class Player
    {
        public int? player_id;
        public string name;
        public int? health;
        public int level_id;
        public int? passive_id;
    }

    public class PlayerGet
    {
        public int player_id;
        public string name;
        public int health;
        public int level_id;
        public int passive_id;
    }

    void Start()
    {
        if (slotButton == null) slotButton = GetComponent<Button>();
        slotButton.onClick.AddListener(OnClicked);
        StartCoroutine(RefreshDisplay());
    }

    private void OnClicked()
    {
        StartCoroutine(CheckAndLoadOrNew());
    }

    private IEnumerator CheckAndLoadOrNew()
    {
        using (UnityWebRequest www = UnityWebRequest.Get($"{BASE_URL}/player/{slotId}"))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                // มีเซฟ → โหลดเลย
                Player player = JsonUtility.FromJson<Player>(www.downloadHandler.text);
                SetCurrentPlayer(player);
                LoadSceneByLevelId(player.level_id);
            }
            else if (www.responseCode == 404)
            {
                // ไม่มี → ขอให้ Manager เปิด popup กรอกชื่อ
                NewGameNameInputManager.Instance.RequestNewPlayerName(slotId);
            }
            else
            {
                slotText.text = $"Save {slotId}\nError";
            }
        }
    }

    private IEnumerator RefreshDisplay()
    {
        using (UnityWebRequest www = UnityWebRequest.Get($"{BASE_URL}/player/{slotId}"))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                PlayerGet p = JsonUtility.FromJson<PlayerGet>(www.downloadHandler.text);
                Debug.Log(p.name + " " + p.passive_id);
                UpdateText(p);
            }
            else if (www.responseCode == 404)
            {
                UpdateText(null);
            }
        }
    }

    private void UpdateText(PlayerGet p)
    {
        if (p != null && !string.IsNullOrEmpty(p.name))
        {
            // แสดงชื่อผู้เล่น บรรทัดแรก
            // บรรทัดที่สองแสดงด่าน
            
            string levelText = p.level_id == 2 ? "Lv 2" : "Lv 1";
            slotText.text = $"{p.name}\n{levelText}";
        }
        else
        {
            // ถ้ายังไม่มีเซฟในสล็อตนี้
            slotText.text = "Empty Slot\n";
            // หรือจะใช้ "ว่าง" ก็ได้
            // slotText.text = "ว่าง\n";
        }
    }

    private void LoadSceneByLevelId(int levelId)
    {
        string scene = levelId == 1 ? sceneForLevel1 : sceneForLevel2;
        SceneManager.LoadScene(scene);
    }

    private void SetCurrentPlayer(Player player)
    {
        ApiManager apiManager = FindAnyObjectByType<ApiManager>();
        apiManager.player_id = slotId;
        apiManager.name = player.name;
        apiManager.health = player.health ?? 3;
        apiManager.level_id = player.level_id;
        apiManager.passive_id = player.passive_id ?? 0;

        Debug.Log("Here" + player.name);
    }
}