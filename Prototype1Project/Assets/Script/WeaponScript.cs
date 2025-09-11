using UnityEngine;
using UnityEngine.Rendering;

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

    public void Shoot(Vector3 CameraPos)
    {
        
        //Instantiate(Bullet, BulletSpawnPos.position, CameraPos);
    }
}
