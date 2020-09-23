using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class implements the functionality for the AI to "damage" the player
public class AIAttack : MonoBehaviour
{
    [SerializeField]
    private Collider2D trigger;

    private AIStats aiStats;
    [SerializeField]
    private float deathTimer = 2.0f;

    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        Collider2D[] collisionArray = FindObjectsOfType<Collider2D>();
        foreach(Collider2D c in collisionArray)
        {
            if (c.isTrigger)
            {
                trigger = c;
                break;
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
                collision.gameObject.GetComponent<Hug>().hitWhileDashing = true;
                if (playerStats != null)
                {
                    
                    playerStats.IncreaseLove(aiStats.Love);
                }
            }
            playerStats.IncreaseSorrow(aiStats.Sorrow);
            
            StartCoroutine(Die());
        }
    }

    IEnumerator Die()
    {
        float timeAtStart = Time.time;
        animator.SetBool("IsDead", true);

        Destroy(gameObject.GetComponent<Rigidbody2D>());
        Destroy(gameObject.GetComponent<Collider2D>());

        while (deathTimer > Time.time - timeAtStart)
        {
            yield return null;
        }

        Destroy(gameObject);
    }
}
