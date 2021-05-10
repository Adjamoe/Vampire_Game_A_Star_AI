using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextPopUp : MonoBehaviour
{
    public GameObject parentPanel;
    public GameObject startSpawnPos;
    public GameObject shopItemButton;
    public Text textBox;
    public GameObject hintTextBox;
    private Player player;

    public List<string> story = new List<string>();
    private int storyNum = 0;
    private bool noStoryLeft = false;
    private AudioManager am;

    public List<StoreItem> shop = new List<StoreItem>();
    private void Start()
    {
        am = FindObjectOfType<AudioManager>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }
    public void DisplayStoryText()
    {
        parentPanel.SetActive(true);
        hintTextBox.SetActive(false);
        if (!noStoryLeft) // say text, then open shop if shop. If not shop then recycle text
        {
            if(storyNum == 0)
            {
                am.Play("npcGreeting");
            }
            parentPanel.SetActive(true);
            textBox.text = story[storyNum];
            storyNum++;
            if (storyNum > story.Count - 1)
            {
                noStoryLeft = true;
            }
        }
        else
        {
            if(shop.Count == 0) // if not a shop, display story
            {
                HideStoryText();
                hintTextBox.SetActive(true);
                storyNum = 0;
                noStoryLeft = false;
            }
            else // is a shop so display shop
            {
                textBox.text = "";
                int i = 0;
                foreach (StoreItem si in shop)
                {
                    GameObject temp = Instantiate(shopItemButton, startSpawnPos.transform) as GameObject;
                    si.button = temp;
                    si.button.transform.localPosition = new Vector3(0, i * - 3f, 0);
                    i++;
                    si.button.GetComponent<Button>().onClick.AddListener(()=>ClickedBuy(si)); // button on click
                    string output = si.name + " costs " + si.cost + " Gold\nDescription: " + si.description;
                    Text txt = si.button.GetComponentInChildren<Text>();
                    txt.text = output;
                }
                UpdateButtons();
            }
        }

    }
    public void ClickedBuy(StoreItem item)
    {
        if (player.GetGold() >= item.cost && player.HasMaxOfItem(item.name, item.maxQuantity) == false)
        {
            player.ReduceGold(item.cost);
            am.Play("purchasedItem");
            player.AddToInventory(item);
            UpdateButtons();
        }
        else
        {
            am.Play("buttonDenied");
        }
    }
    private void UpdateButtons()
    {
        foreach (StoreItem si in shop)
        {
            if(player.HasMaxOfItem(si.name, si.maxQuantity))
            {
                si.button.GetComponent<Button>().interactable = false;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            hintTextBox.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            hintTextBox.SetActive(false);
            storyNum = 0;
            parentPanel.SetActive(false);
        }
    }
    public void HideStoryText()
    {
        parentPanel.SetActive(false);
    }
}
