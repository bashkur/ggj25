using UnityEngine;

public class BubbleSpawnerScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    float timer;
    int seconds;
    public float difficultyScalingMultiplier;
    public float spawnDelay;
    //Default as 1x

    const int initialSpawnCredits = 5;

    public Vector3 ExcludeSpawnAreaXZ;
    //Would declare what area around the player bubbles cannot spawn (like inside the camera)
    public float normalBubbleWeight = 0.75f;
    public float powerUpBubbleWeight = 0.05f;
    public float gasBubbleWeight = 0.1f;
    public float soapBubbleWeight = 0.1f;


    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;   

        seconds = (int)timer;
        Debug.Log(seconds);
    }

    void bubbleTypeGenerator()
    {

    }
}
