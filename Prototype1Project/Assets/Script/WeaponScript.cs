using UnityEngine;

[CreateAssetMenu]

public class WeaponScript : ScriptableObject
{
    public GameObject Bullet;
    public GameObject WeaponModel;
    public Transform BulletSpawnPos;

    // Gun stuff
    [Range(1, 50)] public int gunDamage;
    [Range(0.1f, 2)] public float fireRate;
    [Range(1, 10)] public int DestroyTime;

    // Gernade stuff
    [Range(0.1f, 5)] public float explodeTime;
    [Range(1, 5)] public int CheckRadius;

    // homing stuff
    [Range(0.1f, 5)] public float HomingStartTime;
    [Range(0.1f, 5)] public float HomingStopTime;

   
    
}
