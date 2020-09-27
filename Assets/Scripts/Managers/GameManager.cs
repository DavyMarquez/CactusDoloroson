using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public GameObject player;

    private bool notifiedDead = false;

    // Start is called before the first frame update
    void Start()
    {
        // play music start here?
        notifiedDead = false;
    }

    // Update is called once per frame
    void Update()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if(!notifiedDead && player.GetComponent<Animator>().GetBool("IsDying"))
        {
            
            OnPlayerDeath();
        }
    }


    void OnPlayerDeath()
    {
        Debug.Log("muerte");
        notifiedDead = true;
        // play death music
    }

}
