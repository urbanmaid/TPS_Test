using UnityEngine;

[CreateAssetMenu(
    fileName = "WeaponObject",
    menuName = "Scriptable Object/Weapon",
    order = int.MaxValue
)]
public class WeaponTypes : ScriptableObject
{
    [Header("Basic Info")]
    public string weaponName;
    [TextArea] public string description;
    public Sprite icon;

    [Header("Damage & Attack")]
    public float damage = 25f;
    public float fireRate = 0.8f; // 발사 간격 (초)
    public float range = 240f;
    public float bulletSpread = 0.05f;

    [Header("Ammo & Reload")]
    public int magazineSize = 16;
    public float reloadTime = 3.5f;
    public string ammoType = "Default";

    [Header("Fire Mode")]
    public FireMode fireMode = FireMode.Single;
    public int bulletsPerShot = 1;

    [Header("Physics")]
    public float muzzleVelocity = 300f;
    public float impactForce = 10f;

    [Header("Effects")]
    public ParticleSystem muzzleFlash;
    public GameObject bulletPrefab;
    public AudioClip fireSound;
    public AudioClip reloadSound;

    [Header("Animation")]
    public AnimationClip fireAnimation;
    public AnimationClip reloadAnimation;
    public GameObject meshObject;
}

public enum FireMode
{
    Single,
    Burst,
    Auto
}
