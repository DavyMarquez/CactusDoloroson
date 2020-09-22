using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PuppyMovement : MonoBehaviour
{

    // All puppies in the scene (change for sphere collision for better performance?)
    [SerializeField]
    private List<GameObject> puppyList;

    [SerializeField]
    private GameObject player;

    // Area of effect of separation from others puppies 
    public float area = 1.5f;

    // Puppy speed
    public float speed = 6.0f;

    private Vector2 currentSpeed;

    // Start is called before the first frame update
    void Start()
    {
        puppyList = GameObject.FindGameObjectsWithTag("Puppy").ToList();
        puppyList.Remove(gameObject);
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {

        // UPDATE PUPPY LIST
        // puppy manager?¿

        Vector2 playerPos = player.transform.position;

        Steering();

        // Calculate new position
        Vector2 newPos = new Vector2(transform.position.x, transform.position.y) + currentSpeed * Time.deltaTime;

        // Update position
        transform.position = new Vector3(newPos.x, newPos.y, 0.0f);

        // MAKE PUPPY LOOK AT FOLLOW DIRECTION
    }

    // VISION CONE¿?


    void Steering()
    {
        Vector2 currentPos = transform.position;
        Vector2 playerPos = player.transform.position;

        Vector2 distanceVector = playerPos - currentPos;
        distanceVector.Normalize();

        Vector2 desiredSpeed = distanceVector * speed;


        

        Vector2 separationVector = new Vector2(0.0f, 0.0f);


        foreach(GameObject p in puppyList)
        {
            Vector2 distance = currentPos - new Vector2(p.transform.position.x, p.transform.position.y);
            if(distance.magnitude <= area){
                separationVector += distance;
            }
        }

        RaycastHit2D hit = Physics2D.Raycast(currentPos, currentSpeed.normalized, 1.5f);

        Vector2 wallAvoidance = new Vector2(0.0f, 0.0f);

        Color color = new Color(1.0f, 0.0f, 1.0f);

        if (hit.collider != null && hit.transform.gameObject.tag != "Puppy" && hit.transform.gameObject.tag != "Player")
        {

            Debug.Log("Avoid");

            wallAvoidance = currentSpeed - 2 * Vector2.Dot(currentSpeed, hit.normal) * hit.normal;
            Debug.DrawLine(hit.point, hit.point + wallAvoidance.normalized * 1.5f, color, 3.0f);
            
        }

        currentSpeed = currentSpeed + separationVector + wallAvoidance.normalized;

        Vector2 steering = desiredSpeed - currentSpeed;
        currentSpeed += steering * Time.deltaTime;

        color = new Color(0.0f, 0.0f, 1.0f);
        Vector2 vectorcito = currentSpeed;
        vectorcito.Normalize();

        Debug.DrawLine(currentPos, currentPos + vectorcito * 1.5f, color);


    }
}
