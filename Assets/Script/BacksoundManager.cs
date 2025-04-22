using UnityEngine;
using UnityEngine.SceneManagement;

public class BacksoundManager : MonoBehaviour
{
    public static BacksoundManager instance;

    public AudioClip menuBacksound;
    public AudioClip quizBacksound;

    private AudioSource audioSource;
    private string currentSceneName = "";

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.loop = true;
            audioSource.playOnAwake = false;

            currentSceneName = SceneManager.GetActiveScene().name;
            PlayBacksoundForScene(currentSceneName);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        string sceneNow = SceneManager.GetActiveScene().name;

        if (sceneNow != currentSceneName || !IsCorrectBacksoundPlaying(sceneNow))
        {
            currentSceneName = sceneNow;
            PlayBacksoundForScene(sceneNow);
        }
    }

    void PlayBacksoundForScene(string sceneName)
    {
        AudioClip clipToPlay = null;

        if (sceneName == "Menu" || sceneName == "Difficult")
        {
            clipToPlay = menuBacksound;
        }
        else if (sceneName == "QuizGame")
        {
            clipToPlay = quizBacksound;
        }else {
            audioSource.Stop();
        }

        if (clipToPlay != null && audioSource.clip != clipToPlay)
        {
            audioSource.Stop();
            audioSource.clip = clipToPlay;
            audioSource.Play();
        }
    }

    bool IsCorrectBacksoundPlaying(string sceneName)
    {
        if ((sceneName == "Menu" || sceneName == "Difficult") && audioSource.clip != menuBacksound)
            return false;

        if (sceneName == "QuizGame" && audioSource.clip != quizBacksound)
            return false;

        return true;
    }
}
