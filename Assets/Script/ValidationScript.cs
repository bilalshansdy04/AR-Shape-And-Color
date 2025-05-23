using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ValidationScript : MonoBehaviour
{
    public Image pauseImage;
    public Image validationExit;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Pause(){
        pauseImage.gameObject.SetActive(true);
    }
    public void Resume()
    {
        pauseImage.gameObject.SetActive(false);
    }

    public void Back()
    {
        SceneManager.LoadScene("Difficult");
    }
    public void BackToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void ExitOnClick(){
        validationExit.gameObject.SetActive(true);
    }
    public void ResumeOnClick(){
        validationExit.gameObject.SetActive(false);
    }
    public void QuitGame()
    {
        #if UNITY_EDITOR
            Debug.Log("Game exited");
        #else
            Application.Quit();
        #endif
    }

}
