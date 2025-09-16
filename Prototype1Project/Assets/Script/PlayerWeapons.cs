using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PlayerWeapons : MonoBehaviour
{
    [Header("Required Variables")]
    [Tooltip("List of weapons the player can use. The weapons need to be in the scene and inactive")]
    [SerializeField] List<WeaponScript> weapons;
    [SerializeField] GameObject grenadePrefab;
    [SerializeField] float grenadeThrowRate;
    [SerializeField] Transform cameraPos;
    [SerializeField] LayerMask ignorelayer;

    [Header("Player Variables")]
    [SerializeField] int meleeDamage;
    [SerializeField] float meleeDist;
    [SerializeField] float meleeRate;

    WeaponScript curWeapon;

    int curWeaponIndex;
    int maxIndex = 3;

    bool isAttacking;
    bool canUseGrenade = true;

    void Start()
    {
        maxIndex = weapons.Count - 1;
        if (weapons.Count > 0)
        {
            curWeapon = weapons[0];
            SetActiveWeapon(0);
        }
    }

    void Update()
    {
        SwapWeapons();

        if ((Input.GetButtonDown("Fire1") || Input.GetButton("Fire1")) && curWeapon.currAmmo > 0 && !isAttacking)
        {
            StartCoroutine(Shoot());
        }
        if ((Input.GetKeyDown(KeyCode.V) || Input.GetKey(KeyCode.V)) && !isAttacking)
        {
            StartCoroutine(Melee());
        }

        if (Input.GetKeyDown(KeyCode.F) && canUseGrenade)
        {
            StartCoroutine(ShootGrenade());
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

        //curWeapon.SetActive(false);
        curWeapon = weapons[index];
        curWeaponIndex = index;
        //curWeapon.SetActive(true);
    }

    IEnumerator Shoot()
    {
        isAttacking = true;
        Vector3 shootTarget = cameraPos.position + cameraPos.forward * 30.0f;

        Instantiate(curWeapon.Bullet, curWeapon.BulletSpawnPos.position, Quaternion.LookRotation(shootTarget));
        //shoot
        curWeapon.currAmmo--;
        yield return new WaitForSeconds(curWeapon.fireRate);


        isAttacking = false;
    }



    IEnumerator ShootGrenade()
    {
        canUseGrenade = false;

        Instantiate(grenadePrefab, curWeapon.BulletSpawnPos.position, cameraPos.rotation);

        yield return new WaitForSeconds(grenadeThrowRate);


        canUseGrenade = true;
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
            }
        }

        yield return new WaitForSeconds(meleeRate);

        isAttacking = false;
    }
}
