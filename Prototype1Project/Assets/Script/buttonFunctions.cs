using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{
    
    public void Resume()
    {
        gameManager.instance.UnpauseGame();
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gameManager.instance.UnpauseGame();
    }

    public void Quit()
    {

    #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
    #else
            Application.Quit();
    #endif

    }

    public void MenuBack()
    {
        gameManager.instance.BackOutScreen();
    }

    public void MenuOpen(GameObject screen)
    {
        gameManager.instance.OpenScreen(screen);
    }

}
