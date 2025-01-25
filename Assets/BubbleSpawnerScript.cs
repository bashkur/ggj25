using UnityEngine;
using System.Collections;

public class BubbleSpawnerScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    float timer;
    int seconds;
    int minutes;
    int currentMinute = 1;
    public double difficultyScalingMultiplier;
    public double universalSpawnDelaySeconds = 0.5f;
    //Default as 1x

    public int initialSpawnCredits = 50;
    double maxSpawnCredits;
    int spawnCredits = 0;
    public Vector3 ExcludeSpawnAreaXZ;
    //Would declare what area around the player bubbles cannot spawn too close to player
    Vector3 playerPosition;
    
    
    
    public double normalBubbleWeight = 0.75;
    public double powerUpBubbleWeight = 0.05;
    public double gasBubbleWeight = 0.1;
    public double soapBubbleWeight = 0.1;


    void Start()
    {
        randomPositionGenerator();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;   

        seconds = (int)timer;
        //Debug.Log("current second " + seconds);
        //Debug.Log("current minute " + currentMinute);
        if(seconds % 60 == 0 & seconds/60 == currentMinute & seconds != 0)
        {
                difficultyScaler();
                currentMinute += 1;
        }
        if (spawnCredits <= (maxSpawnCredits - 5))
        {
            StartCoroutine(spawner());
            
            
        }
    }

    void bubbleTypeGenerator()
    {

    }

    void difficultyScaler()
    {
        maxSpawnCredits = (maxSpawnCredits + 5) * difficultyScalingMultiplier;
        maxSpawnCredits = Mathf.Floor(maxSpawnCredits);
    }

    IEnumerator spawner()
    {
       //Instantiate(Bubble, randomPositionGenerator())
       randomPositionGenerator();
       spawnCredits -= 1;
       yield return new WaitForSecondsRealtime(universalSpawnDelaySeconds);

    }

    Vector3 randomPositionGenerator()
    {
        float randomX = Random.Range(ExcludeSpawnAreaXZ.x,-ExcludeSpawnAreaXZ.x); 
        float randomZ = Random.Range(ExcludeSpawnAreaXZ.z,-ExcludeSpawnAreaXZ.z);
        const int Y = 5;

        Vector3 RandomPosition = new Vector3(randomX, Y, randomZ);
        Debug.Log(RandomPosition);
        return RandomPosition;
    }
}
