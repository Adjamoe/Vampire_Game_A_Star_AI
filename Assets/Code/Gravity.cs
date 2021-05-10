using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour
{
    Vector2 velocity;
    float gravAcc = -9.81f;
    protected float spriteXSize;
    protected float spriteYSize;
    protected SpriteRenderer sr;
    protected int wallLayerMask;
    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        wallLayerMask = LayerMask.GetMask("Wall");
        spriteXSize = sr.sprite.bounds.size.x / 2 * transform.localScale.x;
        spriteYSize = (sr.sprite.bounds.size.y / 2) * transform.localScale.y;
    }
    private void Update()
    {
        Move();
    }
    private void Move()
    {
        Vector3 pos = transform.position;
        velocity = new Vector2(0.0f, velocity.y);
        Debug.DrawRay(transform.position, Vector3.down, Color.red, spriteYSize);
        bool grounded = Physics2D.Raycast(transform.position, Vector3.down, spriteYSize, wallLayerMask);

        if (!grounded)
        {
            velocity.y += gravAcc * Time.deltaTime;
        }
        else
        {
            velocity.y = 0.0f;
        }

        pos.y += velocity.y * Time.deltaTime;
        transform.position = pos;
    }
}
