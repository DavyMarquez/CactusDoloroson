using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hug : MonoBehaviour
{
    [SerializeField]
    private float huggingTime = 0.21f;

    [Min(0.0f)]
    public float dashDistance = 3.0f;

    [Min(0.0f)]
    public float dashTime = 0.5f;

    private float dashSpeed;

    [Range(0.0f, 100.0f)]
    public float hugFailPenalitation = 5.0f;

    private PlayerStats playerStats;

    private PlayerMovement playerMovement;

    private bool hugging = false;

    private Animator animator;

    private bool dashing = false;

    public bool invulnerable = false;

    public bool hitWhileDashing = false;

    // Start is called before the first frame update
    void Start()
    {
        playerStats = gameObject.GetComponent<PlayerStats>();
        playerMovement = gameObject.GetComponent<PlayerMovement>();
        animator = gameObject.GetComponent<Animator>();
        dashSpeed = dashDistance / dashTime;
    }

    // Update is called once per frame
    void Update()
    {
        //Hug
        if (Input.GetKey("space") && !dashing)
        {
           
            StartCoroutine(OnDash());
        }

        //
    }

    //Coroutine for the Hug animation
    IEnumerator OnHug()
    {
        float timeAtStart = Time.time;
        //gameObject.GetComponent<Collider2D>().enabled = false;
        Vector2 start = gameObject.transform.position;
        RaycastHit2D hit = Physics2D.Raycast(start, playerMovement.Direction.normalized, 1.5f);
        Debug.DrawLine(start, start + playerMovement.Direction.normalized * 1.5f, Color.green);
        if (hit.transform && hit.transform.gameObject != null && 
            hit.transform.gameObject.GetComponent<AIStats>() != null)
        {
            Debug.Log("Something Hugged");
        }
        else
        {
            invulnerable = false;
        }
        animator.SetBool("IsHugging", true);
        
        while (huggingTime > Time.time - timeAtStart)
        {
            yield return null;
        }
        animator.SetBool("IsHugging", false);
    }

    IEnumerator OnDash()
    {
        hitWhileDashing = false;
        dashing = true;
        invulnerable = true;
        playerMovement.Dashing(dashSpeed);
        float timeAtStart = Time.time;
        while (dashTime > Time.time - timeAtStart)
        {
            if (hitWhileDashing)
            {
                break;
            }
            yield return null;
        }
        playerMovement.NoDashing();
        dashing = false;
        StartCoroutine(OnHug());
    }


}
