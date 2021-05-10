using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StoreItem
{
    public int cost;
    public string name;
    public string description;
    public bool sold;
    public bool createdUI;
    public GameObject button;
    public int maxQuantity;
    public void SetButton(GameObject btn)
    {
        button = btn;
    }
}
