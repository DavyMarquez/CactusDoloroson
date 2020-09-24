using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearMovement : MonoBehaviour
{

    public float speed = 6.0f;

    public float bounceDistance = 3.0f;
    private Vector2 direction;

    public Animator animator;


    // Start is called before the first frame update
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        direction = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
        
        direction.Normalize();
        GetComponent<Renderer>().material.color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f),
            Random.Range(0.0f, 1.0f));
    }

    // Update is called once per frame
    void Update()
    {

        if (animator != null && animator.GetBool("IsDead")) return;

        Vector2 start = transform.position;
        Debug.DrawLine(start, start + direction.normalized * 1.5f, new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f),
            Random.Range(0.0f, 1.0f)));
        RaycastHit2D hit = Physics2D.Raycast(start, direction, bounceDistance);
        
        if (hit.collider != null /*&&  hit.transform.gameObject.tag != "AI"*/)
        {
            direction = 2 * Vector2.Dot(hit.normal, -1.0f * direction) * hit.normal + direction;
            Debug.DrawLine(hit.point, hit.point + direction.normalized * 1.5f, Color.red);
        }

        // Calculate new position
        Vector2 newPos = new Vector2(transform.position.x, transform.position.y) +  direction * speed * Time.deltaTime;

        transform.position = new Vector3(newPos.x, newPos.y, 0.0f);

    }
}
