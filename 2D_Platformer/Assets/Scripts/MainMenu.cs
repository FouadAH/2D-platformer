using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        StartCoroutine(Loading());
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit");
    }

    private IEnumerator Loading()
    {
        SceneManager.LoadSceneAsync("Base", LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync("Level_1", LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync("MainMenu");
        yield return null;
    }
}
