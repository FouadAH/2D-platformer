using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    public GameObject player;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public float health = 100;
    public float maxHealth = 100;
    public int currency;
    public Vector2 lastCheckpointPos;
    public int lastCheckpointLevelIndex;
    public Camera camera;
    public CameraController cameraController;
    public Animator anim;
    private int levelToUnload;
    private int levelToLoad;

    private void OnDrawGizmos()
    {
        if(lastCheckpointPos != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(lastCheckpointPos, 2);
        }
    }
    
    public void LoadScene(int levelToUnload, int levelToLoad)
    {
        player.GetComponent<Player>().enabled = false;
        anim.SetTrigger("FadeOut");
        this.levelToLoad = levelToLoad;
        this.levelToUnload = levelToUnload;
    }

    public void OnFadeComplete()
    {
        SceneManager.LoadSceneAsync(levelToLoad, LoadSceneMode.Additive).completed += LoadScene_completed;
    }

    private void LoadScene_completed(AsyncOperation obj)
    {
        if (obj.isDone)
        {
            player.GetComponent<Player>().enabled = true;
            SceneManager.UnloadSceneAsync(levelToUnload).completed += UnloadScene_completed;
        }
    }

    private void UnloadScene_completed(AsyncOperation obj)
    {
        if (obj.isDone)
        {
            anim.SetTrigger("FadeIn");
            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(levelToLoad));
        }
    }
}
