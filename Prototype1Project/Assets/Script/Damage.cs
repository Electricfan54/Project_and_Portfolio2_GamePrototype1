using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class Damage : MonoBehaviour
{
    enum DamageType { moving, stationary, explosion, DOT }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] DamageType damageType;
    [SerializeField] Rigidbody rb;
    [SerializeField] int damageamount;
    [SerializeField] float damageRate;
    [SerializeField] int speed;
    [SerializeField] int destroyTime;
    [SerializeField] float explodetime;
    [SerializeField] DamageType type;

    float explodetimer;
    bool isDamaging;

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
                isDamaging = true;
                explodetimer += Time.deltaTime;
                if (explodetimer >= explodetime && isDamaging)
                {
                    dmg.TakeDamage(damageamount);
                    Destroy(gameObject);
                }

            }
        }

        if (type == DamageType.moving)
        {
            Destroy(gameObject);
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

}
