using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Jump : MonoBehaviour
{
    // Game Object needs a RigidBody2D to move.  

    // Private attributes
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private float initialJumpForce = 4.0f;
    private bool grounded = false;
    private bool safeToJump = false;    
    private bool moveDirLeft = true;
    private const float jumpCooldown = 2.0f;
    private float jumpTimer = jumpCooldown;
    private bool isJumping = false;
    private Vector2 velocity;
    private const float minYVel = -10.0f;
    private const float gravAcc = 9.81f;
    private int damageAmount = 1;
    private float hitForce = 5.0f;
    private int wallLayerMask;

    // private functions
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        sr.flipX = true;
        wallLayerMask = LayerMask.GetMask("Wall");
    }
    private void Update()
    {
        Move();
    }
    private void Move()
    {
        Vector3 pos = transform.position;
        velocity = new Vector2(velocity.x, velocity.y);
        grounded = Physics2D.Raycast(transform.position, Vector3.down, 0.36f, wallLayerMask) && velocity.y <= 0.0f;
        safeToJump = Physics2D.Raycast(transform.position + new Vector3(8.0f, 0, 0), transform.TransformDirection(Vector3.down), 1, wallLayerMask);

        if (SafeToMove())
        {
            if (moveDirLeft)
                velocity.x = 1.0f;
            else
                velocity.x = -1.0f;          
                
        }
        if (!grounded)
        {
            ApplyGravity();
        }
        else
        {
            if (isJumping)
            {
                isJumping = false;
                jumpTimer = jumpCooldown;
            }
            else
                jumpTimer -= Time.deltaTime;

            if (jumpTimer <= 0.0f)
            {
                velocity.y = initialJumpForce;
                isJumping = true;
            }
            else
                velocity.y = 0.0f;
        }
        pos.x += velocity.x * Time.deltaTime;
        pos.y += velocity.y * Time.deltaTime;
        transform.position = pos;

    }
    private bool SafeToMove()
    {
        if (grounded)
        {
            if (moveDirLeft)
            {
                //Debug.DrawRay(transform.position + new Vector3(0.5f, 0, 0), transform.TransformDirection(Vector3.down), Color.white);
                if (Physics2D.Raycast(transform.position + new Vector3(0.5f, 0, 0), transform.TransformDirection(Vector3.down), 1, wallLayerMask))
                {                    
                    return true;
                }
                else
                {                    
                    safeToJump = false;
                    moveDirLeft = false;
                    sr.flipX = false;
                    return false;
                }
            }
            else
            {
                //Debug.DrawRay(transform.position + new Vector3(-0.5f, 0, 0), transform.TransformDirection(Vector3.down), Color.white);
                if (Physics2D.Raycast(transform.position + new Vector3(-0.5f, 0, 0), transform.TransformDirection(Vector3.down), 1, wallLayerMask))
                {
                    return true;
                }
                else
                {
                    moveDirLeft = true;
                    sr.flipX = true;
                    return false;
                }
            }
        }
        else
            return false;
    }
    private void ApplyGravity()
    {
        velocity.y -= gravAcc * Time.deltaTime;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<Player>().Hit(transform.position, hitForce, damageAmount);
        }
    }

    // Public functions
    public void Hit()
    {
        //DropLoot()
        Destroy(this.gameObject);
    }
}
