using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item
{
    string ID;
    string description;
    int quantity;    
    public Item(string _name, string _description, int _quantity)
    {
        ID = _name;
        description = _description;
        quantity = _quantity;        
    }
    public Item()
    {

    }

    public void SetQuantity(int _quantity)
    {
        quantity = _quantity;
    }
    public void IncreaseQuantity()
    {
        quantity++;
    }

    public string GetStrName()
    {
        return ID;
    }    

    public void SetStrName(string _name)
    {
        ID = _name;
    }
    public string GetDescription()
    {
        return description;
    }
    public void SetDescription(string _description)
    {
        description = _description;
    }
    public int GetQuantity()
    {
        return quantity;
    }

}
