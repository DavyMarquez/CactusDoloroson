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

    [Range(0.0f, 1.0f)]
    public float transparencyWhileDashing = 1.0f;

    public bool reverseTransparencyAnimation = false;

    public ParticleSystem dust;

    [Min(0)]
    public float hugCoolDown = 0.0f;
    private float timeLastHug = 0.0f;

    private PlayerStats playerStats;

    private PlayerMovement playerMovement;

    private Animator animator;

    //Audio
    private AudioSource source;
    public AudioClip otherClip;
    //Fin del audio

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
        timeLastHug = hugCoolDown;
        source = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        //Hug
        timeLastHug += Time.deltaTime;
        if (Input.GetKeyDown("space") && !dashing && timeLastHug >= hugCoolDown)
        {
            StartCoroutine(OnDash());
        }
        
    }

    //Coroutine for the Hug animation
    IEnumerator OnHug()
    {
        source.clip = otherClip;
        source.Play();

        if (!somethingHugged){
            invulnerable = false;
            playerStats.IncreaseSorrow(hugFailPenalitation);
        }

        float timeAtStart = Time.time;

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
        timeLastHug = 0.0f;
        dashing = false;
    }

    IEnumerator OnDash()
    {
        PlayDust();
        invulnerable = true;
        dashing = true;
        somethingHugged = false;
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

        animator.SetBool("IsDashing", true);

        float timeAtStart = Time.time;
        float vanishingProportion = transparencyWhileDashing / dashTimeAux;

        Color color;

        float timeRightNow;

        while (dashTimeAux > Time.time - timeAtStart)
        {
            timeRightNow = Time.time - timeAtStart;
            if (reverseTransparencyAnimation)
            {
                color = new Color(1, 1, 1, 1 - vanishingProportion * timeRightNow);
            }
            else
            {
                color = new Color(1, 1, 1, 1 - transparencyWhileDashing + vanishingProportion * timeRightNow);
            }
            gameObject.GetComponent<SpriteRenderer>().color = color;

            if (somethingHugged)
            {
                
                break;
            }
            yield return null;
        }
        playerMovement.NoDashing();
        animator.SetBool("IsDashing", false);
        color = new Color(1, 1, 1, 1);
        gameObject.GetComponent<SpriteRenderer>().color = color;
        StartCoroutine(OnHug());
    }

    public void ApplyDashBuff()
    {
        dashBuff = true;
    }
    
    public void RemoveDashBuff()
    {
        dashBuff = false;
    }

    void PlayDust()
    {
        dust.Play();
    }
}
