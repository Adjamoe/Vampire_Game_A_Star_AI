using System.Collections;
using UnityEngine;

public class Enemy_Charge : BasicMovement
{
    private bool canSeePlayer = false;
    private bool canCharge = true;
    private float fieldOfView = 10.0f;
    // Game Object needs a RigidBody2D to move.
    // Private attributes
    //private int damageAmount = 1;
    // private functions
    public Sprite mouthOpen, mouthClosed;
    private void Start()
    {
        StartCoroutine(Animate());
    }
    private IEnumerator Animate()
    {
        if (sr.sprite == mouthOpen)
        {
            sr.sprite = mouthClosed;
        }
        else
        {
            sr.sprite = mouthOpen;
        }
        yield return new WaitForSeconds(Random.Range(0.2f, 0.8f));
        StartCoroutine(Animate());

    }
    protected override void Update() {
        
        if (canCharge)
        {
            if (RaycastHitPlayer(fieldOfView))
            {
                maxXVel = 6.0f;
                StartCoroutine(StaminaCoroutine());
            }
        }
        
        base.Update();
        
    } 
    

    bool RaycastHitPlayer(float fieldOfView)
    {
        RaycastHit2D hit;
        //float castDist = distance;

        //Vector2 endPos = castPoint.position + Vector3.right * fieldOfView;
        
        if (moveDirLeft)
        {
            Debug.DrawRay(transform.position + Vector3.left, Vector2.left, Color.red, fieldOfView);
            hit = Physics2D.Raycast(transform.position + Vector3.left, Vector2.left, fieldOfView);
        }
        else
        {
            Debug.DrawRay(transform.position + Vector3.right, Vector2.right, Color.red, fieldOfView);
            hit = Physics2D.Raycast(transform.position + Vector3.right, Vector2.right, fieldOfView);
        }

        canSeePlayer = false;
        if (hit.collider != null)
        {            
            if (hit.collider.gameObject.tag == "Player")
            {
                //Agro enemy to charge at the player
                canSeePlayer = true;
                
            }
        }
        return canSeePlayer;
    }

    IEnumerator StaminaCoroutine()
    {
        canCharge = false;
        canSeePlayer = false;
        am.Play("chargerCharging");
        yield return new WaitForSeconds(3);
        am.Play("chargerRecharging");
        maxXVel = 0.0f;
        StartCoroutine(RecoverStamina());
    }
    IEnumerator RecoverStamina()
    {
        yield return new WaitForSeconds(2);
        maxXVel = 3.0f;
        StartCoroutine(ChargeCooldown());
    }
    IEnumerator ChargeCooldown()
    {
        yield return new WaitForSeconds(2);
        canCharge = true;
    }

}
