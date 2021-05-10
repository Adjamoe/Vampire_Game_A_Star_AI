using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawBlade : MonoBehaviour
{
    public GameObject firstEndPos, secondEndPos;
    private Vector3 targetPos;
    Vector2 velocity;
    public float moveSpeed = 1.0f;
    public float pauseTime = 0.5f;
    private bool pausing = true;

    private void Start()
    {
        transform.position = firstEndPos.transform.position;
        targetPos = firstEndPos.transform.position;
        UpdateTarget();
    }
    private void Update()
    {
        if (transform.position.x < targetPos.x + 0.1f && transform.position.x > targetPos.x - 0.1f && transform.position.y < targetPos.y + 0.1f && transform.position.y > targetPos.y - 0.1f)
        {
            UpdateTarget();
        }
        if (!pausing)
        {
            Vector3 pos = transform.position;
            velocity = Vector2.zero;

            float angle;
            Vector3 sawPos = this.transform.position;
            Vector2 vecSawToTarget = new Vector2(targetPos.x - sawPos.x, targetPos.y - sawPos.y);
            float modVec = Mathf.Sqrt(Mathf.Pow(vecSawToTarget.x, 2.0f) + Mathf.Pow(vecSawToTarget.y, 2.0f));
            float dotProduct = vecSawToTarget.x / modVec;
            if (targetPos.y < sawPos.y)
                angle = 360.0f - (Mathf.Acos(dotProduct) * Mathf.Rad2Deg);
            else
                angle = Mathf.Acos(dotProduct) * Mathf.Rad2Deg;

            velocity.x = moveSpeed * Mathf.Cos(angle * Mathf.Deg2Rad);
            velocity.y = moveSpeed * Mathf.Sin(angle * Mathf.Deg2Rad);

            pos.x += velocity.x * Time.deltaTime;
            pos.y += velocity.y * Time.deltaTime;
            transform.position = pos;
            Debug.DrawLine(firstEndPos.transform.position, secondEndPos.transform.position);
        }
    }
    private void UpdateTarget()
    {
        pausing = true;
        StartCoroutine(Pause());
        if(targetPos == firstEndPos.transform.position)
        {
            targetPos = secondEndPos.transform.position;
        }
        else
        {
            targetPos = firstEndPos.transform.position;
        }
    }
    private IEnumerator Pause()
    {
        yield return new WaitForSeconds(pauseTime);
        pausing = false;
    }
}
