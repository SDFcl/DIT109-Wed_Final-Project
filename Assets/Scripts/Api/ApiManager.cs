using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class ApiManager : MonoBehaviour
{
    public static ApiManager instance;

    public int player_id;
    public string name;
    public int health;
    public int level_id;
    public int passive_id;

    public int CRlevel_id;
    public int CRpassive_id;

    private const string BASE_URL = "http://localhost:8000";

    public class Player
    {
        public int player_id;
        public string name;
        public int health;
        public int level_id;
        public int passive_id;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public int CRsceneName()
    {
        string SceneName = SceneManager.GetActiveScene().name;
        if (SceneName == "Lv1")
        {
            return 2;
        }
        else if (SceneName == "Lv2")
        {
            return 2;
        }
        return 1;
    }

    public void SavePlayerData()
    {
        Player PlayerData = new Player
        {
            player_id = player_id,
            name = name,
            health = health,
            level_id = CRlevel_id,
            passive_id = CRpassive_id
        };
        Debug.Log(PlayerData.player_id + " " + PlayerData.name + " " + PlayerData.health + " " + PlayerData.level_id + " " + PlayerData.passive_id);
        string json = JsonUtility.ToJson(PlayerData);
        Debug.Log("JSON ที่ส่ง: " + json);  // เช็คดูว่า JSON ถูกต้องไหม
        StartCoroutine(PutData(json));
    }

    private IEnumerator PutData(string json)  // หรือเปลี่ยนชื่อเป็น PostData ก็ได้
    {
        using (UnityWebRequest www = UnityWebRequest.Post($"{BASE_URL}/playersave", json, "application/json"))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("เซฟสำเร็จ!");
                Player updated = JsonUtility.FromJson<Player>(www.downloadHandler.text);
                Debug.Log($"อัปเดต player_id: {updated.player_id}, passive_id: {updated.passive_id}");
            }
            else
            {
                Debug.LogError($"เซฟล้มเหลว: {www.responseCode} - {www.error}");
                Debug.LogError("รายละเอียดจาก server: " + www.downloadHandler.text);
            }
        }
    }
}
