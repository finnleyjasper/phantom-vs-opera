using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public Animator transition;
    [Tooltip("How long to pause on the black screen")]public float transitionTime = 1f;
    [Tooltip("Name of the scene with the actual gameplay")] public string PlaySceneName = "Act 1";
    [Tooltip("Score + initials entry after a run ends.")] public string NameRecordSceneName = "Name record";
    [Tooltip("Leaderboard / end-of-run scene.")] public string EndSceneName = "Game Over";

    [HideInInspector] public static LevelLoader Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void LoadNextLevel()
    {
        StartCoroutine(LoadScene(SceneManager.GetActiveScene().buildIndex + 1));
    }

    public void LoadSceneByName(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("LevelLoader: scene name is null or empty.");
            return;
        }

        StartCoroutine(LoadSceneByNameRoutine(sceneName));
    }

    IEnumerator LoadScene(int sceneIndex)
    {
        if (transition != null)
            transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(sceneIndex);
    }

    IEnumerator LoadSceneByNameRoutine(string sceneName)
    {
        if (transition != null)
            transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(sceneName);
    }
}
