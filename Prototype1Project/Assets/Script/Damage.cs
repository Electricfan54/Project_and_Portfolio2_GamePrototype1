using UnityEngine;
using System.Collections;

public class Damage : MonoBehaviour
{
    enum DamageType { moving, stationary, explosion, melee,DOT }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] DamageType damageType;
    [SerializeField] Rigidbody rb;
    [SerializeField] int damageamount;
    [SerializeField] float damageRate;
    [SerializeField] int speed;
    [SerializeField]int destroyTime;
    [SerializeField] DamageType type;
    bool isDamaging;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
