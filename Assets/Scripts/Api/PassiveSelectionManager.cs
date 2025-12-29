using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.SceneManagement;

public class PassiveSelectionManager : MonoBehaviour
{
    public string SceneName;
    [System.Serializable]
    public class Passive
    {
        public int passive_id;
        public string name;
        public string describe;
    }

    [System.Serializable]
    public class PassiveWrapper
    {
        public List<Passive> passives;
    }

    // อ้างอิง UI ใน Scene (ลากมาใส่ใน Inspector)
    public Button[] passiveButtons = new Button[3];   // ปุ่ม 3 ปุ่ม
    public TextMeshProUGUI[] nameTexts = new TextMeshProUGUI[3];            // Text ชื่อ Passive
    public TextMeshProUGUI[] descTexts = new TextMeshProUGUI[3];            // Text คำอธิบาย Passive

    private List<Passive> allPassives = new List<Passive>();
    private Passive[] selectedPassives = new Passive[3];

    private const string BASE_URL = "http://localhost:8000";

    void Start()
    {
        StartCoroutine(FetchPassives());
    }

    // ดึงข้อมูล Passive จาก Backend
    private IEnumerator FetchPassives()
    {
        using (UnityWebRequest www = UnityWebRequest.Get($"{BASE_URL}/passives"))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string json = www.downloadHandler.text;

                // แปลง JSON array เป็น List (เพราะ Unity JsonUtility ไม่รองรับ array ตรงๆ)
                PassiveWrapper wrapper = JsonUtility.FromJson<PassiveWrapper>("{\"passives\":" + json + "}");
                allPassives = wrapper.passives;

                if (allPassives.Count < 3)
                {
                    Debug.LogError("Passives ในฐานข้อมูลน้อยกว่า 3 ตัว!");
                    yield break;
                }

                DisplayRandomPassives();
            }
            else
            {
                Debug.LogError("ดึง Passive ไม่สำเร็จ: " + www.error);
            }
        }
    }

    // สุ่ม 3 Passive และแสดงผล
    private void DisplayRandomPassives()
    {
        List<Passive> tempList = new List<Passive>(allPassives);
        tempList.Shuffle(); // สุ่มสลับตำแหน่ง

        for (int i = 0; i < 3; i++)
        {
            selectedPassives[i] = tempList[i];

            // แสดงชื่อและคำอธิบาย
            nameTexts[i].text = selectedPassives[i].name;
            descTexts[i].text = string.IsNullOrEmpty(selectedPassives[i].describe)
                ? "ไม่มีคำอธิบาย"
                : selectedPassives[i].describe;

            // ล้าง listener เก่าแล้วเพิ่มใหม่
            int index = i;
            passiveButtons[i].onClick.RemoveAllListeners();
            passiveButtons[i].onClick.AddListener(() => OnPassiveSelected(index));
        }
    }

    // เมื่อผู้เล่นเลือก Passive
    private void OnPassiveSelected(int index)
    {
        // เก็บ passive_id ที่เลือก
        ApiManager.instance.CRpassive_id = selectedPassives[index].passive_id;
        Debug.Log($"เลือก Passive แล้ว: ID = {ApiManager.instance.CRpassive_id} ชื่อ: {selectedPassives[index].name}");

        // บันทึกข้อมูลผู้เล่นทันที (รวม passive ที่เพิ่งเลือก)
        ApiManager.instance.SavePlayerData();
        // หลังเซฟเสร็จแล้ว → กลับไปหน้าหลัก (หรือฉากอื่นที่คุณต้องการ)
        // ตัวอย่าง: กลับไป Main Menu
        SceneManager.LoadScene(SceneName);
    }

    private IEnumerator GoToNextBossAfterSave()
    {
        // รอสักครู่เพื่อให้เซฟเสร็จ (ป้องกัน race condition)
        yield return new WaitForSeconds(0.5f);

        // เปลี่ยนฉากไป Main Menu (เปลี่ยนชื่อ Scene ให้ตรงกับของคุณ)
        
        // หรือถ้าใช้ Scene ที่ชื่อต่างกัน เช่น "Menu" หรือ "StartScreen"
        // SceneManager.LoadScene("Menu");
    }
}

// Extension method สำหรับสุ่ม List
public static class ListExtensions
{
    public static void Shuffle<T>(this List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }
}