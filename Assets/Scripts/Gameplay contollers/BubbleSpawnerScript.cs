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
    public float universalSpawnDelaySeconds = 2f;
    public GameObject bubble;
    public GameObject powerUpBubble;
    public GameObject type2Bubble;
    public GameObject type3Bubble;
    GameObject bubbleType;
    GameObject Player;
    float X;
    float Z;

    //Default as 1x


    float maxSpawnCredits = 50f;
    public int spawnCredits = 50;
    public Vector3 ExcludeSpawnAreaXZ;
    //Would declare what area around the player bubbles cannot spawn too close to player
    Vector3 playerPosition;
    float randomX;
    float randomZ;
    
    
    
    public double normalBubbleWeight = 0.7;
    public double powerUpBubbleWeight = 0.1;
    public double Type2BubbleWeight = 0.1;
    public double Type3BubbleWeight = 0.1;

    bool spawnOnCooldown = false;

    void Start()
    {
        Player = GameObject.Find("PlayerArmature");
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;   
        playerPosition = new Vector3(Player.transform.position.x, Player.transform.position.y, Player.transform.position.z);
        seconds = (int)timer;
        //Debug.Log("current second " + seconds);
        //Debug.Log("current minute " + currentMinute);
        if(seconds % 60 == 0 & seconds/60 == currentMinute & seconds != 0)
        {
            difficultyScaler();
            currentMinute += 1;
        }
        if (spawnCredits <= (maxSpawnCredits) & spawnOnCooldown == false)
        {
            StartCoroutine(spawner());
        }
        //Debug.Log(spawnCredits);
    }

    GameObject bubbleTypeGenerator()
    {
        float random = Random.Range(0f,1f);
        //Debug.Log(random);
        if(random <= normalBubbleWeight & spawnCredits >= 1)
        {
            bubbleType = bubble;
            spawnCredits -= 1;
        }
        else if(random <= normalBubbleWeight + powerUpBubbleWeight & spawnCredits >= 1)
        {
            bubbleType = powerUpBubble;
            spawnCredits -= 1;
        }
        else if (random <= (normalBubbleWeight + powerUpBubbleWeight + Type2BubbleWeight) & spawnCredits >= 5)
        {
            bubbleType = type2Bubble;
            spawnCredits -= 5;
        }
        else if (random <= (normalBubbleWeight + powerUpBubbleWeight + Type2BubbleWeight + Type3BubbleWeight) & spawnCredits >= 10)
        {
            bubbleType = type3Bubble;
            spawnCredits -= 10;
        }
        return bubbleType;    
    }

    void difficultyScaler()
    {
        float oldSpawnCredits = maxSpawnCredits;
        maxSpawnCredits = (maxSpawnCredits + 10) * difficultyScalingMultiplier;
        maxSpawnCredits = Mathf.Floor(maxSpawnCredits);
        Debug.Log(maxSpawnCredits);
        spawnCredits += (int)(maxSpawnCredits-oldSpawnCredits);

        if (universalSpawnDelaySeconds > 0)
        {
            universalSpawnDelaySeconds -= 0.2f;
        }
    }

    IEnumerator spawner()
    {
       if (spawnCredits <= (maxSpawnCredits) & spawnOnCooldown == false)
       {
        if (spawnCredits >= 0)
        {
            Instantiate(bubbleTypeGenerator(), randomPositionGenerator(), Quaternion.identity);
        }
        Debug.Log("Works");
        spawnOnCooldown = true;
        randomPositionGenerator();
        yield return new WaitForSeconds(universalSpawnDelaySeconds);
        spawnOnCooldown = false;
       }
       

    }

    Vector3 randomPositionGenerator()
    {
        randomX = Random.Range(-ExcludeSpawnAreaXZ.x,ExcludeSpawnAreaXZ.x); 
        //Debug.Log(randomX);
        randomZ = Random.Range(-ExcludeSpawnAreaXZ.z,ExcludeSpawnAreaXZ.z);
        const float Y = 1f;

        if (randomX > 0)
        {
            randomX = (Random.Range((ExcludeSpawnAreaXZ.x + 10),ExcludeSpawnAreaXZ.x) + ExcludeSpawnAreaXZ.x); 
            X = 1f;
        }
        else
        {
            randomX = (Random.Range((-ExcludeSpawnAreaXZ.x - 10),-ExcludeSpawnAreaXZ.x) - ExcludeSpawnAreaXZ.x); 
            X = -1f;
        }

        if (randomZ > 0)
        {
            randomZ = (Random.Range((ExcludeSpawnAreaXZ.z + 10),ExcludeSpawnAreaXZ.z) + ExcludeSpawnAreaXZ.z); 
            Z = 1f;
        }
        else
        {
            randomZ = (Random.Range((ExcludeSpawnAreaXZ.z - 10),ExcludeSpawnAreaXZ.z) - ExcludeSpawnAreaXZ.z);
            Z = -1f;
        }

        Vector3 RandomPosition = new Vector3((randomX + ((ExcludeSpawnAreaXZ.x)*X)), Y, (randomZ + ((ExcludeSpawnAreaXZ.z)*Z)));
        //Debug.Log(RandomPosition);
        return RandomPosition;
    }
}
