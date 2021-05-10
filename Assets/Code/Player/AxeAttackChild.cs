using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeAttackChild : MonoBehaviour
{
    AxeAttack axeScript;

    // Start is called before the first frame update
    void Start()
    {
        axeScript = this.gameObject.GetComponentInParent<AxeAttack>();
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        axeScript.CollideWithObject(other);
    }
}
