﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TortoiseMovement : MonoBehaviour
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

    // Distance to flee
    [Min(0.0f)]
    public float detectionArea = 10.0f;

    [SerializeField]
    private float wanderTime = 3.0f;

    private Vector2 currentSpeed;

    private bool isWandering;

    private bool isLookingRight = true;

    private Vector2 desiredSpeed;

    private float fleeDir = 1.0f;
    private Vector2 rightSide;
    private Vector2 leftSide;
    private Vector2 currentPos;
    private Vector2 playerPos;
    private Vector2 separationVector;
    private Vector2 distance;
    private Vector2 distanceVector;
    private Vector2 steering;
    private float wallAvoidDistance;
    private bool isDead = false;

    [SerializeField]
    private Sprite deathSprite = null;

    private AIStats aiStats;
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
        
    }

    private void Awake()
    {
        // ensrue that the first time the position is written
        GridAI.GetInstance().InitializePosition(this.gameObject, transform.position);
    }

    private Vector2 oldPosition;
    void Update()
    {
        if (isDead) return;

        oldPosition = transform.position;

        //Check if the puppy is dying
        if (animator.GetBool("IsDead"))
        {
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

        distanceVector = playerPos - currentPos;

        distanceVector.Normalize();

        if (Vector2.Distance(currentPos, playerPos) < detectionArea && aiStats.IsAvoidingPlayer) // Follow/flee player
        {
            desiredSpeed = distanceVector * speed;

            Vector3 aux = Quaternion.Euler(0, 0, 90) * desiredSpeed.normalized;

            desiredSpeed = aux * speed * fleeDir;
        }
        else if (!isWandering)
        {
            StartCoroutine(Wander(wanderTime));
        }

        separationVector = new Vector2(0.0f, 0.0f);

        /*foreach (GameObject p in aiManager.AIList)
        {
            distance = currentPos - new Vector2(p.transform.position.x, p.transform.position.y);
            if (distance.magnitude <= area)
            {
                separationVector += distance;
            }
        }*/

        // Get the closest positions of Grid to separate
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
        Vector2 leftSide = currentPos - perpendicular * 0.7f;
        Debug.DrawLine(rightSide, rightSide + currentSpeed.normalized * 3.0f, Color.yellow);
        Debug.DrawLine(leftSide, leftSide + currentSpeed.normalized * 3.0f, Color.yellow);

        Vector2 wallAvoidance = new Vector2(0.0f, 0.0f);
        RaycastHit2D rightHit = Physics2D.Raycast(rightSide, currentSpeed.normalized, wallAvoidDistance);
        RaycastHit2D leftHit = Physics2D.Raycast(leftSide, currentSpeed.normalized, wallAvoidDistance);
        if (rightHit.collider != null && rightHit.transform.gameObject.tag != "AI"
            && rightHit.transform.gameObject.tag != "Player")
        {
            wallAvoidance = currentSpeed - 2 * Vector2.Dot(currentSpeed, rightHit.normal) * rightHit.normal;

        }
        else if (leftHit.collider != null && leftHit.transform.gameObject.tag != "AI"
            && leftHit.transform.gameObject.tag != "Player")
        {
            wallAvoidance = currentSpeed - 2 * Vector2.Dot(currentSpeed, leftHit.normal) * leftHit.normal;
        }

        currentSpeed = currentSpeed + separationVector + wallAvoidance / 2.0f;

        steering = desiredSpeed - currentSpeed;
        currentSpeed += steering * Time.deltaTime;

    }

    IEnumerator Wander(float WaitTime)
    {
        isWandering = true;

        Vector3 aux = Quaternion.Euler(0, 0, Random.Range(-180.0f, 180.0f)) * currentSpeed.normalized;

        desiredSpeed = speed * new Vector2(aux.x, aux.y);

        float timeAtEnter = Time.time;
        while (WaitTime > Time.time - timeAtEnter)
        {
            yield return null;
        }

        isWandering = false;
    }

    private void OnDestroy()
    {
        GridAI.GetInstance().RemoveFromGrid(this.gameObject);
    }

    IEnumerator Die()
    {
        isDead = true;
        float timeAtStart = Time.time;

        Destroy(gameObject.GetComponent<AIAttack>());
        Destroy(gameObject.GetComponent<AIStats>());

        Collider2D[] collisionArray = gameObject.GetComponents<Collider2D>();
        foreach (Collider2D collider in collisionArray)
        {
            if (collider.isTrigger)
            {
                Destroy(collider);
            }
        }

        if (gameObject.GetComponent<AIAttack>().deadByHug)
        {
            AIManager.GetInstance().IncreaseHuggedTortoises();
        }

        float deathTimer = gameObject.GetComponent<AIAttack>().deathTimer;
        while (deathTimer > Time.time - timeAtStart)
        {
            yield return null;
        }
        //Sprite sólo caparazón
        gameObject.GetComponent<SpriteRenderer>().sprite = deathSprite;

        Destroy(gameObject.GetComponent<Animator>());
        Destroy(this);
    }
}
