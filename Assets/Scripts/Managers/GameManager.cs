using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public void ShowMainMenu()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            SceneManager.LoadScene(0);
        }
        
        Debug.Log("ShowMainMenu");
    }
    
    public void StartGame()
    {
        if (SceneManager.GetActiveScene().buildIndex != 1)
        {
            SceneManager.LoadScene(1);
        }
        
        Debug.Log("StartGame");
    }
    
    public void QuitGame()
    {
        Debug.Log("QuitGame");
        
        Application.Quit();
    }
}
