using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    public string requiredItemName;
    public GameObject gateObj, lockPlayerInGate;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            if (collision.gameObject.GetComponent<Player>().HasItemInInventory(requiredItemName))
            {
                UnlockGate();
            }
        }
    }
    public void UnlockGate()
    {
        gateObj.SetActive(false);
        UnlockPlayerLockInGate();
    }
    public void UnlockPlayerLockInGate()
    {
        if(lockPlayerInGate != null)
            lockPlayerInGate.SetActive(false);
    }
    public void LockPlayerLockInGate()
    {
        gateObj.SetActive(true);
        if(lockPlayerInGate != null)
            lockPlayerInGate.SetActive(true);
    }
}
