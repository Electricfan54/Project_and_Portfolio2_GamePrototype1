using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Unity.VisualScripting;

public enum ammoType
{
    rifle,
    sniper,
    homing,
}

public class PlayerWeapons : MonoBehaviour, IPickup
{
    [Header("Required Variables")]
    [Tooltip("List of weapons the player can use. The weapons need to be in the scene and inactive")]
    [SerializeField] List<WeaponScript> weapons;
    [SerializeField] GameObject grenadePrefab;
    [SerializeField] float grenadeThrowRate;
    [SerializeField] Transform cameraPos;
    [SerializeField] LayerMask ignorelayer;
    [SerializeField] GameObject GunModel;
    [SerializeField] Transform bulletSpawnPos;

    [Header("Player Variables")]
    [SerializeField] int meleeDamage;
    [SerializeField] float meleeDist;
    [SerializeField] float meleeRate;

    [Header("Ammo Variables")]
    [SerializeField] int ammoAmountRifle;
    [SerializeField] int ammoAmountSniper;
    [SerializeField] int ammoAmountHoming;

    WeaponScript curWeapon;

    int curWeaponIndex;
    int maxIndex = 3;

    bool isAttacking = false;
    bool isReloading = false;
    bool canUseGrenade = true;

    

    void Start()
    {
        maxIndex = weapons.Count - 1;
        if (weapons.Count > 0)
        {
            curWeapon = weapons[0];
            SetActiveWeapon(0);
        }
        ammoAmountRifle = 250;
        ammoAmountHoming = 250;
        ammoAmountSniper = 50;
        ReloadAll();
        UpdateUI();
    }

    void Update()
    {
        if (!isAttacking && !isReloading)
        {
            SwapWeapons();
        }

        if ((Input.GetButtonDown("Fire1") || Input.GetButton("Fire1")) && curWeapon.currAmmo > 0 && !isAttacking && !isReloading)
        {
            gameManager.instance.player.GetComponent<PlayerMovement>().aud.PlayOneShot(curWeapon.audioClips[Random.Range(0, curWeapon.audioClips.Length)], curWeapon.shootvolume); 
            StartCoroutine(Shoot());

        }
        
        if (Input.GetKeyDown(KeyCode.R) && !isReloading && curWeapon != null)
        {
            StartCoroutine(Reload());
        }

        if ((Input.GetKeyDown(KeyCode.V) || Input.GetKey(KeyCode.V)) && !isAttacking)
        {
            StartCoroutine(Melee());
            gameManager.instance.CDManager.AddCoolDown("Melee CD", meleeRate);
        }

        if (Input.GetKeyDown(KeyCode.F) && canUseGrenade)
        {
            StartCoroutine(ShootGrenade());
            gameManager.instance.CDManager.AddCoolDown("Gernade CD", grenadeThrowRate);

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

        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (--curWeaponIndex >= 0)
                SetActiveWeapon(curWeaponIndex);
            else
            {
                curWeaponIndex = maxIndex;
                SetActiveWeapon(curWeaponIndex);
            }
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (++curWeaponIndex < weapons.Count)
                SetActiveWeapon(curWeaponIndex);
            else
            {
                curWeaponIndex = 0;
                SetActiveWeapon(curWeaponIndex);
            }
        }
    }
    void SetActiveWeapon(int index)
    {
        if (index < 0 || index >= weapons.Count)
            return;

        //curWeapon.SetActive(false);
        curWeapon = weapons[index];
        curWeaponIndex = index;
        UpdateUI();
        curWeapon.Bullet.GetComponent<Damage>().damageamount = curWeapon.gunDamage;
        GunModel.GetComponent<MeshFilter>().sharedMesh = curWeapon.WeaponModel.GetComponent<MeshFilter>().sharedMesh;
        GunModel.GetComponent<MeshRenderer>().sharedMaterial = curWeapon.WeaponModel.GetComponent<MeshRenderer>().sharedMaterial;
        //curWeapon.SetActive(true);
    }

    IEnumerator Shoot()
    {
        
        isAttacking = true;
        Vector3 shootTarget = cameraPos.position + cameraPos.forward * 30.0f;


        Instantiate(curWeapon.Bullet, bulletSpawnPos.position, Quaternion.LookRotation(shootTarget - bulletSpawnPos.position));
        //shoot
        curWeapon.currAmmo--;
        UpdateUI();

        yield return new WaitForSeconds(curWeapon.fireRate);


        isAttacking = false;
    }



    IEnumerator ShootGrenade()
    {
        canUseGrenade = false;

        Instantiate(grenadePrefab, bulletSpawnPos.position, cameraPos.rotation);

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

    IEnumerator Reload()
    {
        isReloading = true;
        gameManager.instance.CDManager.AddCoolDown("Reloading", curWeapon.ReloadTimer);
        yield return new WaitForSeconds(curWeapon.ReloadTimer);

        int amountToReload = CalcAmmo();
         
        curWeapon.currAmmo += amountToReload;
        UpdateUI();
        isReloading = false;
    }

    public void ReloadAll()
    {
        for (int i = 0; i < weapons.Count; i++)
        {
            weapons[i].currAmmo = weapons[i].clipSize;
        }
    }

    void UpdateUI()
    {
        gameManager.instance.ammoCurrent.text = curWeapon.currAmmo.ToString("F0");

        switch (curWeapon.WeaponAmmoType)
        {
            case ammoType.rifle:
                gameManager.instance.ammoMax.text = ammoAmountRifle.ToString("F0");
                break;
            case ammoType.sniper:
                gameManager.instance.ammoMax.text = ammoAmountSniper.ToString("F0");
                break;
            case ammoType.homing:
                gameManager.instance.ammoMax.text = ammoAmountHoming.ToString("F0");
                break;
        }

    }

    int CalcAmmo()
    {
        int amountToReload = curWeapon.clipSize - curWeapon.currAmmo;

        switch (curWeapon.WeaponAmmoType)
        {
            case ammoType.rifle:
                if (ammoAmountRifle >= amountToReload)
                {
                    ammoAmountRifle -= amountToReload;
                }
                else
                {
                    amountToReload = ammoAmountRifle;
                    ammoAmountRifle = 0;
                }
                    break;
            case ammoType.sniper:
                if (ammoAmountSniper >= amountToReload)
                {
                    ammoAmountSniper -= amountToReload;
                }
                else
                {
                    amountToReload = ammoAmountSniper;
                    ammoAmountSniper = 0;
                }
                break;
            case ammoType.homing:
                if (ammoAmountHoming >= amountToReload)
                {
                    ammoAmountHoming -= amountToReload;
                }
                else
                {
                    amountToReload = ammoAmountHoming;
                    ammoAmountHoming = 0;
                }
                break;
        }

        return amountToReload;
    }

    public void PickupAmmo(ammoType type, int amount)
    {
        switch (type)
        {
            case ammoType.rifle:
                ammoAmountRifle += amount;
                break;
            case ammoType.sniper:
                ammoAmountSniper += amount;
                break;
            case ammoType.homing:
                ammoAmountHoming += amount;
                break;
        }
        UpdateUI();
    }

}
