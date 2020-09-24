using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;
using UnityEditor;

public class PuppyMovement : MonoBehaviour
{
    AIManager aiManager;

    [SerializeField]
    private GameObject player;

    public Animator animator;

    // Area of effect of separation from others puppies 
    [Min(0.0f)]
    public float area = 1.5f;

    // Puppy speed
    [Min(0.0f)]
    public float speed = 6.0f;

    // Distance to follow or flee
    [Min(0.0f)]
    public float detectionArea = 10.0f;

    private float fleeDir = 1.0f;
    private bool wandering = false;

    // The direction the puppy is looking at
    private bool isLookingRight;

    private Vector2 currentSpeed;
    private Vector2 desiredSpeed;
    private Vector2 currentPos;
    private Vector2 playerPos;
    // Distance between AI and player
    private Vector2 distanceVector;
    // Aggregated sum of separation between AIs
    private Vector2 separationVector;
    // For smooth turns
    private Vector2 steering;

    private Vector2 rightSide;
    private Vector2 leftSide;
    private Vector2 wallAvoidance;
    private AIStats aiStats;
    // Start is called before the first frame update
    void Start()
    {
        aiManager = FindObjectOfType<AIManager>();
        if (aiManager == null)
        {
            Debug.LogError("No AIManager found in scene");
        }
        // Add this gameobject to ai list
        aiManager.AddAI(gameObject);

        player = GameObject.FindGameObjectWithTag("Player");

        if (Random.Range(0.0f, 1.0f) > 0.5f)
        {
            fleeDir = -1.0f;
        }
        float sqrtSpeed = Mathf.Sqrt(speed);
        currentSpeed = new Vector2(Random.Range(-sqrtSpeed, sqrtSpeed), Random.Range(-sqrtSpeed, sqrtSpeed));

        animator = gameObject.GetComponent<Animator>();
        aiStats = gameObject.GetComponent<AIStats>();
    }

    // Update is called once per frame
    void Update()
    {

        //Check if the puppy is dying
        if (animator.GetBool("IsDead")) return;

        Steering();

        if (currentSpeed.magnitude > speed)
        {
            currentSpeed = currentSpeed.normalized * speed;
        }

        //Check if the movement direction has changed
        FlipSprite();

        // Calculate new position
        Vector2 newPos = new Vector2(transform.position.x, transform.position.y) + currentSpeed * Time.deltaTime;

        // Update position
        transform.position = new Vector3(newPos.x, newPos.y, 0.0f);
    }

    // Flips the sprite if it changes its direction
    void FlipSprite()
    {
        if (currentSpeed.x > 0)
        {
            if (!isLookingRight)
            {
                transform.localScale = new Vector3(transform.localScale.x * -1.0f, transform.localScale.y, transform.localScale.z);
            }
            isLookingRight = true;
        }
        else if (currentSpeed.x < 0)
        {
            if (isLookingRight)
            {
                transform.localScale = new Vector3(transform.localScale.x * -1.0f, transform.localScale.y, transform.localScale.z);
            }
            isLookingRight = false;
        }
    }

    void Steering()
    {
        currentPos = transform.position;
        playerPos = player.transform.position;

        Color color = Color.red;

        distanceVector = playerPos - currentPos;

        distanceVector.Normalize();

        if (Vector2.Distance(currentPos, playerPos) > detectionArea) // Follow/flee player
        {
            if (!wandering)
            {
                StartCoroutine(Wander(2));
            }
            else
            {
                color = Color.green;
            }
        }
        else {
            desiredSpeed = distanceVector * speed;
            //dont follow the player
            if (aiStats.IsAvoidingPlayer)
            {
                color = Color.yellow;
                Vector3 aux = Quaternion.Euler(0, 0, 90) * desiredSpeed.normalized;

                desiredSpeed = aux * speed * fleeDir;

            }
        }

        separationVector = new Vector2(0.0f, 0.0f);

        Vector2 distance;
        foreach (GameObject p in aiManager.AIList)
        {
            distance = currentPos - new Vector2(p.transform.position.x, p.transform.position.y);
            if(distance.magnitude <= area){
                separationVector += distance;
            }
        }

        Vector3 auxPerp = Quaternion.Euler(0, 0, 90) * currentSpeed.normalized;

        Vector2 perpendicular = new Vector2(auxPerp.x, auxPerp.y);
        perpendicular.Normalize();


        // Check sides possible collision
        rightSide = currentPos + perpendicular * 0.7f;
        leftSide = currentPos - perpendicular * 0.7f;
        Debug.DrawLine(rightSide, rightSide + currentSpeed.normalized * 6.0f, color);
        Debug.DrawLine(leftSide, leftSide + currentSpeed.normalized * 6.0f, color);

        wallAvoidance = new Vector2(0.0f, 0.0f);
        RaycastHit2D rightHit = Physics2D.Raycast(rightSide, currentSpeed.normalized, 3.0f);
        RaycastHit2D leftHit = Physics2D.Raycast(leftSide, currentSpeed.normalized, 3.0f);
        if (rightHit.collider != null && rightHit.transform.gameObject.tag != "AI" 
            && rightHit.transform.gameObject.tag != "Player")
        {
            wallAvoidance = currentSpeed - 2 * Vector2.Dot(currentSpeed, rightHit.normal) * rightHit.normal;
            //Debug.DrawLine(rightHit.point, rightHit.point + wallAvoidance.normalized * 1.5f, Color.green);

        }
        else if(leftHit.collider != null && leftHit.transform.gameObject.tag != "AI"
            && leftHit.transform.gameObject.tag != "Player")
        {
            wallAvoidance = currentSpeed - 2 * Vector2.Dot(currentSpeed, leftHit.normal) * leftHit.normal;
            //Debug.DrawLine(leftHit.point, leftHit.point + wallAvoidance.normalized * 1.5f, Color.green);
        }
        
        currentSpeed = currentSpeed + separationVector + wallAvoidance / 2.0f;

        steering = desiredSpeed - currentSpeed;
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

    private void OnDestroy()
    {
        aiManager.RemoveAI(gameObject);
    }
}
