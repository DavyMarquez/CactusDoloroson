using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
public void Tutorial()
    {
        SceneManager.LoadScene("Tutorial01");
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        //SceneManager.GetActiveScene().buildIndex;
    }

    public void Credits()
    {
        SceneManager.LoadScene("Credits");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("StartMenu");
    }

    public void Next()
    {
        SceneManager.LoadScene("Tutorial02");
    }

    public void Previous()
    {
        SceneManager.LoadScene("Tutorial01");
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("JaviScene");
    }
}
