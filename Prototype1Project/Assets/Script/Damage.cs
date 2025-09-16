using UnityEngine;
using System.Collections;

public class Damage : MonoBehaviour
{
    enum DamageType { moving, stationary, explosion, DOT, Homing }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] Rigidbody rb;
    [SerializeField] public int damageamount;
    [SerializeField] public float damageRate;
    [SerializeField] public int speed;
    [SerializeField] public int destroyTime;
    [SerializeField] public float explodetime;
    [SerializeField] DamageType type;
    [SerializeField] public int radius;
    [SerializeField] SphereCollider explosioncollider;
    [SerializeField] public float homingPauseTime;
    [SerializeField] public float hominggotime;
    [SerializeField] GameObject explosionEffect;
    float explodetimer;
    bool isDamaging;
    Vector3 homingTarget;
    float homingTimer;
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
                    homingTarget = other.transform.position;
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
    }
    // Update is called once per frame
    void Update()
    {




    }

    private void OnTriggerStay(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }
        IDamage dmg = other.GetComponent<IDamage>();
        if (dmg != null && type == DamageType.DOT)
        {
            if (!isDamaging)
            {
                StartCoroutine(damageother(dmg));
            }

        }


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
        yield return new WaitForSeconds(hominggotime);
        rb.linearVelocity = Vector3.zero;
        gameObject.GetComponent<SphereCollider>().radius = 0.1f;
        yield return new WaitForSeconds(homingPauseTime);
        Quaternion rot = Quaternion.LookRotation(homingTarget - transform.position);
        transform.rotation = rot;
        rb.linearVelocity = transform.forward * speed;

        canDamage = true;
        homingTimer = 0;

    }

}
