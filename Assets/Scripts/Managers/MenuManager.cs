using UnityEngine;
using UnityEngine.Serialization;

public class MenuManager : MonoBehaviour
{
    public GameObject splashScreen;
    public GameObject mainMenu;
    public GameObject creditsScreen;
    
    
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
    
    public void StartGame()
    {
        mainMenu.SetActive(false);
        splashScreen.SetActive(false);
        creditsScreen.SetActive(false);
        
        Debug.Log("StartGame");
    }
    
    public void QuitGame()
    {
        Debug.Log("QuitGame");
        
        Application.Quit();
    }
    
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        splashScreen.SetActive(true);
        mainMenu.SetActive(false);
        creditsScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
