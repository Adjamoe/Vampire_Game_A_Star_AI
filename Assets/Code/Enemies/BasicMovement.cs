using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BasicMovement : MonoBehaviour 
{
    private LootDropsManager ldm;
    protected Vector2 velocity;
    protected SpriteRenderer sr;
    protected Vector2 size;
    protected Vector2 halfSize;
    protected bool moveDirLeft = false;
    protected float maxXVel = 3.0f;
    private float maxYVel = 10.0f;
    protected const float gravAcc = -32.0f;

    protected float spriteXSize;
    protected float spriteYSize;
    protected int wallLayerMask;

    protected int health = 2;

    private bool jumping = false;

    private const float accel = 18.0f;
    private const float deccel = 24.0f;
    protected float kbVel = 12.0f;

    private int enemyType;
    public int deadBodyType;
    protected AudioManager am;

    protected int damageAmount = 1;
    protected float hitForce = 10.0f;

    private bool grounded = false;
    private bool stuckInGround = false;
    private const float floorStuckVel = 2.0f;

    protected float speedMultiplier = 1.0f;
    private Vector3 pos;
    private void Awake()
    {
        am = GameObject.Find("AudioManager").GetComponent<AudioManager>();             //Call sounds might slow things down if it's called a lot
        am.Play("enemySound");
        sr = GetComponent<SpriteRenderer>();
        ldm = GameObject.FindGameObjectWithTag("RoomManager").GetComponent<LootDropsManager>();
        sr.flipX = false;
        velocity = Vector2.zero;
        spriteXSize = sr.sprite.bounds.size.x / 2 * this.gameObject.transform.localScale.x;
        spriteYSize = (sr.sprite.bounds.size.y / 2) * this.gameObject.transform.localScale.y;
        wallLayerMask = LayerMask.GetMask("Wall");

        size = sr.sprite.bounds.size / 2.0f;
        size.x *= this.gameObject.transform.localScale.x;
        size.y *= this.gameObject.transform.localScale.y;
        halfSize = size / 2.0f;

        switch (this.gameObject.tag)
        {
            case "Enemy":
                kbVel = 12.0f;
                enemyType = 0;
                break;
            case "Boss":
                kbVel = 0.0f;
                enemyType = 1;
                break;
            default:
                enemyType = -1;
                break;
        }
    }
    protected virtual void Update()
    {
        Move();
    }
    private void Move()
    {        
        pos = transform.position;
        //bool grounded = Physics2D.Raycast(transform.position, Vector3.down, spriteYSize, wallLayerMask);
        velocity = new Vector2(velocity.x, velocity.y);
        SetGrounded();

            if (!grounded || jumping)
            {
                if (velocity.y <= maxYVel)
                    velocity.y += gravAcc * Time.deltaTime;
                if (velocity.y < 0 && grounded)
                {
                    jumping = false;
                    velocity.y = 0.0f;
                }
            }
            else
            {
                velocity.y = 0.0f;
            }

        if (SafeToMove())
        {
            Walk();
        }

        PreventWallCollide();

        if (enemyType == 0)
            pos.x += velocity.x * 2.0f * Time.deltaTime;
        else
            pos.x += velocity.x * speedMultiplier * 3.2f * Time.deltaTime;

        pos.y += velocity.y * speedMultiplier * Time.deltaTime;


        Debug.DrawRay(transform.position, Vector2.down, Color.red, size.y - 0.1f);
        //if (Physics2D.Raycast(transform.position, Vector2.down, size.y - 0.1f, wallLayerMask))
        //{
        //   pos.y += 1.0f;
        //}

        transform.position = pos;
    }

    protected void PreventWallCollide()
    {
        // TODO: Make smoother using collision and gradually being pushed out of walls

        Vector3 halfSizeX = new Vector3(halfSize.x - 0.2f, 0.0f, 0.0f);
        Vector3 sizeY = new Vector3(0.0f, size.y - 0.1f, 0.0f);

        // Hits left wall
        if ((Physics2D.Raycast(transform.position, Vector2.left, halfSize.x + 0.5f, wallLayerMask) ||
             Physics2D.Raycast(transform.position + sizeY, Vector2.left, halfSize.x + 0.5f, wallLayerMask) ||
             Physics2D.Raycast(transform.position - sizeY, Vector2.left, halfSize.x + 0.5f, wallLayerMask)) && 
             velocity.x < 0.0f && !stuckInGround)
        {
            velocity.x = 0.0f;
        }
        // Hits right wall
        else if ((Physics2D.Raycast(transform.position, Vector2.right, halfSize.x + 0.5f, wallLayerMask) ||
                  Physics2D.Raycast(transform.position + sizeY, Vector2.right, halfSize.x + 0.5f, wallLayerMask) ||
                  Physics2D.Raycast(transform.position - sizeY, Vector2.right, halfSize.x + 0.5f, wallLayerMask)) && 
                  velocity.x > 0.0f && !stuckInGround)
        {
            velocity.x = 0.0f;
        }

        // Hits ceiling
        if ((Physics2D.Raycast(transform.position, Vector2.up, size.y, wallLayerMask) ||
             Physics2D.Raycast(transform.position + halfSizeX, Vector2.up, size.y, wallLayerMask) ||
             Physics2D.Raycast(transform.position - halfSizeX, Vector2.up, size.y, wallLayerMask)) && velocity.y > 0.0f)
        {
            velocity.y = 0.0f;
        }
        // Hits floor
        else if (grounded && velocity.y <= 0)
        {
            Vector3 down = transform.TransformDirection(Vector3.down) * (size.y - 0.1f);
            Debug.DrawRay(transform.position, down, Color.cyan);
            if (Physics2D.Raycast(transform.position, Vector3.down, size.y - 0.1f, wallLayerMask))
            {
                stuckInGround = true;
            }
            else
            {
                velocity.y = 0.0f;
            }
        }

        if (stuckInGround)
        {
            if (!Physics2D.Raycast(transform.position, Vector3.down, size.y + 0.1f, wallLayerMask))
            {
                stuckInGround = false;
                velocity.y = 0.0f;
            }
            else
                velocity.y = floorStuckVel;
        }
    }

    protected void PreventWallCollideWithWep()
    {
        // TODO: Make smoother using collision and gradually being pushed out of walls

        Vector3 sizeY = new Vector3(0.0f, size.y - 0.1f, 0.0f);

        // Hits left wall
        if ((Physics2D.Raycast(transform.position, Vector2.left, halfSize.x + 0.5f, wallLayerMask) ||
             Physics2D.Raycast(transform.position + sizeY, Vector2.left, halfSize.x + 5.5f, wallLayerMask) ||
             Physics2D.Raycast(transform.position - sizeY, Vector2.left, halfSize.x + 5.5f, wallLayerMask)) && velocity.x < 0.0f)
        {
            velocity.x = 0.0f;
        }
        // Hits right wall
        else if ((Physics2D.Raycast(transform.position, Vector2.right, halfSize.x + 0.5f, wallLayerMask) ||
                  Physics2D.Raycast(transform.position + sizeY, Vector2.right, halfSize.x + 5.5f, wallLayerMask) ||
                  Physics2D.Raycast(transform.position - sizeY, Vector2.right, halfSize.x + 5.5f, wallLayerMask)) && velocity.x > 0.0f)
        {
            velocity.x = 0.0f;
        }
    }

    private void Walk()
    {
        if (moveDirLeft)
        {
            if (velocity.x < -maxXVel - 0.1f)
                velocity.x += deccel * Time.deltaTime;
            else if (velocity.x > -maxXVel + 0.1f)
                velocity.x -= accel * Time.deltaTime;
        }
        else
        {
            if (velocity.x > maxXVel + 0.1f)
                velocity.x -= deccel * Time.deltaTime;
            else if (velocity.x < maxXVel - 0.1f)
                velocity.x += accel * Time.deltaTime;
        }
    }

    private bool SafeToMove()
    {
        moveDirLeft = sr.flipX;

        if (moveDirLeft)
        {
            Debug.DrawRay(transform.position - new Vector3(-spriteXSize , 0, 0), Vector2.down, Color.red, spriteYSize + 0.1f);
            Debug.DrawRay(transform.position - new Vector3(0, spriteYSize - 0.1f, 0), Vector2.left, Color.red, spriteXSize);
            bool spaceLeft = Physics2D.Raycast(transform.position - new Vector3(spriteXSize + 0.5f,0,0), Vector2.down, spriteYSize, wallLayerMask); // check below a little bit ahead of current position
            bool obstacleLeft = Physics2D.Raycast(transform.position - new Vector3(0, spriteYSize - 0.1f,0), Vector2.left, spriteXSize + 0.5f, wallLayerMask); // check wall directly infront
            if (spaceLeft && !obstacleLeft)
            {
                return true;
            }
            else
            {
                velocity.x = 0.0f;
                moveDirLeft = false;
                sr.flipX = false;
                return false;
            }
        }
        else
        {
            Debug.DrawRay(transform.position + new Vector3(spriteXSize + 0.5f, 0, 0), Vector2.down, Color.red, spriteYSize + 0.1f);
            Debug.DrawRay(transform.position - new Vector3(0, spriteYSize - 0.1f, 0), Vector2.right, Color.red, spriteXSize + 0.5f);
            bool spaceRight = Physics2D.Raycast(transform.position + new Vector3(spriteXSize + 0.5f, 0, 0), Vector2.down, spriteYSize, wallLayerMask); // check below a little bit ahead of current position
            bool obstacleRight = Physics2D.Raycast(transform.position - new Vector3(0, spriteYSize - 0.1f, 0), Vector2.right, spriteXSize + 0.5f, wallLayerMask); // check wall directly infront
            if (spaceRight && !obstacleRight)
            {
                return true;
            }
            else
            {
                velocity.x = 0.0f;
                moveDirLeft = true;
                sr.flipX = true;
                return false;
            }
        }
    }
    public virtual void Hit(int damage, int attackDir)
    {
        health -= damage;


        if (health <= 0)
        {
            am.Play("enemyDied");
            ldm.DropDeadBody(transform.position, deadBodyType);
            ldm.DropLoot(transform.position, 1);
            Destroy(this.gameObject);
        }
        else
        {
            am.Play("enemyHurt");
            Knockback(attackDir);
            StartCoroutine(FlashRed(attackDir));
        }
    }

    protected virtual IEnumerator FlashRed(int attackDir)
    {
        sr.color = new Color(0.5f,0,0);
        yield return new WaitForSeconds(0.1f);
        sr.color = Color.white;
    }

    protected virtual void Knockback(int dir)
    {
        switch (dir)
        {
            case 1:
                velocity.x += kbVel;
                break;
            case 3:
                velocity.x -= kbVel;
                break;
            default:
                break;
        }
    }

    public float GetYVel()
    {
        return velocity.y;
    }

    public void SetYVel(float yVel)
    {
        velocity.y = yVel;
        if (yVel > 0)
            jumping = true;
    }

    public bool GetGrounded()
    {
        return Physics2D.Raycast(transform.position, Vector3.down, spriteYSize, wallLayerMask);
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.tag == "Spikes")
        {
            Hit(2, 5);
        }
    }
    private void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            collider.gameObject.GetComponent<Player>().Hit(transform.position, hitForce, damageAmount);
        }
    }

    protected void SetGrounded()
    {
        grounded = Physics2D.Raycast(transform.position, transform.TransformDirection(Vector3.down), spriteYSize, wallLayerMask);
    }

    protected void ResetVelocity()
    {
        velocity = new Vector2(0.0f, 0.0f);
    }

    protected bool GetStuckInGround()
    {
        return stuckInGround;
    }
    
    protected bool CheckForStuckInGround()
    {
        stuckInGround = Physics2D.Raycast(transform.position, Vector3.down, halfSize.y - 0.1f, wallLayerMask);

        return stuckInGround;
    }
}
