using Scenes.Alex.Scripts.Interfaces;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
public class Target : MonoBehaviour, IDamageable
    {
        public float health = 100f;
        public Slider healthBar;
        public Gradient gradient;
        public Image fill;
        public AudioClip[] hitSounds;
        public AudioClip deathSound;
        public float hitAudioVolume = 1f;
        public GameObject particlePrefab;
        public Transform effectLocation;
        
        private Animator animator;
        private int _animIDisDead;
        // private EnemyAI2 enemyAi2;
    
    

        private void Start()
        {
            UpdateHealthBar();
            animator = GetComponent<Animator>();
            _animIDisDead = Animator.StringToHash("isDead");
        }

        public void TakeDamage(float damage)
        {
            health -= damage;
            
            if (hitSounds.Length > 0)
            {
                var index = Random.Range(0, hitSounds.Length);
                AudioSource.PlayClipAtPoint(hitSounds[index], transform.position, hitAudioVolume);
            }
            
            
            if (health <= 0)
            {
                FindObjectOfType<GameHandler>().AddScore(100);
                Death();
            }

            UpdateHealthBar();
        }

        private void UpdateHealthBar()
        {
            if (healthBar != null)
            {
                healthBar.value = health / 100f; // Assuming health ranges from 0 to 100
                fill.color = gradient.Evaluate(healthBar.normalizedValue);
            }
        }
        
        private void Death()
        {
            Destroy(gameObject);
            /*AudioSource.PlayClipAtPoint(deathSound, transform.position, hitAudioVolume);
            
            var particleVFX = Instantiate(particlePrefab, effectLocation.position, transform.rotation);

            // Handle particle system lifespan
            var ps = particleVFX.GetComponent<ParticleSystem>();
            if (ps != null)
                Destroy(particleVFX, ps.main.duration);
            else {
                var psChild = particleVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(particleVFX, psChild.main.duration);
            }*/
            
            // GetComponent<NavMeshAgent>().enabled = false;
            // GetComponent<EnemyAI2>().enabled = false;
            
        }
        

    }
