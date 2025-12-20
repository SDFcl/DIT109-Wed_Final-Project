using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ButtonEvent : MonoBehaviour
{
    public void LoadScene(string SceneName)
    {
        if (string.IsNullOrEmpty(SceneName))
        {
            //SceneManager.LoadScene(SceneName);
            Debug.Log("NotFindName");
            return;
        }
        SceneManager.LoadScene(SceneName);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
