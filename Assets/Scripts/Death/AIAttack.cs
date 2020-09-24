using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class implements the functionality for the AI to "damage" the player
public class AIAttack : MonoBehaviour
{
    
    private AIStats aiStats;

    [SerializeField]
    private float deathTimer = 2.0f;

    [SerializeField]
    private float smellTimer = 2.0f;

    public Animator animator;
    private GameObject player;

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

    void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.collider && collision.gameObject.tag == "Player")
        {
            PlayerStats playerStats = collision.gameObject.GetComponent<PlayerStats>();
            if (collision.gameObject.GetComponent<Hug>().invulnerable)
            {
                if (!collision.gameObject.GetComponent<Hug>().somethingHugged)
                {
                    collision.gameObject.GetComponent<Hug>().somethingHugged = true;
                    if (playerStats != null)
                    {
                        playerStats.IncreaseLove(aiStats.Love);
                        playerStats.IncreaseSorrow(aiStats.Sorrow);

                        StartCoroutine(Die());
                    }
                }


            }
            else
            {
                playerStats.IncreaseSorrow(aiStats.Sorrow);

                StartCoroutine(Die());
            }

        }
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
                        SkunkMovement skunk = gameObject.GetComponent<SkunkMovement>();
                        if (skunk != null)
                        {
                            playerStats.setAvoidingPlayer(true);
                            playerStats.RemoveSmellInTime(smellTimer);
                        }

                        StartCoroutine(Die());
                    }
                }


            }
            else
            {
                playerStats.IncreaseSorrow(aiStats.Sorrow);
                StartCoroutine(Die());
            }

        }
    }

    IEnumerator Die()
    {
        float timeAtStart = Time.time;
        if (animator != null)
        {
            animator.SetBool("IsDead", true);
        }
        else
        {
            Destroy(gameObject);
        }

        Destroy(gameObject.GetComponent<Rigidbody2D>());
        Collider2D[] collisionArray = gameObject.GetComponents<Collider2D>();
        foreach (Collider2D collider in collisionArray)
        {
            Destroy(collider);
        }

        while (deathTimer > Time.time - timeAtStart)
        {
            yield return null;
        }

        Destroy(gameObject);
    }

}
