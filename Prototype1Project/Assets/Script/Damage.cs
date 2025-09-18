using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class Damage : MonoBehaviour
{
    enum DamageType { moving, stationary, explosion, DOT, Homing, poision }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] Rigidbody rb;

    public int damageamount;
    public float damageRate;
    public int speed;
    public int destroyTime;
    [SerializeField] DamageType type;
    [Tooltip("Ammount of time before explosion")]
    public float explodetime;
    [Tooltip("expolsion radius/ homing projectile lock on radius")]
    public int radius;
    [SerializeField] SphereCollider explosioncollider;
    [SerializeField] GameObject explosionEffect;
    float explodetimer;

    [Tooltip("how long the homing bullet stops for")]
    public float homingPauseTime;
    [Tooltip("how long the pullet goes for after detecting a target")]
    public float hominggotime;

    GameObject homingTarget;
    [Tooltip("the speed the projectile will move after finding an enemy")]
    public float speedAfterStop;
    float effectTimer;

    [Tooltip("Poison things")]
   public bool ispoison;
    public int posionwaitbeforhit;
    public int PoisonDurration;
    bool isDamaging;


    bool canDamage = false;
    void Start()
    {
        if (type == DamageType.moving)
        {
            Destroy(gameObject, destroyTime);
            if (type == DamageType.moving)
            {
                rb.linearVelocity = transform.forward * speed;
            }

        }
        if (type == DamageType.Homing)
        {
            gameObject.GetComponent<SphereCollider>().radius = radius;
            rb.linearVelocity = transform.forward * speed;
            Destroy(gameObject, destroyTime);
        }
        if (type == DamageType.explosion)
        {

            rb.AddForce(transform.forward * speed, ForceMode.Impulse);
            explosioncollider.radius = 0;
            IDamage dmg = GetComponent<IDamage>();
            StartCoroutine(explode(dmg));




        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        IDamage dmg = other.GetComponent<IDamage>();
        if (dmg != null && type == DamageType.moving || type == DamageType.stationary)
        {
            dmg.TakeDamage(damageamount);
        }
        if (type == DamageType.explosion)//Idea for explosion add a sphere with gravity that has gravity and a seprerate sphere for the explosion thats connected to the explosion
        {
            if (dmg != null)
            {
                if (other.CompareTag("Player"))
                {
                    //need player movement script to access  movement
                    gameManager.instance.player.GetComponent<PlayerMovement>().LauchPlayer((other.transform.position - transform.position));

                }
                else
                {
                    dmg.TakeDamage(damageamount);
                }





            }
        }

        if (type == DamageType.moving)
        {
            Destroy(gameObject);
        }

        if (type == DamageType.Homing)
        {

            if (dmg != null)
            {
                if (canDamage)
                {
                    dmg.TakeDamage(damageamount);
                    canDamage = false;
                }
                else
                {
                    homingTarget = other.gameObject;
                    StartCoroutine(HomingDelay());

                }
            }





        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (type == DamageType.explosion)
        {
            isDamaging = false;
            explodetimer += Time.deltaTime;
            if (explodetimer >= explodetime)
            {
                Destroy(gameObject);
            }
        }
        IDamage dmg = other.GetComponent<IDamage>();
        //if (type == DamageType.poision)
        //{
        //    if (dmg != null)
        //    {
        //        StartCoroutine(PoisonLeave(dmg));
        //    }
        //}
    }
    // Update is called once per frame
    void Update()
    {

        if (ispoison)
        {
            effectTimer += Time.deltaTime;
            gameManager.instance.player.GetComponent<PlayerMovement>().ApplyEffect(damageamount, PoisonDurration, posionwaitbeforhit);
            if(effectTimer>=PoisonDurration)
            {
                ispoison = false;
                effectTimer = 0;
                gameManager.instance.player.GetComponent<PlayerMovement>().hasStatusEffect = false;
            }
            
        }


    }

    private void OnTriggerStay(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }
        IDamage dmg = other.GetComponent<IDamage>();
        if (dmg != null && type == DamageType.DOT || dmg != null && type == DamageType.poision)
        {
            if (!isDamaging)
            {
                StartCoroutine(damageother(dmg));
            }
            if (type == DamageType.poision)
            {
                if (other.CompareTag("Player"))
                {
                    gameManager.instance.player.GetComponent<PlayerMovement>().hasStatusEffect = true;
                    gameManager.instance.player.GetComponent<PlayerMovement>().ApplyEffect(damageamount, PoisonDurration, posionwaitbeforhit);
                    ispoison = true;
                }

            }

        }
        //if (dmg != null && type == DamageType.poision)
        //{
        //    if (!isDamaging)
        //    {
        //        StartCoroutine(damageother(dmg));
        //    }

        //}



    }
    IEnumerator damageother(IDamage d)
    {
        isDamaging = true;
        d.TakeDamage(damageamount);
        yield return new WaitForSeconds(damageRate);
        isDamaging = false;
    }
    IEnumerator explode(IDamage d)
    {

        yield return new WaitForSeconds(explodetime);
        explosioncollider.radius = radius;
        isDamaging = false;

        GameObject temp;
        temp = Instantiate(explosionEffect, transform.position, Quaternion.identity);
        Destroy(temp, 2f);
        Destroy(gameObject, 0.1f);


    }
    IEnumerator HomingDelay()
    {
        yield return new WaitForSeconds(hominggotime); // Time it goes before it stops
        rb.linearVelocity = Vector3.zero;
        gameObject.GetComponent<SphereCollider>().radius = 0.1f;
        yield return new WaitForSeconds(homingPauseTime);// Time it stops for
        Quaternion rot;
        if (homingTarget == null)
        {
            rot = Quaternion.identity;

        }
        else
        {
            rot = Quaternion.LookRotation(homingTarget.transform.position - transform.position);
        }

        transform.rotation = rot;
        rb.linearVelocity = transform.forward * speedAfterStop; //speed after stoping

        canDamage = true;
        

    }

    IEnumerator PoisonLeave(IDamage d)
    {
        isDamaging = true;
        gameManager.instance.player.GetComponent<PlayerMovement>().hasStatusEffect = true;
        for (int i = 0; i < PoisonDurration; i++)
        {
            yield return new WaitForSeconds(posionwaitbeforhit);
            d.TakeDamage(damageamount);

        }
        isDamaging = false;
        gameManager.instance.player.GetComponent<PlayerMovement>().hasStatusEffect = false;
    }

}
