using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkunkMovement : MonoBehaviour
{

    AIManager aiManager;

    /*[SerializeField]
    private GameObject player;*/

    public Animator animator;

    [Min(0.0f)]
    public float checkWallDistance = 2f;

    // Skunk speed
    [Min(0.0f)]
    public float speed = 3.0f;

    private Vector2 currentSpeed;
    private bool wandering = false;
    private Vector2 newPos;
    
    void Start()
    {
        aiManager = FindObjectOfType<AIManager>();
        if (aiManager == null)
        {
            Debug.LogError("No AIManager found in scene");
        }

        // Add this gameobject to ai list
        aiManager.AddAI(gameObject);

        //player = GameObject.FindGameObjectWithTag("Player");

        float sqrtSpeed = Mathf.Sqrt(speed);
        currentSpeed = new Vector2(Random.Range(-sqrtSpeed, sqrtSpeed), Random.Range(-sqrtSpeed, sqrtSpeed));

        animator = gameObject.GetComponent<Animator>();

    }

    void Update()
    {

        //if (animator.GetBool("IsDead")) return;

        if (!wandering)
        {
            StartCoroutine(Wander(3));
        }

        if (currentSpeed.magnitude > speed)
        {
            currentSpeed = currentSpeed.normalized * speed;
        }

        Vector2 start = transform.position;
        //Debug.DrawLine(start, start + currentSpeed.normalized * 1.5f, new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f)));
        RaycastHit2D hit = Physics2D.Raycast(start, currentSpeed, checkWallDistance);

        if (hit.collider != null && hit.transform.gameObject.tag != "AI" && hit.transform.gameObject.tag != "Player")
        {
            currentSpeed = (currentSpeed * -1) * 2;
        }

        //Check if the movement direction has changed
        //FlipSprite();

        // Calculate new position
        newPos = new Vector2(transform.position.x, transform.position.y) + currentSpeed * Time.deltaTime;

        // Update position
        transform.position = new Vector3(newPos.x, newPos.y, 0.0f);
    }

    IEnumerator Wander(float WaitTime)
    {
        wandering = true;

        float rot = Random.Range(-180.0f, 180.0f);
        Vector3 aux = Quaternion.Euler(0f, 0f, rot) * currentSpeed.normalized;

        float timeAtEnter = Time.time;
        while (WaitTime > Time.time - timeAtEnter)
        {
            yield return null;

        }
        //currentSpeed = desiredSpeed;
        currentSpeed = speed * new Vector2(aux.x, aux.y);
        wandering = false;
    }

    private void OnDestroy()
    {
        aiManager.RemoveAI(gameObject);
    }

}
