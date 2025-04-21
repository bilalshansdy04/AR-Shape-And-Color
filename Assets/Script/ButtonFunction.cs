using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunction : MonoBehaviour
{
    [System.Serializable]
    public class ObjectLainnya
    {
        public GameObject obj;
        public AudioClip audioObject;
    }

    [System.Serializable]
    public class ObjectAR
    {
        public ObjectLainnya[] objectLainnyas;  // Array objek dan audio per marker
    }

    public ObjectAR[] objectArs;               // List marker dengan grup objek
    public GameObject[] basicObject;           // Marker dasar
    public GameObject[] buttonObject;    
    public GameObject playAudioButton;       // Tombol UI
    public AudioSource audioSource;            // Pemutar audio

    private GameObject targetAr;
    private int indexObjectActive;
    private int currentObjectIndex = 0;

    void Start()
    {
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        if (playAudioButton != null)
            playAudioButton.SetActive(false);
    }

    public void OnTargetFound(GameObject target)
    {
        targetAr = target;
        CheckTotalObject();
        
        currentObjectIndex = 0;
        ButtonChangeObject(0);

        // Tampilkan tombol play audio
        if (playAudioButton != null)
            playAudioButton.SetActive(true);
    }

    public void OnTargetLost()
    {
        for (int i = 0; i < buttonObject.Length; i++)
        {
            buttonObject[i].SetActive(false);
        }

        // Matikan semua objek pada marker yang sedang aktif
        foreach (var obj in objectArs[indexObjectActive].objectLainnyas)
        {
            obj.obj.SetActive(false);
        }

        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    void CheckTotalObject()
    {
        Debug.Log("Target ditemukan: " + targetAr.name);
        for (int i = 0; i < basicObject.Length; i++)
        {
            if (targetAr.name == basicObject[i].name)
            {
                indexObjectActive = i;
                int jumlahObjek = objectArs[i].objectLainnyas.Length;

                for (int j = 0; j < buttonObject.Length; j++)
                {
                    buttonObject[j].SetActive(j < jumlahObjek);
                }

                break;
            }
        }
    }

    public void ButtonChangeObject(int indexObject)
    {
        currentObjectIndex = indexObject;
        var objekList = objectArs[indexObjectActive].objectLainnyas;

        for (int i = 0; i < objekList.Length; i++)
        {
            objekList[i].obj.SetActive(i == indexObject);
        }

        // Mainkan audio jika ada
        AudioClip clip = objekList[indexObject].audioObject;
        if (clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }

    public void PlayCurrentAudio()
    {
        var objekList = objectArs[indexObjectActive].objectLainnyas;
        if (currentObjectIndex >= 0 && currentObjectIndex < objekList.Length)
        {
            PlayAudio(objekList[currentObjectIndex].audioObject);
        }
    }

    void PlayAudio(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
