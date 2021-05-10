using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Loot : MonoBehaviour
{
    LootDropsManager ldm;
    public string description, id;        
    Item item;
    // Start is called before the first frame update
    void Start()
    {
        ldm = GameObject.FindGameObjectWithTag("RoomManager").GetComponent<LootDropsManager>();
        if(item == null)
            item = new Item();
        if (description != "")
        {
            SetItemInfo(id, description, 1);
            ldm.AddLootToRoom(this.gameObject);
        }
    }
    public Item GetItem()
    {
        return item;
    }
    public void SetItemInfo(string _name, string _desc, int _quantity)
    {
        if(GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().HasNoteInInventory(_name))
        {
            Destroy(this.gameObject);
            return;
        }
        if (item == null)
            item = new Item();
        item.SetStrName(_name);
        item.SetDescription(_desc);
        item.SetQuantity(_quantity);
    }

}
