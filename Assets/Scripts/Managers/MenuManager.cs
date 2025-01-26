using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject splashScreen;
    public GameObject mainMenu;
    public GameObject creditsScreen;


    private void Awake()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            ShowSplashScreen();
        }
    }

    private void ShowSplashScreen()
    {
        splashScreen.SetActive(true);
        mainMenu.SetActive(false);
        creditsScreen.SetActive(false);
        
        Debug.Log("ShowSplashScreen");
    }

    public void ShowMainMenu()
    {
        mainMenu.SetActive(true);
        splashScreen.SetActive(false);
        creditsScreen.SetActive(false);
        
        Debug.Log("ShowMainMenu");
    }
    
    public void ShowCredits()
    {
        creditsScreen.SetActive(true);
        splashScreen.SetActive(false);
        mainMenu.SetActive(false);
        
        Debug.Log("ShowCredits");
    }
    
}