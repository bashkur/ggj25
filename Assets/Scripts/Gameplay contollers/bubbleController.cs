using UnityEngine;

public class bubbleController : MonoBehaviour
{
    
    Vector3 bubblePosition;
    Vector3 playerPosition;
    GameObject Player;
    public string BubbleType;
    public float speed;
    public int CreditReturn;
    
    public BubbleSpawnerScript spawnerCredits;
    public PlayerShooting playerScript;
    //GameObject Player = GameObject.Find("Player");
    //Vector3 playerPosition = new Vector3(Player.position.transform.x,Player.position.transform.y,Player.position.transform.z);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Player = GameObject.Find("PlayerArmature");
        playerScript = Player.GetComponent<PlayerShooting>();
        
        spawnerCredits = GameObject.Find("Spawner").GetComponent<BubbleSpawnerScript>();
        //playerPosition = new Vector3(Player.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        bubblePosition = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
        playerPosition = new Vector3(Player.transform.position.x, Player.transform.position.y, Player.transform.position.z);
        //Debug.Log(playerPosition);
        this.transform.position = Vector3.MoveTowards(bubblePosition, (new Vector3(playerPosition.x,1.5f,playerPosition.z)), (speed * Time.deltaTime));
        //Debug.Log(playerPosition);
    }

        void OnDestroy()
    {
        //Debug.Log("Deleted " + BubbleType);
        if(this.BubbleType == "BasicBubble")
        {
            spawnerCredits.spawnCredits +=1;

        }
        else if(BubbleType == "powerUpBubble")
        {
            spawnerCredits.spawnCredits +=1;
            playerScript.fireRate += 5;
        }
        else if(BubbleType == "Bubble2")
        {
            spawnerCredits.spawnCredits +=5;
        }
            else if(BubbleType == "Bubble3")
        {
            spawnerCredits.spawnCredits +=10;
        }
    }
}
