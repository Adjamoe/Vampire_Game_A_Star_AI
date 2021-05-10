using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Slug : BasicMovement 
{
    // Game Object needs a RigidBody2D to move.  

    // Private attributes
    public Sprite mouthOpen, mouthClosed;
    private void Start()
    {
        StartCoroutine(Animate());
    }
    private IEnumerator Animate()
    {
        if(sr.sprite == mouthOpen)
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
}
