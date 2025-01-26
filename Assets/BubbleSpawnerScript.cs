using UnityEngine;
using System.Collections;

public class BubbleSpawnerScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    float timer;
    int seconds;
    int minutes;
    int currentMinute = 1;
    public float difficultyScalingMultiplier;
    public float universalSpawnDelaySeconds = 3f;
    public GameObject bubble;
    //Default as 1x

    public float initialSpawnCredits = 50;
    float maxSpawnCredits = 50;
    int spawnCredits = 40;
    public Vector3 ExcludeSpawnAreaXZ;
    //Would declare what area around the player bubbles cannot spawn too close to player
    Vector3 playerPosition;
    float randomX;
    float randomZ;
    
    
    
    public double normalBubbleWeight = 0.75;
    public double powerUpBubbleWeight = 0.05;
    public double gasBubbleWeight = 0.1;
    public double soapBubbleWeight = 0.1;

    bool spawnOnCooldown = false;

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
        if (spawnCredits <= (maxSpawnCredits) & spawnCredits >= 0 & spawnOnCooldown == false)
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
        if (universalSpawnDelaySeconds > 0)
        {
            universalSpawnDelaySeconds -= 0.2f;
        }
    }

    IEnumerator spawner()
    {
       if (spawnCredits <= (maxSpawnCredits) & spawnOnCooldown == false)
       {
        Instantiate(bubble, randomPositionGenerator(),Quaternion.identity);
        spawnOnCooldown = true;
        spawnCredits -= 1;
        randomPositionGenerator();
        yield return new WaitForSeconds(universalSpawnDelaySeconds);
        spawnOnCooldown = false;
       }
       

    }

    Vector3 randomPositionGenerator()
    {
        randomX = Random.Range(ExcludeSpawnAreaXZ.x,-ExcludeSpawnAreaXZ.x); 
        randomZ = Random.Range(ExcludeSpawnAreaXZ.z,-ExcludeSpawnAreaXZ.z);
        const int Y = 3;

        if (randomX >= 0)
        {
            randomX = (Random.Range((ExcludeSpawnAreaXZ.x + 2),ExcludeSpawnAreaXZ.x) + ExcludeSpawnAreaXZ.x); 
        }
        else
        {
            randomX = (Random.Range((-ExcludeSpawnAreaXZ.x - 2),-ExcludeSpawnAreaXZ.x) - ExcludeSpawnAreaXZ.x); 
        }

        if (randomZ > 0)
        {
            randomZ = (Random.Range((ExcludeSpawnAreaXZ.z + 2),ExcludeSpawnAreaXZ.z) + ExcludeSpawnAreaXZ.z); 
        }
        else
        {
            randomZ = (Random.Range((ExcludeSpawnAreaXZ.z - 2),ExcludeSpawnAreaXZ.z) - ExcludeSpawnAreaXZ.z);
        }

        Vector3 RandomPosition = new Vector3((randomX + ExcludeSpawnAreaXZ.x), Y, randomZ);
        Debug.Log(RandomPosition);
        return RandomPosition;
    }
}
