using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class PlayerMovement : MonoBehaviour
{

    [Min(1.0f)]
    public float buffSpeedMultiplayer = 2.0f;

    // player speed
    [Min(0)]
    public float speed = 10.0f;

    private Animator animator;

    private bool speedBuff = false;

    private float currentSpeed = 0.0f;

    private float dashSpeed = 0.0f;

    private bool dashing = false;

    private Vector2 direction;
    public Vector2 Direction
    {
        get { return direction; }
    }

    private bool isLookingRight = false;

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
        isLookingRight = animator.GetBool("IsLookingRight");
        speedBuff = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Update position
        Vector2 currentPos = transform.position;

        if (dashing)
        {
            Vector2 dashDir = direction;
            if(dashDir.x == 0.0f && dashDir.y == 0.0f)
            {
                dashDir = GetFacingDirection();
            }
            GetComponent<Rigidbody2D>().MovePosition(currentPos + dashSpeed * dashDir * Time.deltaTime);
            return;
        }

        // Get input direction
        direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        direction.Normalize();



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

        GetComponent<Rigidbody2D>().MovePosition(currentPos + currentSpeed * direction * Time.deltaTime);
    }

    public void ApplySpeedBuff()
    {
        speedBuff = true;
    }

    public void RemoveSpeedBuff()
    {
        speedBuff = false;
    }
    

    public void Dashing(float newSpeed)
    {
        dashing = true;
        dashSpeed = newSpeed;
    }

    public void NoDashing()
    {
        dashing = false;
    }

    Vector2 GetFacingDirection()
    {
        if (animator.GetBool("IsLookingRight"))
        {
            return new Vector2(1.0f, 0.0f);
        }
        else
        {
            return new Vector2(-1.0f, 0.0f);
        }
    }
}
