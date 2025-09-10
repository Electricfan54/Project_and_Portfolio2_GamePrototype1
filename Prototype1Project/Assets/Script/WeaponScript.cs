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

    public void Shoot(Quaternion ReticalDir)
    {
        Instantiate(Bullet, BulletSpawnPos.position, ReticalDir);
    }
}
