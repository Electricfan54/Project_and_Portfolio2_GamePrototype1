using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

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
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] EnemyType enemyType;
    [Tooltip("Changes the navMeshAgent's stopping dist variable")]
    [SerializeField] int enemyStoppingDist;
    [SerializeField] int enemyRotationSpeed;
    [Tooltip("Measured in attacks per second. the higher the number the faster the enemy attacks.")]
    [SerializeField] int maxHP;
    [SerializeField] float attackSpeed;
    [SerializeField] int damage;
    [SerializeField] Image enemyHealthBar;

    [Header("Ranged Variables")]
    [SerializeField] Transform bulletSpawnPos;
    [SerializeField] Transform target;
    [SerializeField] GameObject bulletPrefab;


    [Header("Melee Variables")]
    [SerializeField] int meleeRange;

    // Non Serialized variables
    int HP;

    bool isAttacking = false;

    // for rotating the enemy towards the target
    Vector3 rotDir;
    Quaternion rot;

    Color colorOrig;

    // to prevent fatal errors
    bool isDead = false;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        //If found set target to player
        // Temp code until the game manager has a reference to the player
        Transform temp = GameObject.FindWithTag("Player").transform;
        if (temp != null)
        {
            target = temp;
        }
    }

    void Start()
    {
        colorOrig = meshRenderer.material.color;
        HP = maxHP;
        UpdateEnemyUI();
        agent.stoppingDistance = enemyStoppingDist;
        //Tell the game manager this enemy is alive
        
    }

    void Update()
    {

        if (isDead || target == null)
            return;

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            FaceTarget();
            Attack();
        }

        //Temp code for testing
        if (Input.GetKeyDown(KeyCode.F))
        {
            TakeDamage(1);
        }
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

        Instantiate(bulletPrefab, bulletSpawnPos.position, transform.rotation);

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
                // waiting for the game manager to have a reference to the player
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
}
