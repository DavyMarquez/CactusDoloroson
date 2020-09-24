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
    
    [Min(0.0f)]
    public float dashBuffedDistance = 3.0f;

    [Min(0.0f)]
    public float dashBuffedTime = 0.5f;
    
    [SerializeField]
    private float dashSpeed;

    [Range(0.0f, 100.0f)]
    public float hugFailPenalitation = 5.0f;

    private PlayerStats playerStats;

    private PlayerMovement playerMovement;

    private Animator animator;

    private bool dashing = false;

    public bool invulnerable = false;

    public bool hitWhileDashing = false;

    public bool somethingHugged = false;

    public bool directionHasChanged = false;

    public bool dashBuff = false;

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
        
    }

    //Coroutine for the Hug animation
    IEnumerator OnHug()
    {
        float timeAtStart = Time.time;
        if (!somethingHugged){
            invulnerable = false;
        }
        animator.SetBool("IsHugging", true);

        bool isLookingRight = animator.GetBool("IsLookingRight");
        
        while (huggingTime > Time.time - timeAtStart)
        {
            if (isLookingRight != animator.GetBool("IsLookingRight"))
            {
                if (isLookingRight)
                {
                    //Hug animation looking left
                    animator.CrossFadeInFixedTime("HugLeft", 0, -1, Time.time - timeAtStart);
                }
                else
                {
                    //Hug animation looking right
                    animator.CrossFadeInFixedTime("HugRight", 0, -1, Time.time - timeAtStart);

                }
                isLookingRight = animator.GetBool("IsLookingRight");
            }
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
        float dashTimeAux = dashTime;
        if (dashBuff)
        {
            playerMovement.Dashing(dashBuffedDistance / dashBuffedTime);
            dashTimeAux = dashBuffedTime;
        }
        else
        {
            playerMovement.Dashing(dashSpeed);
        }
        
        float timeAtStart = Time.time;
        while (dashTimeAux > Time.time - timeAtStart)
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

    public void ApplyDashBuff()
    {
        dashBuff = true;
    }

}
