using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;
using UnityEditor;

public class PuppyMovement : MonoBehaviour
{
    //private AIManager aiManager;

    [SerializeField]
    private GameObject player;

    private Animator animator;

    // Area of effect of separation from others puppies 
    [Min(0.0f)]
    public float area = 1.5f;

    // Puppy speed
    [Min(0.0f)]
    public float speed = 6.0f;

    // Distance to follow or flee
    [Min(0.0f)]
    public float detectionArea = 10.0f;

    [SerializeField]
    private float wanderTime = 2.0f;

    private float fleeDir = 1.0f;
    private bool wandering = false;

    // The direction the puppy is looking at
    private bool isLookingRight = false;

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
    private Vector2 distance;
    private float wallAvoidDistance;
    private bool isDead = false;

    private AIStats aiStats;
    // Start is called before the first frame update
    void Start()
    {
        /*aiManager = FindObjectOfType<AIManager>();
        if (aiManager == null)
        {
            Debug.LogError("No AIManager found in scene");
        }
        // Add this gameobject to ai list
        aiManager.AddAI(gameObject);*/

        player = GameObject.FindGameObjectWithTag("Player");

        if (Random.Range(0.0f, 1.0f) > 0.5f)
        {
            fleeDir = -1.0f;
        }
        float sqrtSpeed = Mathf.Sqrt(speed);
        currentSpeed = new Vector2(Random.Range(-sqrtSpeed, sqrtSpeed), Random.Range(-sqrtSpeed, sqrtSpeed));

        animator = gameObject.GetComponent<Animator>();
        aiStats = gameObject.GetComponent<AIStats>();

        Collider2D[] collisionArray = gameObject.GetComponents<CircleCollider2D>();
        foreach (CircleCollider2D c in collisionArray)
        {
            if (!c.isTrigger)
            {
                wallAvoidDistance = c.radius * 1.5f;
            }
        }

        // Debug.Log("Initial: " + GridAI.GetInstance().GetNumAIs());

    }

    private void Awake()
    {
        // ensrue that the first time the position is written
        GridAI.GetInstance().InitializePosition(this.gameObject, transform.position);
    }

    private Vector2 oldPosition;

    // Update is called once per frame
    void Update()
    {
        if (isDead) return;

        oldPosition = transform.position;

        //Check if the puppy is dying
        if (animator.GetBool("IsDead")) {
            StartCoroutine(Die());
            return;
        }

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

        //GetComponent<Rigidbody2D>().MovePosition(currentPos + currentSpeed * Time.deltaTime);


        GridAI.GetInstance().UpdatePosition(this.gameObject, oldPosition, newPos);
    }

    // Flips the sprite if it changes its direction
    void FlipSprite()
    {
        if (currentSpeed.x > 0)
        {
            if (!isLookingRight)
            {
                transform.localScale = new Vector3(transform.localScale.x * -1.0f, transform.localScale.y, transform.localScale.z);
                isLookingRight = true;
            }
        }
        else if (currentSpeed.x < 0)
        {
            if (isLookingRight)
            {
                transform.localScale = new Vector3(transform.localScale.x * -1.0f, transform.localScale.y, transform.localScale.z);
                isLookingRight = false;
            }
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
                StartCoroutine(Wander(wanderTime));
            }
            else
            {
                color = Color.green;
            }
        }
        else {
            desiredSpeed = distanceVector * speed;
            if (player.GetComponent<PlayerStats>().Smell)
            {
                color = Color.yellow;
                Vector3 aux = Quaternion.Euler(0, 0, 90) * desiredSpeed.normalized;

                desiredSpeed = aux * speed * fleeDir;

            }
        }

        separationVector = new Vector2(0.0f, 0.0f);

        // iterate all elements in array
        /*foreach (GameObject p in aiManager.AIList)
        {
            distance = currentPos - new Vector2(p.transform.position.x, p.transform.position.y);
            if(distance.magnitude <= area)
            {
                separationVector += distance;
            }
        }*/

        /*
        int i = 0;
        Collider2D[] overlap = Physics2D.OverlapCircleAll(transform.position, area, LayerMask.NameToLayer("AI"));
        foreach(Collider2D c in overlap)
        {
            i += 1;
            distance = currentPos - new Vector2(c.transform.position.x, c.transform.position.y);
            separationVector += distance;

        }
        */

        // Get the closest positions to steer
        //foreach (GameObject go in GridAI.GetInstance().GetClosePositions(currentPos))
        foreach (GameObject go in GridAI.GetInstance().GetClosePositions(currentPos, area))
        {
            distance = currentPos - new Vector2(go.transform.position.x, go.transform.position.y);
            separationVector += distance;
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
        RaycastHit2D rightHit = Physics2D.Raycast(rightSide, currentSpeed.normalized, wallAvoidDistance);
        RaycastHit2D leftHit = Physics2D.Raycast(leftSide, currentSpeed.normalized, wallAvoidDistance);
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
        GridAI.GetInstance().RemoveFromGrid(this.gameObject);
    }

    IEnumerator Die()
    {
        isDead = true;
        float timeAtStart = Time.time;
        if (gameObject.GetComponent<AIAttack>().deadByHug)
        {
            AIManager.GetInstance().IncreaseHuggedPuppies();
        }

        Vector3 aux = new Vector3(transform.position.x, transform.position.y, 1);
        transform.position = aux;

        Destroy(gameObject.GetComponent<Rigidbody2D>());
        Collider2D[] collisionArray = gameObject.GetComponents<Collider2D>();
        foreach (Collider2D collider in collisionArray)
        {
            Destroy(collider);
        }

        float deathTimer = gameObject.GetComponent<AIAttack>().deathTimer;
        while (deathTimer > Time.time - timeAtStart)
        {
            yield return null;
        }

        Destroy(gameObject);
    }
}
