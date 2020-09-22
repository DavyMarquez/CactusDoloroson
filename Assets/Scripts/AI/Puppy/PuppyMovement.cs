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
    private Vector2 prevSpeed;

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

        Vector2 currentPos = transform.position;
        Vector2 playerPos = player.transform.position;

        Steering();

        if(currentSpeed.magnitude > speed)
        {
            currentSpeed = currentSpeed.normalized * speed;
        }

        // Calculate new position
        Vector2 newPos = new Vector2(transform.position.x, transform.position.y) + currentSpeed * Time.deltaTime;

        // Update position
        transform.position = new Vector3(newPos.x, newPos.y, 0.0f);

       // GetComponent<Rigidbody2D>().MovePosition(currentPos + currentSpeed * Time.deltaTime);
        // MAKE PUPPY LOOK AT FOLLOW DIRECTION
        prevSpeed = currentSpeed;
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

        

        Vector2 wallAvoidance = new Vector2(0.0f, 0.0f);

        Vector3 auxPerp = Quaternion.Euler(0, 0, 90) * currentSpeed.normalized;

        Vector2 perpendicular = new Vector2(auxPerp.x, auxPerp.y);
        perpendicular.Normalize();

        //Debug.DrawLine(currentPos, currentPos + currentSpeed.normalized * 6.0f, Color.red);

        // Check sides possible collision
        Vector2 rightSide = currentPos + perpendicular * 0.7f;
        Vector2 leftSide = currentPos - perpendicular * 0.7f;
        //Debug.DrawLine(rightSide, rightSide + currentSpeed.normalized * 6.0f, Color.yellow);
        //Debug.DrawLine(leftSide, leftSide + currentSpeed.normalized * 6.0f, Color.yellow);

        RaycastHit2D rightHit = Physics2D.Raycast(rightSide, prevSpeed.normalized, 3.0f);
        RaycastHit2D leftHit = Physics2D.Raycast(leftSide, prevSpeed.normalized, 3.0f);
        if (rightHit.collider != null && rightHit.transform.gameObject.tag != "Puppy" 
            && rightHit.transform.gameObject.tag != "Player")
        {
            wallAvoidance = currentSpeed - 2 * Vector2.Dot(currentSpeed, rightHit.normal) * rightHit.normal;
            //Debug.DrawLine(rightHit.point, rightHit.point + wallAvoidance.normalized * 1.5f, Color.green);

        }
        else if(leftHit.collider != null && leftHit.transform.gameObject.tag != "Puppy"
            && leftHit.transform.gameObject.tag != "Player")
        {
            wallAvoidance = currentSpeed - 2 * Vector2.Dot(currentSpeed, leftHit.normal) * leftHit.normal;
            //Debug.DrawLine(leftHit.point, leftHit.point + wallAvoidance.normalized * 1.5f, Color.green);
        }
        
        currentSpeed = currentSpeed + separationVector + wallAvoidance / 2.0f;

        Vector2 steering = desiredSpeed - currentSpeed;
        currentSpeed += steering * Time.deltaTime;


    }
}
