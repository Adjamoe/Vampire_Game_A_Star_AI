using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootDropsManager : MonoBehaviour
{
    public GameObject gold, blood, note, item; // gold = type 1, blood = type 2, note = type 3
    public List<GameObject> lootInRoom = new List<GameObject>();
    private UIManager ui;
    public GameObject greenDeadBody, purpleDeadBody, bossDeadBody, purpleFlyingDeadBody;
    AudioManager am;
    private void Start()
    {
        am = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        ui = GetComponent<UIManager>();
    }
    public void DropDeadBody(Vector3 position, int type)
    {
        switch (type)
        {
            case 0:
                lootInRoom.Add(Instantiate(greenDeadBody, position, Quaternion.identity));
                break;
            case 1:
                lootInRoom.Add(Instantiate(purpleDeadBody, position, Quaternion.identity));
                break;
            case 2:
                lootInRoom.Add(Instantiate(bossDeadBody, position, Quaternion.identity));
                break;            
            case 3:
                lootInRoom.Add(Instantiate(purpleFlyingDeadBody, position, Quaternion.identity));
                break;
        }
    }
    private Vector3 RandomX()
    {
        return new Vector3(Random.Range(-1f, 1f), 0, 0);
    }
    public void DropLoot(Vector3 position, int amount) // normal enemies drop money and maybe blood
    {
        for (int i = 0; i < amount; ++i)
        {
            GameObject tempGold = Instantiate(gold, position + RandomX(), Quaternion.identity);
            tempGold.GetComponent<Loot>().SetItemInfo("Gold", "", Random.Range(1, 10));
            lootInRoom.Add(tempGold);
        }
        if(Random.Range(0,5) == 3)
        {
            GameObject tempBlood = Instantiate(blood, position + RandomX(), Quaternion.identity);
            tempBlood.GetComponent<Loot>().SetItemInfo("Blood", "", 1);
            lootInRoom.Add(tempBlood);
        }
    }
    public void AddLootToRoom(GameObject item)
    {
        lootInRoom.Add(item);
    }
    public void DropLoot(Vector3 _position, string _name, string _description, int _quantity) // drops specific loot like keys and notes
    {
        GameObject tempItem = Instantiate(item, _position + RandomX(), Quaternion.identity);
        tempItem.GetComponent<Loot>().SetItemInfo(_name, _description, _quantity);
        lootInRoom.Add(tempItem);
    }
    public void PickUpLoot(GameObject item)
    {
        for(int i = 0; i < lootInRoom.Count; ++i)
        {
            if(lootInRoom[i] == item)
            {
                if(item.tag != "Enemy")
                    am.Play("pickedUpItem");
                Destroy(lootInRoom[i].gameObject);
            }
        }
    }
    public void ClearRoomOfLoot()
    {
        for (int i = 0; i < lootInRoom.Count; ++i)
        {
            if(lootInRoom[i] != null)
                Destroy(lootInRoom[i].gameObject);
        }
        lootInRoom.Clear();
    }
}
