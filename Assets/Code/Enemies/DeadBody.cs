using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadBody : MonoBehaviour
{

    LootDropsManager ldm;
    private void Start()
    {
        ldm = GameObject.FindGameObjectWithTag("RoomManager").GetComponent<LootDropsManager>();
        StartCoroutine(Decay());
    }

    private IEnumerator Decay()
    {
        yield return new WaitForSeconds(5f);
        if(this.gameObject != null)
        {
            ldm.PickUpLoot(this.gameObject);
            Destroy(this.gameObject);
        }
    }
}
