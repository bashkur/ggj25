using UnityEngine;
using TMPro;
using System.Collections;

public class GameHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text scoreText;

    [Header("Settings")]
    [Tooltip("How long (in seconds) the score 'count-up' animation should take.")]
    [SerializeField] private float scoreAnimationDuration = 0.5f;

    private float elapsedTime = 0f;
    private float displayedScore = 0f;
    private int totalScore = 0;

    private Coroutine scoreCoroutine;

    /// <summary>
    /// Gets the raw elapsed time in seconds since the game started.
    /// </summary>
    public float ElapsedTime
    {
        get { return elapsedTime; }
    }

    /// <summary>
    /// Gets the player's total (target) score (the final value you are animating to).
    /// </summary>
    public int TotalScore
    {
        get { return totalScore; }
    }

    private void Start()
    {
        if (!timerText)
            timerText = GameObject.Find("Timer").GetComponent<TMP_Text>();

        if (!scoreText)
            scoreText = GameObject.Find("Score").GetComponent<TMP_Text>();
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        int minutes = (int)(elapsedTime / 60f);
        int seconds = (int)(elapsedTime % 60f);
        
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    
    public void AddScore(int amount)
    {
        // Update the total (target) score
        int oldTotalScore = totalScore;
        totalScore += amount;

        // Stop any ongoing score animation to prevent overlap
        if (scoreCoroutine != null)
        {
            StopCoroutine(scoreCoroutine);
        }

        // Start a new coroutine to animate from displayedScore to totalScore
        scoreCoroutine = StartCoroutine(AnimateScore(oldTotalScore, totalScore));
    }
    
    private IEnumerator AnimateScore(float startValue, float endValue)
    {
        float elapsed = 0f;

        // Use the current displayedScore as the start in case AddScore is called rapidly.
        float actualStart = displayedScore;

        while (elapsed < scoreAnimationDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / scoreAnimationDuration);

            displayedScore = Mathf.Lerp(actualStart, endValue, t);
            scoreText.text = Mathf.RoundToInt(displayedScore).ToString();

            yield return null;
        }

        // Ensure final value is set
        displayedScore = endValue;
        scoreText.text = displayedScore.ToString();
    }

    /* ---------------------------
     *   OPTIONAL HELPER METHODS
     * ---------------------------
     */

    /// <summary>
    /// Returns the current timer string in MM:SS format, 
    /// if you need to fetch the exact formatted text.
    /// </summary>
    public string GetFormattedTimer()
    {
        int minutes = (int)(elapsedTime / 60f);
        int seconds = (int)(elapsedTime % 60f);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    /// <summary>
    /// Returns the score string exactly as displayed in the UI (rounded to an int).
    /// </summary>
    public string GetDisplayedScoreString()
    {
        return scoreText != null ? scoreText.text : Mathf.RoundToInt(displayedScore).ToString();
    }
}
