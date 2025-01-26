using Managers;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    public SceneAsset menuScene;
    public SceneAsset gameScene;
    public SceneAsset leaderboardScene;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    public void ShowMainMenu()
    {
        if (SceneManager.GetActiveScene().name != menuScene.name)
        {
            SceneManager.LoadScene(menuScene.name);
        }
        
        Debug.Log("ShowMainMenu");
    }
    
    public void ShowCredits()
    {
        if (SceneManager.GetActiveScene().name != menuScene.name)
        {
            SceneManager.LoadScene(menuScene.name);
        }
        
        MenuManager.Instance.ShowCredits();
        
        Debug.Log("ShowCredits");
    }
    
    public void StartGame()
    {

        if (SceneManager.GetActiveScene().name != gameScene.name)
        {
            SceneManager.LoadScene(gameScene.name);
        }
        
        Debug.Log("StartGame");
    }
    
    public void ShowLeaderboard()
    {
        if (SceneManager.GetActiveScene().name != leaderboardScene.name)
        {
            SceneManager.LoadScene(leaderboardScene.name);
        }
        
        Debug.Log("ShowLeaderboard");
    }
    
    public void QuitGame()
    {
        Debug.Log("QuitGame");
        
        Application.Quit();
    }
}
