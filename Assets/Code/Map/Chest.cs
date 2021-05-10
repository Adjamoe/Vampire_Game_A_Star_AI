using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public string uniqueItemName, uniqueItemDescription;
    public int uniqueItemQuanity;
    public GameObject interactText;
    private LootDropsManager ldm;
    private bool opened = false;
    public Sprite openChest;
    SpriteRenderer sr;
    AudioManager am;
    private void Start()
    {
        am = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        if (Random.Range(0, 3) == 1 && uniqueItemName == "")
        {
            this.gameObject.SetActive(false);
        }
        sr = GetComponent<SpriteRenderer>();
        ldm = GameObject.FindGameObjectWithTag("RoomManager").GetComponent<LootDropsManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            interactText.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            interactText.SetActive(false);
        }
    }
    public void Open()
    {
        if (!opened)
        {
            am.Play("unlockChest");
            sr.sprite = openChest;
            opened = true;
            if (uniqueItemName != "")
            {
                    ldm.DropLoot(transform.position, uniqueItemName, uniqueItemDescription, uniqueItemQuanity);
            }
            else
            {
                ldm.DropLoot(transform.position, Random.Range(5, 15));
            }
        }
    }
}
