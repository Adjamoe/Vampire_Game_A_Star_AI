using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAxeAttackChild : MonoBehaviour
{
    BossAxeAttack bossAxeScript;

    // Start is called before the first frame update
    void Start()
    {
        bossAxeScript = this.gameObject.GetComponentInParent<BossAxeAttack>();
    }

    public void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
            bossAxeScript.CollideWithPlayer(other.gameObject.GetComponent<Player>());
    }
}
