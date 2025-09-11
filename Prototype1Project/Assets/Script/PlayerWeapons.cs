using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class PlayerWeapons : MonoBehaviour
{
    [Header("Required Variables")]
    [Tooltip("List of weapons the player can use. The weapons need to be in the scene and inactive")]
    [SerializeField] List<GameObject> weapons;
    [SerializeField] Transform cameraPos;
    [SerializeField] LayerMask ignorelayer;

    [Header("Player Variables")]
    [SerializeField] int meleeDamage;
    [SerializeField] float meleeDist;
    [SerializeField] float meleeRate;

    GameObject curWeapon;
    WeaponScript curWeaponScript;

    int curWeaponIndex;
    int maxIndex = 2;

    bool isAttacking;

    void Start()
    {
        maxIndex = weapons.Count;
        if (weapons.Count > 0)
        {
            curWeapon = weapons[0];
            SetActiveWeapon(0);
        }
    }

    void Update()
    {
        SwapWeapons();

        if ((Input.GetButtonDown("Fire1") || Input.GetButton("Fire1")) && !isAttacking)
        {
            StartCoroutine(Shoot());
        }
        if ((Input.GetKeyDown(KeyCode.V) || Input.GetKey(KeyCode.V)) && !isAttacking)
        {
            StartCoroutine(Melee());
        }
    }

    void SwapWeapons()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetActiveWeapon(0);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetActiveWeapon(1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetActiveWeapon(2);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (--curWeaponIndex >= 0)
                SetActiveWeapon(curWeaponIndex);
            else
                SetActiveWeapon(maxIndex);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (++curWeaponIndex < weapons.Count)
                SetActiveWeapon(curWeaponIndex);
            else
                SetActiveWeapon(0);
        }
    }
    void SetActiveWeapon(int index)
    {
        if (index < 0 || index >= weapons.Count)
            return;

        curWeapon.SetActive(false);
        curWeapon = weapons[index];
        curWeaponIndex = index;
        curWeapon.SetActive(true);
    }

    IEnumerator Shoot()
    {
        isAttacking = true;

        RaycastHit reticleHit;
        if (Physics.Raycast(cameraPos.position, cameraPos.forward, out reticleHit, 100.0f))
        {
            // gets the point the raycasts hits
            curWeaponScript.Shoot(reticleHit.point);
        }
        else
        {
            // if raycast misses get a point far away form player
            Vector3 pos = cameraPos.position + cameraPos.forward * 50.0f;
            curWeaponScript.Shoot(pos);
        }

        yield return new WaitForSeconds(curWeapon.GetComponent<WeaponScript>().fireRate);


        isAttacking = false;
    }

    IEnumerator Melee()
    {
        isAttacking = true;
        
        RaycastHit hit;
        if (Physics.Raycast(cameraPos.position, cameraPos.forward, out hit, meleeDist, ~ignorelayer))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                hit.collider.GetComponent<IDamage>().TakeDamage(meleeDamage);
                Debug.Log("Melee Hit");
            }
        }

        yield return new WaitForSeconds(meleeRate);

        isAttacking = false;
    }
}
