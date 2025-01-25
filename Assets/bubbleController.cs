using UnityEngine;

public class bubbleController : MonoBehaviour
{
    Vector3 bubblePosition;
    public float speed;
    Vector3 playerPosition = new Vector3(0,0,0);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bubblePosition = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
        this.transform.position = Vector3.MoveTowards(bubblePosition, (new Vector3(playerPosition.x,3,playerPosition.z)), (speed * Time.deltaTime));
    }
}
