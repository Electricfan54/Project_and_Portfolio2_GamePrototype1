using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour, IDamage
{
    NavMeshAgent agent;

    enum EnemyType
    {
        Melee,
        Ranged
    }

    [SerializeField] Renderer meshRenderer;
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] EnemyType enemyType;
    [SerializeField] int enemyRotationSpeed;
    [Tooltip("Measured in attacks per second. the higher the number the faster the enemy attacks.")]
    [SerializeField] float attackSpeed;
    [SerializeField] int maxHP;
    int HP;

    [Header("Ranged Variables")]
    [SerializeField] Transform bulletSpawnPos;
    [SerializeField] Transform target;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] int damage;


    [Header("Melee Variables")]
    [SerializeField] int meleeRange;

    bool isAttacking = false;

    // for rotating the enemy towards the target
    Vector3 rotDir;
    Quaternion rot;

    Color colorOrig;



    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        //If found set target to player
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
        //Tell the game manager this enemy is alive
    }

    void Update()
    {

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            FaceTarget();
            Attack();
        }

        //Temp code
        if (Input.GetKeyDown(KeyCode.F))
        {
            TakeDamage(1);
        }
    }

    void FaceTarget()
    {
        rotDir = target.position - transform.position;
        rot = Quaternion.LookRotation(rotDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, enemyRotationSpeed * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        // Setting destination in fixed update so the path is recalculated less than in update
        agent.SetDestination(target.position);
    }

    void Attack()
    {
        switch (enemyType)
        {
            case EnemyType.Melee:
                // Should offset the position of the raycast
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

        //Temporary test code
        //GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPos.position, transform.rotation);
        //bullet.GetComponent<Rigidbody>().AddForce(transform.forward * 10.0f, ForceMode.Impulse);
        //Destroy(bullet, 3);

        yield return new WaitForSeconds(1 / attackSpeed);
        isAttacking = false;
    }

    IEnumerator MeleeAttack()
    {
        isAttacking = true;


        RaycastHit hit;
        // Temporary position for now
        Vector3 rayPos = new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z);
        
        if (Physics.Raycast(rayPos, transform.forward, out hit, meleeRange, ~ignoreLayer))
        {
            if (hit.collider.CompareTag("Player"))
            {
                // get IDamage component and damage the player
                Debug.Log("Melee Attack");
            }
        }

        yield return new WaitForSeconds(1 / attackSpeed);
        isAttacking = false;
    }

    public void TakeDamage(int damageAmount)
    {
        HP -= damageAmount;
        StartCoroutine(DamageFlash());
        if (HP <= 0)
        {
            //Tell the game manager this enemy is dead
            Destroy(gameObject);
        }
    }

    IEnumerator DamageFlash()
    {
        meshRenderer.material.color = Color.red;
        yield return new WaitForSeconds(.1f);
        meshRenderer.material.color = colorOrig;
    }
}
