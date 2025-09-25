using System.Collections;
using UnityEngine;

public class Barrel : MonoBehaviour, IDamage
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] int Health;
    [SerializeField] int MaxHealth;
    [SerializeField] GameObject affects;
    [SerializeField] int respawnTime;
    [SerializeField] Renderer model;
    public ParticleSystem hiteffect;
    bool didplay;
    Color colororiginal;


    public void TakeDamage(int damageAmount)
    {
        Health -= damageAmount;
        if (Health <= 0)
        {

            // gameObject.SetActive(false);
            affects.SetActive(true);
            model.enabled = false;
            gameObject.GetComponent<CapsuleCollider>().isTrigger = true;
            StartCoroutine(barrelrespwan());
        }
        else
        {

            StartCoroutine(flashred());
        }
    }

    void Start()
    {
        colororiginal = model.material.color;

    }

    // Update is called once per frame
    void Update()
    {

    }
    IEnumerator barrelrespwan()
    {if (!didplay)
        {
 Instantiate(hiteffect, gameObject.transform.position, Quaternion.identity);
            didplay = true;
        }
       
        yield return new WaitForSeconds(respawnTime);
        model.enabled = true;
        gameObject.SetActive(true);
        affects.SetActive(false);
        Health = MaxHealth;
        gameObject.GetComponent<CapsuleCollider>().isTrigger = false;
        didplay = false;
    }
    IEnumerator flashred()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colororiginal;
    }
}
