using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicFly : BasicMovement
{
    protected GameObject player;
    protected float flySpeed = 5.0f;
    private float sightRange = 10.0f;
    private float hover = 0.0f;
    protected int wallMask;

    private Vector2 knock;

    protected virtual void Start()
    {
        wallMask = LayerMask.GetMask("Wall");
        player = GameObject.FindGameObjectWithTag("Player");
    }
    protected override void Update()
    {
        if(PlayerClose())
        {
            AttackPlayer();
        }
        else
        {
            Chill();
        }
    }
    protected bool PlayerClose()
    {
        float dist = Mathf.Sqrt(((player.transform.position.y - transform.position.y) * (player.transform.position.y - transform.position.y)) + (player.transform.position.x - transform.position.x) * (player.transform.position.x - transform.position.x));
        if(dist < sightRange && !Physics2D.Linecast(player.transform.position, transform.position, wallMask))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    protected void AttackPlayer()
    {
        Vector3 pos = transform.position;
        velocity = new Vector2(velocity.x, velocity.y);

        float angle;
        Vector3 playerPos = player.transform.position;
        Vector3 enemyPos = this.transform.position;
        Vector2 vecEnemyToPlayer = new Vector2(playerPos.x - enemyPos.x, playerPos.y - enemyPos.y);
        float modVec = Mathf.Sqrt(Mathf.Pow(vecEnemyToPlayer.x, 2.0f) + Mathf.Pow(vecEnemyToPlayer.y, 2.0f));
        float dotProduct = vecEnemyToPlayer.x / modVec;
        if (playerPos.y < enemyPos.y)
            angle =  360.0f - (Mathf.Acos(dotProduct) * Mathf.Rad2Deg);
        else
            angle =  Mathf.Acos(dotProduct) * Mathf.Rad2Deg;

        velocity.x = flySpeed * Mathf.Cos(angle * Mathf.Deg2Rad);
        velocity.y = flySpeed * Mathf.Sin(angle * Mathf.Deg2Rad);
        
        if (player.transform.position.x >= transform.position.x) // player on right
        {
            if(!(player.transform.position.x < transform.position.x + 0.1f)) // not a tiny difference to stop flickering 
                sr.flipX = true;
            Debug.DrawRay(transform.position, Vector2.right, Color.red, spriteXSize + 0.1f);
            if (Physics2D.Raycast(transform.position, Vector2.right, (spriteXSize + 0.1f), wallLayerMask)) // not going to collide with a wall
                velocity.x = 0; // don't go, there is an obstaclae
        }
        else // player on left
        {
            if (!(player.transform.position.x > transform.position.x - 0.1f)) // not a tiny difference to stop flickering 
                sr.flipX = false;
            Debug.DrawRay(transform.position, Vector2.left, Color.red, -spriteXSize - 0.1f);
            if (Physics2D.Raycast(transform.position, Vector2.left, (spriteXSize + 0.1f), wallLayerMask))
                velocity.x = 0;
        }
        if(player.transform.position.y >= transform.position.y) // player above
        {
            if (!(player.transform.position.y < transform.position.y + 0.1f)) // not a tiny difference to stop flickering 
                sr.flipY = false;
            Debug.DrawRay(transform.position, Vector2.up, Color.red, spriteYSize + 0.1f);
            if (Physics2D.Raycast(transform.position, Vector2.up, (spriteYSize + 0.1f), wallLayerMask))
                velocity.y = 0;
        }
        else // player below
        {
            if (!(player.transform.position.y > transform.position.y - 0.1f)) // not a tiny difference to stop flickering 
                sr.flipY = true;
            Debug.DrawRay(transform.position, Vector2.down, Color.red, -spriteYSize - 0.1f);
            if (Physics2D.Raycast(transform.position, Vector2.down, (spriteYSize + 0.1f), wallLayerMask))
                velocity.y = 0;
        }



        velocity += knock;
        if(knock.x > 0)
        {
            knock.x -= Time.deltaTime * 24.0f;
        }
        else if (knock.x < 0)
        {
            knock.x += Time.deltaTime * 24.0f;
        }
        if (knock.y > 0)
        {
            knock.y -= Time.deltaTime * 24.0f;
        }
        else if (knock.y < 0)
        {
            knock.y += Time.deltaTime * 24.0f;
        }


        pos.x += velocity.x * Time.deltaTime;
        pos.y += velocity.y * Time.deltaTime;
        transform.position = pos;

    }
    private void Chill()
    {
        velocity = Vector3.zero;
        Vector3 pos = transform.position;
        hover += Time.deltaTime * flySpeed;
        velocity.x += Mathf.Sin(hover);
        velocity.y += Mathf.Cos(-hover);
        pos.x += velocity.x * Time.deltaTime;
        pos.y += velocity.y * Time.deltaTime;
        transform.position = pos;
        if (hover > 360)
        {
            hover = 4;
            //sr.flipX = !sr.flipX;
            //sr.flipY = !sr.flipY;
        }
    }
    public override void Hit(int damage, int attackDir)
    {
        if(health > 0)
        {
            Knockback(attackDir);
        }
        base.Hit(damage, attackDir);
    }
    protected override void Knockback(int dir)
    {        
        // 0 = Up, 1 = Right, 2 = Down, 3 = Left
        knock = Vector2.zero;
        switch (dir)
        {
            case 0:
                knock.y += kbVel;
                break;
            case 1:
                knock.x += kbVel;
                break;
            case 2:
                knock.y -= kbVel;
                break;
            case 3:
                knock.x -= kbVel;
                break;
            default:
                break;
        }
    }
}
