using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class NewGameNameInputManager : MonoBehaviour
{
    public static NewGameNameInputManager Instance;

    [Header("UI References")]
    [SerializeField] private GameObject nameInputPanel;
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    [Header("Default Values")]
    [SerializeField] private int defaultHealth = 3;
    [SerializeField] private int defaultLevelId = 1;
    [SerializeField] private int? defaultPassiveId = null;

    [Header("Scenes")]
    [SerializeField] private string sceneForLevel1 = "Level1";
    [SerializeField] private string sceneForLevel2 = "Level2";

    private int pendingSlotId = -1;
    private const string BASE_URL = "http://localhost:8000";

    [System.Serializable]
    public class Player
    {
        public int? player_id;
        public string name;
        public int? health;
        public int? level_id;
        public int? passive_id;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        nameInputPanel.SetActive(false);
        confirmButton.onClick.AddListener(OnConfirm);
        cancelButton.onClick.AddListener(OnCancel);
    }

    public void RequestNewPlayerName(int slotId)
    {
        pendingSlotId = slotId;
        nameInputPanel.SetActive(true);
        nameInputField.text = "";
        nameInputField.ActivateInputField();
    }

    private void OnConfirm()
    {
        string playerName = nameInputField.text.Trim();
        if (string.IsNullOrEmpty(playerName))
            playerName = "Player " + pendingSlotId;

        StartCoroutine(CreateNewPlayer(playerName));
        nameInputPanel.SetActive(false);
    }

    private void OnCancel()
    {
        pendingSlotId = -1;
        nameInputPanel.SetActive(false);
    }

    private IEnumerator CreateNewPlayer(string playerName)
    {
        Player newPlayer = new Player
        {
            player_id = pendingSlotId,
            name = playerName,
            health = defaultHealth,
            level_id = defaultLevelId,
            passive_id = defaultPassiveId
        };

        string json = JsonUtility.ToJson(newPlayer);
        Debug.Log(json);

        using (UnityWebRequest www = UnityWebRequest.Post($"{BASE_URL}/player", json, "application/json"))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Player created = JsonUtility.FromJson<Player>(www.downloadHandler.text);
                SetCurrentPlayer(created);
                LoadSceneByLevelId(created.level_id ?? 1);
            }
            else
            {
                Debug.LogError("Create failed: " + www.error);
            }
        }
    }

    private void LoadSceneByLevelId(int levelId)
    {
        string scene = levelId == 1 ? sceneForLevel1 : sceneForLevel2;
        SceneManager.LoadScene(scene);
    }

    private void SetCurrentPlayer(Player player)
    {
        ApiManager.instance.player_id = player.player_id ?? 0;
        ApiManager.instance.name = player.name ?? "";
        ApiManager.instance.health = player.health ?? 3;
        ApiManager.instance.level_id = player.level_id ?? 1;
        ApiManager.instance.passive_id = player.passive_id ?? 0;

        Debug.Log("Here");
    }
}