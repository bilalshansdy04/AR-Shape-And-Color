using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public float delayBeforeLoad = 0.5f;

    public void mengenalWarna()
    {
        StartCoroutine(LoadSceneWithDelay("Difficult"));
    }

    public void AR()
    {
        StartCoroutine(LoadSceneWithDelay("AR"));
    }

    private IEnumerator LoadSceneWithDelay(string sceneName)
    {
        yield return new WaitForSeconds(delayBeforeLoad); 
        SceneManager.LoadScene(sceneName);
    }
}