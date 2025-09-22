using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[System.Serializable]
public struct ammoDrop
{
    [HideInInspector] public ammoType type;
    public int weight;
    public int amountMin;
    public int amountMax;
    public GameObject prefab;
}

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour, IDamage
{
    NavMeshAgent agent;

    enum EnemyType
    {
        Melee,
        Ranged
    }

    // Serialized variables
    [SerializeField] Renderer meshRenderer;
    [SerializeField] Animator animator;
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] EnemyType enemyType;
    [SerializeField] Transform enemyHeadPos;

    [Tooltip("Changes the navMeshAgent's stopping dist variable")]
    [SerializeField] int enemyStoppingDist;
    [Tooltip("Changes the navMeshAgent's speed variable")]
    [SerializeField] int speed;
    [SerializeField] int enemyRotationSpeed;
    [SerializeField] int maxHP;

    [Tooltip("Measured in attacks per second. the higher the number the faster the enemy attacks.")]
    [SerializeField] float attackSpeed;
    [SerializeField] int damage;
    [SerializeField] Image enemyHealthBar;

    [Header("Ranged Variables")]
    [SerializeField] Transform bulletSpawnPos;
    [SerializeField] Transform target;
    [SerializeField] GameObject bulletPrefab;


    [Header("Melee Variables")]
    [SerializeField] int meleeRange;

    [Header("Drop Rate Variables")]
    [Range(0, 100)]
    [SerializeField] int dropChance;
    [SerializeField] ammoDrop rifleAmmoWeight;
    [SerializeField] ammoDrop sniperAmmoWeight;
    [SerializeField] ammoDrop homingAmmoWeight;

   


    // Non Serialized variables
    int HP;

    bool isAttacking = false;
    bool isWalking = false;

    bool canSeePlayer;

    int durationtimer;
    int ticktimer;
public  bool hasStatusEffect;
    // for rotating the enemy towards the target
    Vector3 rotDir;
    Quaternion rot;

    Color colorOrig;

    // To help prevent fatal errors
    bool isDead = false;

    void Awake()
    {
        rifleAmmoWeight.type = ammoType.rifle;
        sniperAmmoWeight.type = ammoType.sniper;
        homingAmmoWeight.type = ammoType.homing;
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        target = gameManager.instance.player.transform;

        colorOrig = meshRenderer.material.color;
        HP = maxHP;
        UpdateEnemyUI();
        agent.stoppingDistance = enemyStoppingDist;
        agent.speed = speed;
        //Tell the game manager this enemy is alive
        gameManager.instance.UpdateEnemyCount(1);
        UpdateAnimations();
    }

    void Update()
    {

        if (isDead || target == null)
            return;

        // since the game is wave based the enemy will always know where the player is
        // this raycast is to prevent the enemy from trying to shoot through a wall
        RaycastHit hit;
        Vector3 playerDir = target.transform.position - enemyHeadPos.position;
        bool ray = Physics.Raycast(enemyHeadPos.position, playerDir, out hit, 50.0f, ~ignoreLayer);
        if (ray && hit.collider.CompareTag("Player"))
        {
            canSeePlayer = true;
            // reset the stopping distance after the enemy is done inching towards the player
            if (agent.stoppingDistance != enemyStoppingDist)
                agent.stoppingDistance = enemyStoppingDist;
        }
        else
            canSeePlayer = false;

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            isWalking = false;

            if (canSeePlayer)
            {
                FaceTarget();
                Attack();
            }
            else if (ray)
            {
                // if the enemy cant see the player through a wall inch the agent closer to try and get it to the player
                agent.stoppingDistance -= 1;
                // keep the stopping distance from getting too low
                if (agent.stoppingDistance < 3)
                    agent.stoppingDistance = 3;
            }
        }
        else
        {
            isWalking = true;
            canSeePlayer = false;
        }

        UpdateAnimations();

#if UNITY_EDITOR
        //Temp code for testing
        if (Input.GetKeyDown(KeyCode.F))
        {
            TakeDamage(1);
        }
#endif
    }

    void FaceTarget()
    {
        rotDir = new Vector3(target.position.x, transform.position.y, target.position.z) - transform.position;
        /// if statement to prevent unity message
        if (rotDir !=  Vector3.zero)
            rot = Quaternion.LookRotation(rotDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, enemyRotationSpeed * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (isDead) return;
        // Setting destination in fixed update so the path is recalculated less than in update
        if (target != null)
            agent.SetDestination(target.position);
    }

    void Attack()
    {
        switch (enemyType)
        {
            case EnemyType.Melee:
                if (!isAttacking)
                    StartCoroutine(MeleeAttack());
                break;
            case EnemyType.Ranged:
                if (!isAttacking)
                    StartCoroutine(Shoot());
                break;
        }
    }

    IEnumerator Shoot()
    {
        isAttacking = true;
        Vector3 playerDir = target.transform.position - bulletSpawnPos.position;
        Quaternion dir = Quaternion.LookRotation(playerDir);
        Instantiate(bulletPrefab, bulletSpawnPos.position, dir);

        yield return new WaitForSeconds(1 / attackSpeed);
        isAttacking = false;
    }

    IEnumerator MeleeAttack()
    {
        isAttacking = true;


        RaycastHit hit;
        // Should offset the position of the raycast
        // Temporary position for now
        Vector3 rayPos = new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z);
        
        if (Physics.Raycast(rayPos, transform.forward, out hit, meleeRange, ~ignoreLayer))
        {
            if (hit.collider.CompareTag("Player"))
            {
                // get IDamage component and damage the player
                IDamage dmg = hit.collider.GetComponent<IDamage>();
                if (dmg != null)
                    dmg.TakeDamage(damage);
            }
        }

        yield return new WaitForSeconds(1 / attackSpeed);
        isAttacking = false;
    }

    public void TakeDamage(int damageAmount)
    {
        HP -= damageAmount;
        UpdateEnemyUI();
        StartCoroutine(DamageFlash());
        if (HP <= 0)
        {
            //Tell the game manager this enemy is dead
            gameManager.instance.UpdateEnemyCount(-1);
            gameManager.instance.playerScript.HealPlayerOnKill();
            if (UnityEngine.Random.Range(1, 100) <= dropChance)
                DropAmmo();
            isDead = true;
            Destroy(gameObject);
        }
    }

    void UpdateEnemyUI()
    {
        if (enemyHealthBar == null)
            return;
        enemyHealthBar.fillAmount = (float)HP / maxHP;
    }

    IEnumerator DamageFlash()
    {
        meshRenderer.material.color = Color.red;
        yield return new WaitForSeconds(.1f);
        meshRenderer.material.color = colorOrig;
    }

    void UpdateAnimations()
    {
        animator.SetBool("isMoving", isWalking);
        animator.SetFloat("moveBlend", agent.velocity.magnitude / speed);
        switch (enemyType)
        {
            case EnemyType.Melee:
                animator.SetBool("isSwinging", isAttacking);
                animator.SetBool("isMelee", true);
                break;
            case EnemyType.Ranged:
                animator.SetBool("isShooting", !isWalking);
                animator.SetBool("isMelee", false);
                break;
        }

    }

    void DropAmmo()
    {
        int rangeMax = rifleAmmoWeight.weight + sniperAmmoWeight.weight + homingAmmoWeight.weight;
        int randType = UnityEngine.Random.Range(0, rangeMax);

        ammoDrop[] weightArray = { rifleAmmoWeight, sniperAmmoWeight, homingAmmoWeight };
        // the limit is 2 since the ammo type enum currently only goes up to 2.
        for (int i = 0; i < 3; i++)
        {
            if (randType < weightArray[i].weight)
            {
                randType = i;
                break;
            }
            randType -= weightArray[i].weight;
        }

        //if check to avoid bugs
        if (randType > 2)
            randType = 0;

            switch (randType)
            {
                case (int)ammoType.rifle:
                SpawnAmmo(rifleAmmoWeight);
                    break;
                case (int)ammoType.sniper:
                SpawnAmmo(sniperAmmoWeight);
                break;
                case (int)ammoType.homing:
                SpawnAmmo(homingAmmoWeight);
                break;

            }
    }

    void SpawnAmmo(ammoDrop drop)
    {
        GameObject dropObject = Instantiate(drop.prefab, transform.position, Quaternion.identity);
        AmmoPickup pickup = dropObject.GetComponent<AmmoPickup>();
        if (pickup != null)
        {
            pickup.type = drop.type;
            pickup.amount = UnityEngine.Random.Range(drop.amountMin, drop.amountMax);
        }
    }
}
