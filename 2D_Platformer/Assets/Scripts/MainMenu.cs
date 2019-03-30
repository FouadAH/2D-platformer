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
        SceneManager.LoadSceneAsync("Level_1", LoadSceneMode.Additive).completed += MainMenu_completed;
        yield return null;
    }

    private void MainMenu_completed(AsyncOperation obj)
    {
        if(obj.isDone)
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(3));
            SceneManager.UnloadSceneAsync("MainMenu");
        }
    }
    
}
