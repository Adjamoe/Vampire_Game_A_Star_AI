using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawRotating : MonoBehaviour
{
    float speed = 80;
    void Update()
    {
        Vector3 rot = transform.eulerAngles;
        rot.z += Time.deltaTime * speed;
        transform.eulerAngles = rot;       
    }
}
