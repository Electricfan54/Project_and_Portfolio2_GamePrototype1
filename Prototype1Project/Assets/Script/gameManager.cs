using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

// Wave Structs
[System.Serializable]
public struct WaveStruct
{

    [SerializeField] public List<EnemyStruct> enemies;
    [Tooltip("The spawn points for all enemies.")]
    [SerializeField] public List<Transform> spawnPoints;

}

[System.Serializable]
public struct EnemyStruct
{
    [Tooltip("The type of enemy to spawn.")]
    [SerializeField] public GameObject enemyType;
    [Tooltip("How many of this enemy to spawn.")]
    [SerializeField] public int spawnAmount;
    [Tooltip("The delay (in seconds) between spawning this enemy type.")]
    [SerializeField] public float spawnDelay;

}

public class gameManager : MonoBehaviour
{

    public static gameManager instance;

    [Header("UI Specific")]
    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;

    public bool isPaused;

    List<GameObject> menuHierarchy = new List<GameObject>();

    [SerializeField] TMP_Text enemyCountText;
    [SerializeField] TMP_Text waveNumText;

    public Image playerHPBar;
    public GameObject playerDamageFlash;

    [Header("Wave Customization")] // Wave specific variables here
    [SerializeField] List<WaveStruct> enemyWaves;

    float spawnTimer;
    float waveSpawnDelay;

    int spawnPosIndex;

    [HideInInspector] public int waveSpawnedTotal;
    [HideInInspector] public int maxWaveEnemies;

    int waveNum;
    bool waveActive;

    [HideInInspector] public int enemyCount;

    [Header("Player Specific")]
    public GameObject player;
    public PlayerMovement playerScript;

    [Header("Unorganized")]
    float timeScaleOrig;

    void Awake()
    {
        instance = this;
        timeScaleOrig = Time.timeScale;
        waveActive = false;
    }

    private void Start()
    {
        StartGame();
    }

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

        spawnTimer += Time.deltaTime;

        if (waveActive && (spawnTimer >= waveSpawnDelay))
        {

            if ((waveSpawnedTotal >= maxWaveEnemies) && enemyWaves[0].enemies.Count > 1)
            {
                enemyWaves[0].enemies.RemoveAt(0);
                waveSpawnDelay = enemyWaves[0].enemies[0].spawnDelay;
                maxWaveEnemies += enemyWaves[0].enemies[0].spawnAmount;
            }
            else
            {
                
            }

            if (waveSpawnedTotal < maxWaveEnemies)
            {
                SpawnEnemy();
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

        if ((waveSpawnedTotal == maxWaveEnemies) && enemyCount == 0)
        {
            waveActive = false;
            enemyWaves.RemoveAt(0);
            StartWave();
        }

    }

    public void GameOver()
    {

    }

    public void StartGame()
    {
        waveNum = 0;
        spawnTimer = 0;
        StartWave();
    }

    public void StartWave()
    {

        if (enemyWaves.Count > 0)
        {
            spawnPosIndex = 0;
            maxWaveEnemies = 0;
            waveSpawnDelay = enemyWaves[0].enemies[0].spawnDelay;
            maxWaveEnemies += enemyWaves[0].enemies[0].spawnAmount;
            waveNum++;
            waveNumText.text = waveNum.ToString("F0");
            waveActive = true;
        }
        else
        {
            // Show win screen here!

        }

    }

    public void SpawnEnemy()
    {
        spawnTimer = 0;

        GameObject enemyType = enemyWaves[0].enemies[0].enemyType;
        Transform spawnPos = enemyWaves[0].spawnPoints[spawnPosIndex];
        spawnPosIndex++;

        if (spawnPosIndex >= enemyWaves[0].spawnPoints.Count)
        {
            spawnPosIndex = 0;
        }

        Instantiate<GameObject>(enemyType, spawnPos);
        waveSpawnedTotal++;

    }

}
