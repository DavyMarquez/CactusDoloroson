using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;
using UnityEditor;

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

    // Distance to follow or flee
    public float detectionArea = 10.0f;

    private Vector2 currentSpeed;
    private Vector2 desiredSpeed;

    private bool follow = true;

    private float fleeDir = 1.0f;
    private bool wandering = false;

    // Start is called before the first frame update
    void Start()
    {
        puppyList = GameObject.FindGameObjectsWithTag("Puppy").ToList();
        puppyList.Remove(gameObject);
        player = GameObject.FindGameObjectWithTag("Player");

        if (Random.Range(0.0f, 1.0f) > 0.5f)
        {
            fleeDir = -1.0f;
        }
        float sqrtSpeed = Mathf.Sqrt(speed);
        currentSpeed = new Vector2(Random.Range(-sqrtSpeed, sqrtSpeed), Random.Range(-sqrtSpeed, sqrtSpeed));
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKey("space") )
        {
            follow = !follow;
        }

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
    }

    // VISION CONE¿?


    void Steering()
    {
        Vector2 currentPos = transform.position;
        Vector2 playerPos = player.transform.position;

        Vector2 distanceVector = playerPos - currentPos;

        distanceVector.Normalize();

        if (Vector2.Distance(currentPos, playerPos) > detectionArea) // Follow/flee player
        {
            if (!wandering)
            {
                StartCoroutine(Wander(2));
            }
        }
        else {
            desiredSpeed = distanceVector * speed;
            if (!follow)
            {
                Vector3 aux = Quaternion.Euler(0, 0, 90) * desiredSpeed.normalized;

                desiredSpeed = aux * speed * fleeDir;

            }
        }

        Vector2 separationVector = new Vector2(0.0f, 0.0f);


        foreach(GameObject p in puppyList)
        {
            Vector2 distance = currentPos - new Vector2(p.transform.position.x, p.transform.position.y);
            if(distance.magnitude <= area){
                separationVector += distance;
            }
        }

        Vector3 auxPerp = Quaternion.Euler(0, 0, 90) * currentSpeed.normalized;

        Vector2 perpendicular = new Vector2(auxPerp.x, auxPerp.y);
        perpendicular.Normalize();

        //Debug.DrawLine(currentPos, currentPos + currentSpeed.normalized * 6.0f, Color.red);

        // Check sides possible collision
        Vector2 rightSide = currentPos + perpendicular * 0.7f;
        Vector2 leftSide = currentPos - perpendicular * 0.7f;
        Debug.DrawLine(rightSide, rightSide + currentSpeed.normalized * 6.0f, Color.yellow);
        Debug.DrawLine(leftSide, leftSide + currentSpeed.normalized * 6.0f, Color.yellow);

        Vector2 wallAvoidance = new Vector2(0.0f, 0.0f);
        RaycastHit2D rightHit = Physics2D.Raycast(rightSide, currentSpeed.normalized, 3.0f);
        RaycastHit2D leftHit = Physics2D.Raycast(leftSide, currentSpeed.normalized, 3.0f);
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

    IEnumerator Wander(float WaitTime)
    {
        wandering = true;

        Vector3 aux = Quaternion.Euler(0, 0, Random.Range(-180.0f, 180.0f)) * currentSpeed.normalized;

        desiredSpeed = speed * new Vector2(aux.x, aux.y);

        float timeAtEnter = Time.time;
        while (WaitTime > Time.time - timeAtEnter)
        {
            yield return null;
           
        }

        wandering = false;
    }
}
