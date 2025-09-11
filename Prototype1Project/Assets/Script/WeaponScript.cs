using UnityEngine;

public class WeaponScript : MonoBehaviour
{
    [SerializeField] GameObject Bullet;
    [SerializeField] GameObject WeaponModel;
    [SerializeField] public Transform BulletSpawnPos;

    public float fireRate;

    void Start()
    {
        
    }


    void Update()
    {
        
    }

    public void Shoot(Vector3 GivenPos)
    {
        Vector3 pos = GivenPos - BulletSpawnPos.position;
        Instantiate(Bullet, BulletSpawnPos.position, Quaternion.LookRotation(pos));
    }
}
