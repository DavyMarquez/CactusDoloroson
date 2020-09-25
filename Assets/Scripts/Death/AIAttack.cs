using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class implements the functionality for the AI to "damage" the player
public class AIAttack : MonoBehaviour
{
    
    private AIStats aiStats;

    public float deathTimer = 2.0f;

    [SerializeField]
    private float smellTimer = 2.0f;

    public Animator animator;
    private GameObject player;
    public bool deadByHug = false;

    private PlayerStats playerStats;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        Collider2D[] collisionArray = gameObject.GetComponents<Collider2D>();
        foreach (Collider2D c in collisionArray)
        {
            if (!c.isTrigger)
            {
                Physics2D.IgnoreCollision(player.GetComponent<Collider2D>(), c);
            }
        }
        aiStats = gameObject.GetComponent<AIStats>();

        animator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision && collision.gameObject.tag == "Player")
        {
            playerStats = collision.gameObject.GetComponent<PlayerStats>();
            if (collision.gameObject.GetComponent<Hug>().invulnerable)
            {
                if (!collision.gameObject.GetComponent<Hug>().somethingHugged)
                {
                    collision.gameObject.GetComponent<Hug>().somethingHugged = true;
                    if (playerStats != null)
                    {
                        playerStats.IncreaseLove(aiStats.Love);
                        playerStats.IncreaseSorrow(aiStats.Sorrow);
                        playerStats.TimeSinceLastInteractionReset();
                        SkunkMovement skunk = gameObject.GetComponent<SkunkMovement>();
                        if (skunk != null)
                        {
                            playerStats.setAvoidingPlayer(true);
                            playerStats.RemoveSmellInTime(smellTimer);
                        }

                        deadByHug = true;
                        animator.SetBool("IsDead", true);
                    }
                }
            }
            else
            {
                if (gameObject.GetComponent<BearMovement>() != null)
                {
                    playerStats.IncreaseLove(aiStats.Love);
                }
                else
                {
                    playerStats.IncreaseSorrow(aiStats.Sorrow);
                }
                playerStats.TimeSinceLastInteractionReset();
                animator.SetBool("IsDead", true);
            }
            Collider2D[] collisionArray = gameObject.GetComponents<Collider2D>();
            foreach (Collider2D c in collisionArray)
            {
                if (!c.isTrigger)
                {
                    Physics2D.IgnoreCollision(player.GetComponent<Collider2D>(), c, false);
                    GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
                }
            }
        }
    }
}
