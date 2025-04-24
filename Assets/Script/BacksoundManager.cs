using UnityEngine;
using UnityEngine.SceneManagement;

public class BacksoundManager : MonoBehaviour
{
    public static BacksoundManager instance;

    public AudioClip menuBacksound;
    public AudioClip quizBacksound;

    private AudioSource audioSource;
    private string currentSceneName = "";
    
    [Range(0f, 1f)]
    public float generalVolume = 1f; 
    public float quizVolumeMultiplier = 1f; 

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
            audioSource.volume = generalVolume;
        }
        else if (sceneName == "QuizGame")
        {
            clipToPlay = quizBacksound;
            audioSource.volume = generalVolume * quizVolumeMultiplier;
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

    // Untuk atur volume dari slider
    // public void SetGeneralVolume(float value)
    // {
    //     generalVolume = value;
    //     // Apply volume langsung ke scene sekarang
    //     PlayBacksoundForScene(SceneManager.GetActiveScene().name);
    // }
}
