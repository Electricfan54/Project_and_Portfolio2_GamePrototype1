using UnityEngine;

[CreateAssetMenu]

public class WeaponScript : ScriptableObject
{
    public GameObject Bullet;
    public GameObject WeaponModel;
    public ammoType WeaponAmmoType;
    public AudioClip[] audioClips;
    public ParticleSystem hiteffect;
    // Gun stuff
    [Range(1, 50)] public int gunDamage;
    [Range(0.1f, 5)] public float fireRate;
    [Range(1, 10)] public int DestroyTime;
    [Range(0, 50)] public int currAmmo;
    [Range(1, 50)] public int clipSize;
    [Range(1, 250)] public int maxAmmo;
    [Range(0.1f, 10)] public float ReloadTimer;
 [Range(0, 1)] public float shootvolume;
    // Gernade stuff
    [Range(0.1f, 5)] public float explodeTime;
    [Range(1, 5)] public int CheckRadius;

    // homing stuff
    [Range(0.1f, 5)] public float HomingStartTime;
    [Range(0.1f, 5)] public float HomingStopTime;
   


}
