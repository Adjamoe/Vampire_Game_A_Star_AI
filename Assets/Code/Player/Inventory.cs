using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    int gold;
    int blood;

    List<Item> inventory = new List<Item>();
    List<Item> journal = new List<Item>();
    UIManager ui;

    public Inventory(UIManager _ui) // use for new game
    {
        gold = 0;
        blood = 0;
        inventory.Add(new Item("Axe", "Used to deal damage to others", 1));
        ui = _ui;
    }
    public Inventory(int _gold, int _blood, List<Item> _inventory) // use for loading previous save
    {
        gold = _gold;
        blood = _blood;
        inventory = _inventory;
    }
    public void AddToInventory(StoreItem item)
    {
        if (!HasItem(item.name))
        {
            inventory.Add(new Item(item.name, item.description, 1));
        }
        else
        {
            InceaseQuantityOfItem(item.name);
        }
    }
    public void AddToInventory(Item item)
    {
        if (item.GetStrName() == "Gold")
        {
            gold += item.GetQuantity();
            ui.DisplayAddedToText(3, 1);
        }
        else if (item.GetStrName() == "Blood")
        {
            ++blood;
            ui.DisplayAddedToText(4, 1);
        }
        else
        {
            if (!HasItem(item.GetStrName()))
            {
                ui.DisplayAddedToText(0, 1);
                inventory.Add(new Item(item.GetStrName(), item.GetDescription(), item.GetQuantity()));
            }
            else
            {
                ui.DisplayAddedToText(2, 1);
                InceaseQuantityOfItem(item.GetStrName());
            }
        }

    }
    public void AddToJournal(Item item)
    {
        if(!HasNote(item.GetStrName()))
        {
            ui.DisplayAddedToText(1, 1);
            journal.Add(item);
        }
    }
    public void IncreaseGold(int amount)
    {
        gold += amount;
    }
    public void IncreaseBlood(int amount)
    {
        blood += amount;
    }

    public List<Item> GetInventory()
    {
        return inventory;
    }    
    public List<Item> GetJournal()
    {
        return journal;
    }
    public int GetGold()
    {
        return gold;
    }
    public int GetBlood()
    {
        return blood;
    }
    public void LoseMoneyFromDeath(int amountLost)
    {
        if(gold > 1)
            gold -= amountLost;
    }
    public bool HasItem(string itemName)
    {
        foreach(Item i in inventory)
        {
            if(i.GetStrName() == itemName)
            {
                return true;
            }
        }
        return false;
    }
    public bool HasNote(string noteName)
    {
        foreach (Item i in journal)
        {
            if (i.GetStrName() == noteName)
            {
                return true;
            }
        }
        return false;
    }
    public void InceaseQuantityOfItem(string itemName)
    {
        foreach (Item i in inventory)
        {
            if (i.GetStrName() == itemName)
            {
                i.IncreaseQuantity();
            }
        }
    }
    public bool maxedItem(string itemName, int maxQuantity)
    {
        foreach (Item i in inventory)
        {
            if (i.GetStrName() == itemName)
            {
                if(i.GetQuantity() >= maxQuantity)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        return false;
    }
}
