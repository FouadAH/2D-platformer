using System.Collections;
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
    public ParallaxCamera camera;
    public CameraController cameraController;

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
        SceneManager.LoadScene(levelToLoad, LoadSceneMode.Additive);
        StartCoroutine(UnloadScene(levelToUnload, levelToLoad));
    }

    private IEnumerator UnloadScene(int levelToUnload, int levelToLoad)
    {
        yield return new WaitForSeconds(1f);
        SceneManager.UnloadSceneAsync(levelToUnload);
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(levelToLoad));
    }


}
