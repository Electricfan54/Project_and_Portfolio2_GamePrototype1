using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class Damage : MonoBehaviour
{
    enum DamageType { moving, stationary, explosion, DOT ,Homing}
    // Start is called once before the first execution of Update after the MonoBehaviour is created
  [SerializeField] Rigidbody rb;
    [SerializeField] int damageamount;
    [SerializeField] float damageRate;
    [SerializeField] int speed;
    [SerializeField] int destroyTime;
    [SerializeField] float explodetime;
    [SerializeField] DamageType type;
    [SerializeField] int radius;
    [SerializeField] SphereCollider explosioncollider;
    [SerializeField] float homingPauseTime;
    float explodetimer;
    bool isDamaging;
    Vector3 homingTarget;
    float homingTimer;
    bool canDamage;
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
        if(type==DamageType.Homing)
        {
          
            rb.linearVelocity = transform.forward * speed;
            Destroy(gameObject, destroyTime);
        }
        if (type == DamageType.explosion)
        {
            
            rb.AddForce(transform.forward * speed,ForceMode.Impulse);
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
                if(other.CompareTag("Player"))
                {
                    //need player movement script to access  movement
                    gameManager.instance.player.GetComponent<PlayerMovement>().LauchPlayer((other.transform.position - transform.position));

                }
                dmg.TakeDamage(damageamount);
                



            }
        }

        if (type == DamageType.moving)
        {
            Destroy(gameObject);
        }

        if (type == DamageType.Homing)
        {
            
               if(dmg != null)
                if(canDamage)
                                    {
                    dmg.TakeDamage(damageamount);
                    canDamage = false;
                }
                else
                {
StartCoroutine(HomingDelay());
                    Destroy(gameObject, destroyTime);
                    canDamage = true;
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
        
        if (type == DamageType.Homing )
        {homingTimer += Time.deltaTime;
            
           

        }
       
        
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
        Destroy(gameObject,0.1f);
       
    }
    IEnumerator HomingDelay()
    {
        rb.linearVelocity = Vector3.zero;
        yield return new WaitForSeconds(homingPauseTime);
 Quaternion rot = Quaternion.LookRotation(new Vector3(homingTarget.x, transform.position.y, homingTarget.z));
        rb.rotation = Quaternion.Lerp(transform.rotation, rot, 5 * Time.deltaTime);
        rb.linearVelocity = transform.forward * speed;
        gameObject.GetComponent<SphereCollider>().radius = 0.1f;
        canDamage = true;
        homingTimer = 0;

    }

}
