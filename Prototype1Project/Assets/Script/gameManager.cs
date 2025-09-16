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

[System.Serializable]
public struct EndlessWeights
{
    [Tooltip("The type of enemy to spawn.")]
    [SerializeField] public GameObject enemyType;
    [Tooltip("Higher values mean they spawn more frequently in LATER waves.")]
    [SerializeField] [Range(1, 100)] public int enemyWeight;

}

public class gameManager : MonoBehaviour
{

    public static gameManager instance;

    [Header("UI Specific")]
    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;

    public bool isPaused;

    List<GameObject> menuHierarchy = new List<GameObject>();

    [SerializeField] TMP_Text enemyCountText;
    [SerializeField] TMP_Text waveNumText;

    public Image playerHPBar;
    public GameObject playerDamageFlash;

    [Header("Wave Customization")] // Wave specific variables here
    [SerializeField] List<WaveStruct> enemyWaves;

    [Tooltip("Debugging option to disable the wave system.")]
    [SerializeField] bool disableWaveGame;
    [Tooltip("Debugging option to disable the win condition after beating all waves.")]
    [SerializeField] bool disableWinCondition;

    float spawnTimer;
    float waveSpawnDelay;

    int spawnPosIndex;

    public int waveSpawnedTotal;
    public int maxWaveEnemies;

    int waveNum;
    bool waveActive;

    [HideInInspector] public int enemyCount;

    [Header("Endless Mode Customization")]
    [Tooltip("Enables the endless mode that is usually accessible after reaching the last preset wave.")]
    [SerializeField] bool endlessActive;
    [Tooltip("By how much to modify the endless difficulty based on this value and the wave number.")]
    [SerializeField] float diffMod;
    float diffMult;

    [Tooltip("The spawn locations for the enemies during endless mode.")]
    [SerializeField] List<Transform> endlessSpawns = new List<Transform>();

    [Tooltip("The enemy weights for endless mode. Higher weights mean they are more likely to appear in later waves, and vice-versa.")]
    [SerializeField] List<EndlessWeights> enemyWeights = new List<EndlessWeights>();

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

        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<PlayerMovement>();

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
                menuHierarchy.Add(menuPause);
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

        if ((waveSpawnedTotal == maxWaveEnemies) && enemyCount == 0 && (enemyWaves.Count > 0))
        {
            waveActive = false;
            enemyWaves.RemoveAt(0);
            StartWave();
        }

    }

    public void GameOver()
    {
        PauseGame();
        menuHierarchy.Add(menuLose);
        menuActive = menuLose;
        menuActive.SetActive(true);
    }

    public void StartGame()
    {
        if (!disableWaveGame)
        {
            waveNum = 0;
            spawnTimer = 0;

            if (!endlessActive)
                StartWave();
            else
            {
                waveNum = 1;
                GenerateWave();
            }

        }
    }

    public void StartWave()
    {

        if (enemyWaves.Count > 0)
        {
            spawnPosIndex = 0;
            waveSpawnedTotal = 0;
            maxWaveEnemies = 0;
            waveSpawnDelay = enemyWaves[0].enemies[0].spawnDelay;
            maxWaveEnemies += enemyWaves[0].enemies[0].spawnAmount;
            waveNum++;
            waveNumText.text = waveNum.ToString("F0");
            waveActive = true;
        }
        else
        {
            if (!endlessActive)
            {
                // Show win screen here!
                if (!disableWinCondition)
                {
                    PauseGame();
                    menuHierarchy.Add(menuWin);
                    menuActive = menuWin;
                    menuActive.SetActive(true);
                    endlessActive = true;
                    GenerateWave();
                }
            }
            else
            {
                GenerateWave();
            }
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

    void GenerateWave()
    {

        // Apply the difficulty multiplier
        diffMult = waveNum * diffMod;

        // Create a wave variable
        WaveStruct genWave = new();
        genWave.enemies = new();

        // Attach the selected spawnpoints for the endless mode (not random)
        genWave.spawnPoints = endlessSpawns;

        // Generate enemy clusters
        int clusterCount = Random.Range(Mathf.RoundToInt(diffMult / 2), (int)diffMult + 1);

        for (int i = 0; i < clusterCount; i++)
        {

            EnemyStruct genEnemy = new();

            GameObject genType = null;
            int genAmount;
            float genDelay;

            // Select enemy class
            int poolSelect = (int)Mathf.Clamp(Random.Range(1, 101) * diffMult, 1, 100);

            for (int j = 0; j < enemyWeights.Count; j++)
            {
                if (!(poolSelect >= enemyWeights[j].enemyWeight))
                {
                    break;
                }

                genType = enemyWeights[j].enemyType;
            }

            genAmount = (int)(Random.Range(1, diffMult + 1) * diffMult);
            genDelay = (Random.Range(1, diffMult + 1) / diffMult);

            genEnemy.enemyType = genType;
            genEnemy.spawnAmount = genAmount;
            genEnemy.spawnDelay = genDelay;

            genWave.enemies.Add(genEnemy);

        }

        enemyWaves.Add(genWave);
        StartWave();

    }

}
