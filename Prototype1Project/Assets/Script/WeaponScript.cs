using UnityEngine;

public class WeaponScript : MonoBehaviour
{
    [SerializeField] GameObject Bullet;
    [SerializeField] GameObject WeaponModel;
    [SerializeField] Transform BulletSpawnPos;

    public float fireRate;

    void Start()
    {
        
    }


    void Update()
    {
        
    }

    public void Shoot()
    {
        Instantiate(Bullet, BulletSpawnPos.position, Quaternion.LookRotation(transform.forward));
    }
}
