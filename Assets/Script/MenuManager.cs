using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void mengenalWarna(){
        SceneManager.LoadScene("Difficult");
    }

    public void AR(){
        SceneManager.LoadScene("AR");
    }
}
