using System.Collections;
using Scenes.Alex.Scripts.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerTarget : MonoBehaviour, IDamageable
{
    public float health = 100f;
    public Slider healthBar;
    public Gradient gradient;
    public Image fill;
    [SerializeField] private TMP_Text currentHealthText;
    
    public AudioClip hitSound;
    public float hitAudioVolume = 1f;

    IEnumerator DieAndGoToMenu()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(0);
    }

    private void Start()
    {
        // gameStateScreen = FindObjectOfType<GameStateScreen>();
        // Debug.Log(gameStateScreen);
        UpdateHealthBar();
        UpdateHealthUI();
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        // AudioSource.PlayClipAtPoint(hitSound, transform.position, hitAudioVolume);
        if (health <= 0)
        {
            // gameObject.SetActive(false);
            // gameStateScreen.Open("GameOver");
            SceneManager.LoadScene(0);
        }

        UpdateHealthBar();
        UpdateHealthUI();
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.value = health / 100f; // Assuming health ranges from 0 to 100
            fill.color = gradient.Evaluate(healthBar.normalizedValue);
        }
    }
    
    private void UpdateHealthUI()
    {
        // currentHealthText.text = health.ToString();
    }
}