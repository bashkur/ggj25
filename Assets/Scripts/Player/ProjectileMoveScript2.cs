using System.Collections;
using System.Collections.Generic;
using Scenes.Alex.Scripts.Interfaces;
using UnityEngine;

public class ProjectileMoveScript2 : MonoBehaviour
{
    [Header("Movement & Rotation")]
    public float speed;
    public bool rotate = false;
    public float rotateAmount = 45;

    [Header("Accuracy")]
    [Tooltip("From 0% to 100%")]
    public float accuracy;
    public float fireRate;

    [Header("Effects & Trails")]
    public GameObject muzzlePrefab;
    public GameObject hitPrefab;
    public List<GameObject> trails;
    public Transform muzzleTransform;

    [Header("Damage & Behavior")]
    public float damage = 10f;
    public bool pierce = false;
    public bool bounce = false;
    public float bounceForce = 10;
    public float maxDistance = 50;

    private Vector3 startPos;
    private Vector3 offset;
    private bool collided;
    private Rigidbody rb;
    private GameObject target;

    void Start()
    {
        startPos = transform.position;
        rb = GetComponent<Rigidbody>();

        // Adjust projectile rotation based on accuracy
        if (accuracy != 100)
        {
            // Convert percentage to a 0–1 range offset
            float inaccuracy = 1 - (accuracy / 100f);
            float rotationOffset = Random.Range(-inaccuracy, inaccuracy) * 90f;

            Quaternion currentRotation = transform.rotation;
            Quaternion accuracyRotation = Quaternion.Euler(0, rotationOffset, 0);
            transform.rotation = currentRotation * accuracyRotation;
        }

        // Instantiate muzzle effect (if assigned)
        if (muzzlePrefab != null)
        {
            Vector3 position = muzzleTransform ? muzzleTransform.position : transform.position;
            var muzzleVFX = Instantiate(muzzlePrefab, position, transform.rotation);

            if (muzzleTransform)
            {
                muzzleVFX.transform.SetParent(muzzleTransform);
            }

            muzzleVFX.transform.forward = transform.forward;

            // If the muzzle effect has a ParticleSystem, destroy it after its duration
            var ps = muzzleVFX.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                Destroy(muzzleVFX, ps.main.duration);
            }
            else
            {
                var psChild = muzzleVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(muzzleVFX, psChild.main.duration);
            }
        }
    }

    void FixedUpdate()
    {

        // Optionally rotate the projectile around its forward axis
        if (rotate)
        {
            transform.Rotate(0, 0, rotateAmount, Space.Self);
        }

        // Move the projectile forward
        if (speed != 0 && rb != null)
        {
            rb.position += (transform.forward + offset) * (speed * Time.deltaTime);
        }
        
        // Check if projectile has traveled beyond maxDistance
        if (Vector3.Distance(startPos, transform.position) >= maxDistance)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision co)
    {
        // Avoid self-collision checks
        if (co.gameObject.CompareTag("Bullet") || collided)
            return;

        collided = true;

        // Apply damage if the hit object is damageable
        IDamageable damageable = co.gameObject.GetComponent<IDamageable>();
        damageable?.TakeDamage(damage);

        // If it can pierce through damageables, do not destroy right away
        if (damageable != null && pierce)
        {
            return;
        }

        // Detach and stop all trails
        if (trails.Count > 0)
        {
            foreach (var trail in trails)
            {
                trail.transform.parent = null;
                var ps = trail.GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    ps.Stop();
                    Destroy(ps.gameObject, ps.main.duration + ps.main.startLifetime.constantMax);
                }
            }
        }

        // Stop the projectile
        speed = 0;
        rb.isKinematic = true;

        // Spawn hit effect if it exists and we hit a non-damageable object
        if (hitPrefab != null && damageable == null)
        {
            ContactPoint contact = co.contacts[0];
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
            Vector3 pos = contact.point;

            var hitVFX = Instantiate(hitPrefab, pos, rot);
            var ps = hitVFX.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                Destroy(hitVFX, ps.main.duration);
            }
            else
            {
                var psChild = hitVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(hitVFX, psChild.main.duration);
            }
        }

        // Handle bounce if enabled
        if (bounce)
        {
            rb.useGravity = true;
            rb.linearDamping = 0.5f;

            ContactPoint contact = co.contacts[0];
            rb.AddForce(
                Vector3.Reflect((contact.point - startPos).normalized, contact.normal) * bounceForce,
                ForceMode.Impulse);

            // Script ends here so it won't destroy the projectile until you decide
            // (Remove this next line if you actually want the projectile to persist after bounce.)
            Destroy(this);
            return;
        }

        // Destroy the entire projectile
        Destroy(gameObject);
    }
}
