using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rigidBody;

    // player speed
    [Min(0)]
    public float speed = 10.0f;

    // getter and setter
    public float Speed
    {
        get { return speed; }
        set { speed = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();


    }

    // Update is called once per frame
    void Update()
    {
        // Get input direction
        Vector2 direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        //direction.Normalize();

        Debug.Log("input : " + direction);

        Vector2 pos = transform.position;

        // Move rigidbody
       // rigidBody.MovePosition(pos + direction * speed * Time.deltaTime);

        Vector2 newPos = new Vector2(pos.x, pos.y) + speed * direction * Time.deltaTime;

        //transform.position = new Vector3(newPos.x, newPos.y, 0.0f);


    }
}
