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
public struct EndlessCosts
{
    [Tooltip("The type of enemy to spawn.")]
    [SerializeField] public GameObject enemyType;
    [Tooltip("How many credits it costs to spawn this enemy.")]
    [SerializeField] [Range(1, 1000)] public int creditCost;

}

public class gameManager : MonoBehaviour
{

    public static gameManager instance;

    [Header("UI Specific")]
    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject isPoisionIcon;

    public bool isPaused;

    List<GameObject> menuHierarchy = new List<GameObject>();

    [SerializeField] TMP_Text enemyCountText;
    [SerializeField] TMP_Text waveNumText;

    public Image playerHPBar;
    public GameObject playerDamageFlash;

    public TMP_Text ammoCurrent, ammoMax;

    [Header("Wave Customization")] // Wave specific variables here
    [SerializeField] List<WaveStruct> enemyWaves;

    [Tooltip("Debugging option to disable the wave system.")]
    [SerializeField] bool disableWaveGame;
    [Tooltip("Debugging option to disable the win condition after beating all waves.")]
    [SerializeField] bool disableWinCondition;

    float spawnTimer;
    float waveSpawnDelay;

    [Tooltip("How long the intermission between waves should be.")]
    [SerializeField] float waveIntermission;
    float graceTimer;

    bool intermission;

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
    [Tooltip("Initial credit value each wave.")]
    [SerializeField] int startCredits;
    [Tooltip("Initial spawn delay at the start of endless mode (continues to decrease as waves go on).")]
    [SerializeField] float endlessDelay;

    float diffcoeff;
    int currCredits;
    public int waveStartCreds;

    [Tooltip("The spawn locations for the enemies during endless mode.")]
    [SerializeField] List<Transform> endlessSpawns = new List<Transform>();

    [Tooltip("The enemy credit costs for endless mode. Enemies with more expensive costs spawn less frequently theoretically.")]
    [SerializeField] List<EndlessCosts> enemyCosts = new List<EndlessCosts>();

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
        poisonIcon();

        if (intermission)
        {
            GracePeriod();
        }

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

            if (enemyWaves.Count <= 0 && !endlessActive)
            {
                WinGame();
            }

            intermission = true;
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
            intermission = false;

            if (!endlessActive)
                StartWave();
            else
            {
                GenerateWave();
            }

        }
    }

    public void GracePeriod()
    {

        graceTimer += Time.deltaTime;

        if (graceTimer >= waveIntermission)
        {
            intermission = false;
            StartWave();
        }

    }

    public void StartWave()
    {

        if (enemyWaves.Count > 0)
        {
            spawnPosIndex = 0;
            waveSpawnedTotal = 0;
            maxWaveEnemies = 0;
            graceTimer = 0;
            waveSpawnDelay = enemyWaves[0].enemies[0].spawnDelay;
            maxWaveEnemies += enemyWaves[0].enemies[0].spawnAmount;
            waveNum++;
            waveNumText.text = waveNum.ToString("F0");
            waveActive = true;
        }
        else
        {
            if (endlessActive)
            {
                if (enemyWaves.Count > 0)
                {
                    enemyWaves.Clear();
                }
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

    public void WinGame()
    {
        // Show win screen here!
        if (!disableWinCondition)
        {
            PauseGame();
            menuHierarchy.Add(menuWin);
            menuActive = menuWin;
            menuActive.SetActive(true);
            endlessActive = true;
        }
    }

    void GenerateWave()
    {

        List<EndlessCosts> tempCosts = new List<EndlessCosts>(enemyCosts);

        // Apply the difficulty coefficient
        diffcoeff = Mathf.Max(waveNum, 1) * diffMod;

        // Create a wave variable
        WaveStruct genWave = new();
        genWave.enemies = new();

        // Attach the selected spawnpoints for the endless mode (not random)
        genWave.spawnPoints = endlessSpawns;

        // Set the amount of credits to spend
        currCredits = startCredits * Mathf.CeilToInt(diffcoeff);
        waveStartCreds = currCredits;

        endlessDelay -= (endlessDelay / (50 / diffcoeff));
        endlessDelay = Mathf.Clamp(endlessDelay, 0.01f, 99999);

        // Generate enemy clusters
        while (tempCosts.Count > 0 && currCredits > 0 && currCredits > tempCosts[0].creditCost)
        {
            int currEnemIndex = Random.Range(0, tempCosts.Count);

            if (currCredits >= tempCosts[currEnemIndex].creditCost)
            {
                EnemyStruct genEnemy = new();

                GameObject genType = tempCosts[currEnemIndex].enemyType;
                int maxGenAmount = (Mathf.FloorToInt(currCredits / tempCosts[currEnemIndex].creditCost));
                int genAmount = Random.Range(1, maxGenAmount + 1);

                currCredits = (currCredits - (tempCosts[currEnemIndex].creditCost * genAmount));

                genEnemy.enemyType = genType;
                genEnemy.spawnAmount = genAmount;
                genEnemy.spawnDelay = endlessDelay;

                genWave.enemies.Add(genEnemy);

            }
            else
            {
                tempCosts.Remove(tempCosts[currEnemIndex]);
            }
        }
        

        enemyWaves.Add(genWave);
        StartWave();

    }

    void poisonIcon()
    {
        if (playerScript.hasStatusEffect)
        {
            isPoisionIcon.SetActive(true);
        }
        else
        {
            isPoisionIcon.SetActive(false);
        }
    }

}
