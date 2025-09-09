using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Device;
using TMPro;

public class gameManager : MonoBehaviour
{

    public static gameManager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuConfirmQuit;
    [SerializeField] TMP_Text enemyCountText;

    public Image playerHPBar;
    public GameObject playerDamageFlash;

    public GameObject player;
    public PlayerMovement playerScript;

    List<GameObject> menuHierarchy = new List<GameObject>();

    float timeScaleOrig;

    public bool isPaused;

    int enemyCount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
        timeScaleOrig = Time.timeScale;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetButtonDown("Cancel"))
        {
            if (menuActive == null)
            {
                PauseGame();
                menuActive = menuPause;
                menuActive.SetActive(true);
            }
            else if (menuActive == menuPause)
            {
                UnpauseGame();
            }
        }

    }

    public void PauseGame()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        menuHierarchy.Add(menuPause);
    }

    public void UnpauseGame()
    {
        isPaused = !isPaused;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
        menuHierarchy.Clear();
    }

    public void BackOutScreen() // Backs out in the menu list hierarchy
    {
        menuActive.SetActive(false);
        menuHierarchy.Remove(menuHierarchy[^1]);
        menuActive = menuHierarchy[^1];
        menuActive.SetActive(true);
    }

    public void OpenScreen(GameObject screen) // Accesses the selected menu screen and adds it to the hierarchy
    {
        menuActive.SetActive(false);
        menuHierarchy.Add(screen);
        menuActive = screen;
        menuActive.SetActive(true);
    }

    public void UpdateEnemyCount(int amount)
    {
        enemyCount += amount;
        enemyCountText.text = enemyCount.ToString("F0");
    }

}
