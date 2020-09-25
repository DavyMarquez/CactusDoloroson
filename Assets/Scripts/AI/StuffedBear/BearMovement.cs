using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearMovement : MonoBehaviour
{

    public float speed = 6.0f;

    public float bounceDistance = 3.0f;
    private Vector2 direction;

    public Animator animator;
    private bool isLookingRight = true;
    private Vector2 newPos = new Vector2(0.0f, 0.0f);
    private bool isDead = false;

    // Start is called before the first frame update
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        direction = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
        
        direction.Normalize();
        /*GetComponent<Renderer>().material.color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f),
            Random.Range(0.0f, 1.0f));*/
    }

    // Update is called once per frame
    void Update()
    {

        if (isDead) return;

        //Check if the puppy is dying
        if (animator.GetBool("IsDead"))
        {
            StartCoroutine(Die());
            return;
        }

        Vector2 start = transform.position;
        Debug.DrawLine(start, start + direction.normalized * 1.5f, new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f),
            Random.Range(0.0f, 1.0f)));
        RaycastHit2D hit = Physics2D.Raycast(start, direction, bounceDistance);
        
        if (hit.collider != null /*&&  hit.transform.gameObject.tag != "AI"*/)
        {
            direction = 2 * Vector2.Dot(hit.normal, -1.0f * direction) * hit.normal + direction;
            Debug.DrawLine(hit.point, hit.point + direction.normalized * 1.5f, Color.red);
        }

        FlipSprite();

        // Calculate new position
        newPos = new Vector2(transform.position.x, transform.position.y) +  direction * speed * Time.deltaTime;

        //transform.position = new Vector3(newPos.x, newPos.y, 0.0f);

        GetComponent<Rigidbody2D>().MovePosition(transform.position  + 
            speed * new Vector3(direction.x, direction.y, 0.0f) * Time.deltaTime);
    }

    void FlipSprite()
    {
        if (direction.x > 0)
        {
            if (!isLookingRight)
            {
                transform.localScale = new Vector3(transform.localScale.x * -1.0f, transform.localScale.y, transform.localScale.z);
                isLookingRight = true;
            }
        }
        else if (direction.x < 0)
        {
            if (isLookingRight)
            {
                transform.localScale = new Vector3(transform.localScale.x * -1.0f, transform.localScale.y, transform.localScale.z);
                isLookingRight = false;
            }
        }
    }

    IEnumerator Die()
    {
        isDead = true;
        float timeAtStart = Time.time;

        AIManager.GetInstance().IncreaseHuggedBears();

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
