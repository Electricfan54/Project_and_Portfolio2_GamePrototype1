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

    public void Shoot(Transform CameraPos)
    {
        
        Instantiate(Bullet, BulletSpawnPos.position, Quaternion.LookRotation(CameraPos.forward));
    }
}
