using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    public List<GameObject> npcObjs;
    List<bool> npcUnlockedBools;
    private RoomManager rm;
    private void Start()
    {
        rm = GameObject.FindGameObjectWithTag("RoomManager").GetComponent<RoomManager>();
        //npcObjs = new List<GameObject>();
        npcUnlockedBools = rm.GetUnlockedNPCs();
    }
    public void UpdateNPCs()
    {
        npcUnlockedBools = rm.GetUnlockedNPCs();
        int i = 0;
        foreach(GameObject npc in npcObjs)
        {
            if(npcUnlockedBools[i])
            {
                npc.SetActive(true);
            }
            else
            {
                npc.SetActive(false);
            }
            i++;
        }
    }
}
