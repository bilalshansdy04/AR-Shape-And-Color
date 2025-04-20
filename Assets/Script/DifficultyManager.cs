using UnityEngine;
using UnityEngine.SceneManagement;

public class DifficultyManager : MonoBehaviour
{
    public static string tingkatKesulitanDipilih;

    public void PilihKesulitan(string kesulitan)
    {
        tingkatKesulitanDipilih = kesulitan;
        SceneManager.LoadScene("QuizGame");
    }
        public void BackToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
