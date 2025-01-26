using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private Transform muzzle;
    
    private int currentAmmo;
    private bool isReloading;
    private int layerPlayerBullet;
    private AudioClip[] reloadSounds;
    private float[] reloadTimings;
    private ThirdPersonControllerTwinStick controller;

    float timeSinceLastShot;

    public GameObject bulletPrefab;


    // GunData
    public float accuracy;
    public float damage;
    public float bulletSpeed;
    public float maxDistance;
    public int canPierce;
    public int fireRate;

    public AudioClip fireSound;
    public float fireVolume;


    private void Awake()
    {
        layerPlayerBullet = LayerMask.NameToLayer("PlayerBullet");
    }

    private void Start()
    {
        ThirdPersonControllerTwinStick.shootInput += Fire;
        controller = GetComponent<ThirdPersonControllerTwinStick>();
        
        // currentAmmoText = GameObject.Find("CurrentAmmoCount").GetComponent<TextMeshProUGUI>();
    }

    private IEnumerator ReloadSounds()
    {
        for (int i = 0; i < reloadSounds.Length; i++)
        {
            yield return new WaitForSeconds(reloadTimings[i]);
            AudioSource.PlayClipAtPoint(reloadSounds[i], transform.position, 1);
        }
    }

    private bool CanShoot() => timeSinceLastShot > 1f / (fireRate / 60f);

    public void Fire()
    {
        if (CanShoot())
        {
            Debug.Log("Pew!");
            Debug.Log("GunData" + bulletPrefab);
            Debug.Log("Muzzle" + muzzle);
            GameObject bullet = Instantiate(bulletPrefab, muzzle.position, muzzle.rotation);
            Rigidbody bulletRigidbody = bullet.GetComponent<Rigidbody>();
            ProjectileMoveScript2 moveScript = bullet.GetComponent<ProjectileMoveScript2>();
            bullet.layer = layerPlayerBullet;

            controller.animIsShooting = true;
            Invoke(nameof(ResetShootBool), 0.2f);
            


            if (bulletRigidbody != null)
            {
                moveScript.accuracy = accuracy;
                moveScript.damage = damage;
                moveScript.speed = bulletSpeed;
                moveScript.maxDistance = maxDistance;
                // moveScript.pierce = canPierce;
                moveScript.muzzleTransform = muzzle;
            }
            
            // AudioSource.PlayClipAtPoint(fireSound, muzzle.position, fireVolume);
            
            timeSinceLastShot = 0;
            // UpdateAmmoUI();
        }
    }

    private void Update()
    {
        timeSinceLastShot += Time.deltaTime;
    }
    
    
    private void OnDestroy()
    {
        ThirdPersonControllerTwinStick.shootInput -= Fire;
    }
    
    private void ResetShootBool()
    {
        controller.animIsShooting = false;
    }
}
