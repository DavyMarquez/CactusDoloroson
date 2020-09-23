using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class implements the functionality for the AI to "damage" the player
public class AIAttack : MonoBehaviour
{
    [SerializeField]
    private Collider2D trigger;
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Destroy(gameObject);
        }
    }
}
