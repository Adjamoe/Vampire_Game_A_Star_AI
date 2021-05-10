using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateTwoSprites : MonoBehaviour
{
    public Sprite mouthOpen, mouthClosed;
    protected SpriteRenderer sr;
    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        StartCoroutine(Animate());
        sr.sprite = mouthOpen;
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
}
