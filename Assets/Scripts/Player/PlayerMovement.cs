using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [Min(1.0f)]
    public float buffSpeedMultiplayer = 2.0f;

    // player speed
    [Min(0)]
    public float speed = 10.0f;

    public Animator animator;

    private bool speedBuff = false;

    private float currentSpeed = 0.0f;

    [SerializeField]
    private float huggingTime = 0.21f;

    // getter and setter
    public float Speed
    {
        get { return speed; }
        set { speed = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // Get input direction
        Vector2 direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        direction.Normalize();

        // Calculate new position
        Vector2 newPos = new Vector2(transform.position.x, transform.position.y) + speed * direction * Time.deltaTime;

        // Update position
        //transform.position = new Vector3(newPos.x, newPos.y, 0.0f);
        Vector2 currentPos = transform.position;

        //Check the direction and if Julia is walking or not and make the proper animation
        if (direction.x != 0)
        {
            animator.SetBool("IsLookingRight", direction.x > 0 ? true : false);
            animator.SetBool("IsWalking", true);
        }
        else
        {
            animator.SetBool("IsWalking", direction.y != 0 ? true : false);
        }

        if (speedBuff)
        {
            currentSpeed = buffSpeedMultiplayer * speed;
        }
        else
        {
            currentSpeed = speed;
        }

        //Hug
        if (Input.GetKey("space"))
        {
            StartCoroutine(Hug());
        }

        GetComponent<Rigidbody2D>().MovePosition(currentPos + speed * direction * Time.deltaTime);
    }

    public void ApplySpeedBuff()
    {
        speedBuff = true;
    }

    public void RemoveSpeedBuff()
    {
        speedBuff = false;
    }
    
    //Coroutine for the Hug animation
    IEnumerator Hug()
    {
        animator.SetBool("IsHugging", true);
        float timeAtStart = Time.time;
        while(huggingTime > Time.time - timeAtStart)
        {
            yield return null;
        }
        animator.SetBool("IsHugging", false);
    }
}
