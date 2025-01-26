
using Scenes.Alex.Scripts.Interfaces;
using UnityEngine;

public class bubbleController : MonoBehaviour
{
    
    Vector3 bubblePosition;
    Vector3 playerPosition;
    GameObject Player;
    public string BubbleType;
    public float speed;
    public int CreditReturn;
    private bool collided;
    
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
    
    void OnCollisionEnter(Collision co)
    {
        // Avoid self-collision checks
        if (co.gameObject.CompareTag("Enemy") || collided)
            return;

        collided = true;

        // Apply damage if the hit object is damageable
        IDamageable damageable = co.gameObject.GetComponent<IDamageable>();
        damageable?.TakeDamage(1000);


        // Destroy the entire projectile
        Destroy(gameObject);
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

            private void onTriggerEnter(Collider other)
        {
            if(other.tag == "Player")
            {
                Debug.Log("Works");
                Destroy(this);
            }     
            else
            {
                Debug.Log("doesnt work");   
            }
        }
}
