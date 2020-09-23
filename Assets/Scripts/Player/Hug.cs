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

    public bool somethingHugged = false;

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
        if (!somethingHugged){
            invulnerable = false;
        }
        animator.SetBool("IsHugging", true);
        
        while (huggingTime > Time.time - timeAtStart)
        {
            yield return null;
        }
        animator.SetBool("IsHugging", false);
        invulnerable = false;
        somethingHugged = false;
    }

    IEnumerator OnDash()
    {
        somethingHugged = false;
        dashing = true;
        invulnerable = true;
        playerMovement.Dashing(dashSpeed);
        float timeAtStart = Time.time;
        while (dashTime > Time.time - timeAtStart)
        {
            if (somethingHugged)
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
