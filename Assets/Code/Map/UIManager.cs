using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private RoomManager rm;
    public int noteCounter = 0;
    public GameObject journalEntry1, journalEntry2, journalEntry3;
    public Sprite journalEntry1Sprite, journalEntry2Sprite, journalEntry3Sprite;
    public Text journalEntry1Description, journalEntry2Description, journalEntry3Description;
    public GameObject journalMenu, mapMenu, skillsMenu, inventoryMenu, optionsMenu, startMenu, mapSlot, playerDiedMenu, creditsMenu;
    public Text goldText, bloodText;
    public Text inventoryItemListTxt;
    public GameObject lifeBar;
    public Slider healthSlider, magicSlider;
    public Text playerLostMoneyText, playerStatsText, creditsText, creditsButtonText;
    public GameObject AddedToText;
    private Player player;
    private int currentMenu;
    private int maxMenus = 3;
    private int minMenus = 0;
    private bool menusDisplayed = false;
    private bool gamePaused = true;
    private bool creditsGone = false;
    private int soundLevel;
    public Slider slider;
    public Text SliderText;
    private AudioManager am;
    private bool gameStarted = false;
    private List<string> lore = new List<string>();
    private int currentLoreNum;
    public Text loreText;
    private int textDispalyCalls = 0;
    private void Start()
    {
        if (!Screen.fullScreen)
            Screen.fullScreen = !Screen.fullScreen;

        am = GameObject.Find("AudioManager").GetComponent<AudioManager>();

        //am.Play("song");

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        rm = GetComponent<RoomManager>();
        FillLore();
        SetAllUIInactive();
        DisplayStartMenu();

        soundLevel = am.GetVolume();
        slider.maxValue = 10;
        slider.minValue = 0;
        slider.value = soundLevel;

        healthSlider.maxValue = player.GetMaxHealth();
        healthSlider.minValue = 0;
        magicSlider.maxValue = player.GetMaxMagic();
        magicSlider.minValue = 0;
        //player.ChangeLayerOrder(0);
    }
    private void Update()
    {
        if (optionsMenu.activeSelf)
        {
            if ((int)slider.value != soundLevel)
            {
                soundLevel = (int)slider.value;
                SliderText.text = "" + soundLevel + "/10";
                am.SetVolume(soundLevel);
            }
        }
    }
    public void StartGame()
    {
        rm.start();
        am.Play("buttonPress");
        startMenu.SetActive(false);
        lifeBar.SetActive(true);
        Time.timeScale = 1f;
        gameStarted = true;
        gamePaused = false;
        player.SetCanMove(true);
        //player.ChangeLayerOrder(1);

    }
    public bool GetGameStarted()
    {
        return gameStarted;
    }
    private void SetAllUIInactive()
    {
        journalMenu.SetActive(false);
        mapMenu.SetActive(false); // set to false in roommanager so it gets a copy of it before turning it hidden
        skillsMenu.SetActive(false);
        inventoryMenu.SetActive(false);
        playerDiedMenu.SetActive(false);
        lifeBar.SetActive(false);
        AddedToText.SetActive(false);
    }
    public void DisplayMenu()
    {
        am.Play("buttonPress");
        if(!menusDisplayed)
        {
            player.SetCanMove(false);
            //player.ChangeLayerOrder(0);
            currentMenu = 1;
            DisplayMapMenu();
            menusDisplayed = true;
            lifeBar.SetActive(false);
        }
        else
        {
            player.SetCanMove(true);
            //player.ChangeLayerOrder(1);
            SetAllUIInactive();
            menusDisplayed = false;
            lifeBar.SetActive(true);
        }
    }
    public bool DisplayingMenus()
    {
        return menusDisplayed;
    }
    public void CycleNextMenu()
    {
        am.Play("buttonPress");
        HidePreviousMenu(currentMenu);
        if(currentMenu != maxMenus)
        {
            ++currentMenu;
        }
        else
        {
            currentMenu = minMenus;
        }
        DisplayMenu(currentMenu);
    }
    public void CyclePreviousMenu()
    {
        am.Play("buttonPress");
        HidePreviousMenu(currentMenu);
        if (currentMenu != minMenus)
        {
            --currentMenu;
        }
        else
        {
            currentMenu = maxMenus;
        }
        DisplayMenu(currentMenu);
    }
    private void DisplayMenu(int menu)
    {
        // journal      0 
        // map          1
        // skills       2
        // inventory    3
        switch(menu)
        {
            case 0:
                DisplayJournalMenu();
                break;
            case 1:
                DisplayMapMenu();
                break;
            case 2:
                UpdateSkillsMenu();
                DisplaySkillsMenu();
                break;
            case 3:
                DisplayInventoryMenu();
                break;
        }
    }
    private void HidePreviousMenu(int menu)
    {
        // journal      0 
        // map          1
        // skills       2
        // inventory    3
        switch (menu)
        {
            case 0:
                journalMenu.SetActive(false);
                break;
            case 1:
                mapMenu.SetActive(false);
                break;
            case 2:
                skillsMenu.SetActive(false);
                break;
            case 3:
                inventoryMenu.SetActive(false);
                break;
        }
    }
    private void DisplayJournalMenu()
    {
        UpdateJournal();
        journalMenu.SetActive(true);
    }
    private void DisplayMapMenu()
    {
        rm.RefreshMap();
        mapMenu.SetActive(true);
    }
    private void DisplaySkillsMenu()
    {
        skillsMenu.SetActive(true);
    }
    private void DisplayInventoryMenu()
    {
        UpdateInventory();
        inventoryMenu.SetActive(true);
    }
    private void DisplayOptionsMenu()
    {
        optionsMenu.SetActive(true);
    }
    private void DisplayStartMenu()
    {
        startMenu.SetActive(true);
        DisplayLore();
    }
    public GameObject GetMap()
    {
        return mapSlot;
    }
    public void UpdateJournal()
    {
        List<Item> journal = player.GetJournal();
        foreach(Item i in journal)
        {
            if(i.GetStrName() == "note1")
            {
                journalEntry1Description.text = i.GetDescription(); // "The Dungeons are far from a natural creation. They were created by human slaves whilst the vampires ruled the world, the slaves worked diligently for their freedom but none ever saw the light of day again"
                journalEntry1.GetComponent <SpriteRenderer>().sprite = journalEntry1Sprite;
            }
            if (i.GetStrName() == "note2")
            {
                journalEntry2Description.text = i.GetDescription(); //The day humans discovered the vampires weakness to human blood was the day the uprising stood a chance. Since this day humans have never stopped weaponising human blood, there are rumours of a human testing facility in which they test these weapons on captured vampires
                journalEntry2.GetComponent<SpriteRenderer>().sprite = journalEntry2Sprite;
            }
            if (i.GetStrName() == "note3")
            {
                journalEntry3Description.text = i.GetDescription(); //The bounty hunters that the council employ to remove the undesirable vampires grow in power. Strangely when they begin to get strong they often seem to disappear
                journalEntry3.GetComponent<SpriteRenderer>().sprite = journalEntry3Sprite;
            }
        }        
    }
    public void UpdateInventory()
    {
        List<Item> inventory = player.GetInventory();
        string inventoryOutput = "";
        goldText.text = "" + player.GetGold();
        bloodText.text = "" + player.GetBlood();
        for (int i = 0; i < inventory.Count; ++i)
        {
            if(inventory[i].GetStrName() != "")
            {
                inventoryOutput = inventoryOutput + "\n\n" + (i+1) + ":  " + inventory[i].GetStrName() + "\n" + inventory[i].GetDescription(); 
            }
        }
        if(inventoryOutput != "")
        {
            inventoryItemListTxt.text = inventoryOutput;
        }
        else
        {
            inventoryItemListTxt.text = "You have nothing in your inventory...";
        }
    }
    public void UpdateSkillsMenu()
    {
        playerStatsText.text = "Health: " + player.GetHealth() + " / " + player.GetMaxHealth() + "\n" +
                               "Magic: " + player.GetMagic() + " / " + player.GetMaxMagic() + "\n" +
                               "Damage: " + player.GetDamage();
    }
    public void Pause()
    {
        if(DisplayingMenus())
        {
            DisplayMenu();
        }
        am.Play("buttonPress");
        if (!gamePaused)
        {
            player.SetCanMove(false);
            lifeBar.SetActive(false);
            //player.ChangeLayerOrder(0);
            gamePaused = true;
            Time.timeScale = 0f;
            optionsMenu.SetActive(true);
        }
        else
        {
            player.SetCanMove(true);
            lifeBar.SetActive(true);
            //player.ChangeLayerOrder(1);
            gamePaused = false;
            Time.timeScale = 1f;
            optionsMenu.SetActive(false);
        }
    }
    public void UpdateHealth(int health)
    {
        healthSlider.value = health;
        if(health <= 0)
        {
            lifeBar.SetActive(false);
        }
    }
    public void DisplayPlayerDiedMenu()
    {
        SetAllUIInactive();
        playerDiedMenu.SetActive(true);
        Time.timeScale = 0.0f;
        gamePaused = true;
        //player.SetCanMove(false);
    }
    public void UpdateMagic(int magic)
    {
        magicSlider.value = magic;
    }
    public void PlayerHasRespawned()
    {
        playerDiedMenu.SetActive(false);
        lifeBar.SetActive(true);
        Time.timeScale = 1.0f;
        gamePaused = false;
        player.SetCanMove(true);
        gameStarted = true;
        UpdateHealth(player.GetMaxHealth());
        SetAllUIInactive();
        lifeBar.SetActive(true);
        menusDisplayed = false;
    }
    public void PlayerLost(int amount)
    {
        playerLostMoneyText.text = "Your defeat cost you " + amount + " Gold";
    }
    public void BackToMainMenu()
    {
        player.SetCanMove(false);
        gameStarted = false;
        rm.ClearRoom();
        am.Play("buttonPress");
        Time.timeScale = 1.0f;
        gamePaused = false;
        optionsMenu.SetActive(false);
        startMenu.SetActive(true);
    }
    public void QuitGame()
    {
        am.Play("buttonPress");
        Application.Quit();
    }
    public void RollCredits()
    {
        am.Play("buttonPress");
        if (!playerDiedMenu.activeSelf)
        {
            rm.UnlockNPC(1, true);
            rm.UnlockNPC(0, false);
            rm.UnlockNPC(2, true);
            creditsGone = false;
            Time.timeScale = 0f;
            lifeBar.SetActive(false);
            creditsMenu.SetActive(true);
        }
    }
    public void CreditsButton()
    {
        am.Play("buttonPress");
        if (creditsGone)
        {
            Time.timeScale = 1f;
            lifeBar.SetActive(true);
            creditsMenu.SetActive(false);
            creditsButtonText.text = "Continue Playing";
        }
        else
        {
            creditsButtonText.text = "Close";
            creditsGone = true;
            creditsText.text = "The shop has now been unlocked in the village, head back there to buy some upgrades!";
        }
    }
    public void UpdatePlayerHealthAndMagicMax()
    {
        healthSlider.maxValue = player.GetMaxHealth();
        magicSlider.maxValue = player.GetMaxMagic();
        healthSlider.value = player.GetHealth();
        magicSlider.value = player.GetMagic();
    }
    public bool GetPaused()
    {
        return gamePaused;
    }
    private IEnumerator HideAddedToText()
    {
        yield return new WaitForSeconds(2.0f);
        textDispalyCalls--;
        if(textDispalyCalls <= 0)
        {
            AddedToText.SetActive(false);
        }
    }
    public void DisplayAddedToText(int type, int amount)
    {
        AddedToText.SetActive(true);
        switch (type)
        {
            case 0:
                AddedToText.GetComponent<Text>().text = "Added Item to your Inventory";
                break;
            case 1:
                AddedToText.GetComponent<Text>().text = "Added Lore to your Journal";
                break;
            case 2:
                AddedToText.GetComponent<Text>().text = "Increased quantitiy of Item in your inventory";
                break;            
            case 3:
                AddedToText.GetComponent<Text>().text = "Added Gold to your inventory";
                break;
            case 4:
                AddedToText.GetComponent<Text>().text = "Added Blood to your inventory";
                break;
        }
        textDispalyCalls++;
        StartCoroutine(HideAddedToText());
    }
    private void FillLore()
    {
        lore.Add("No one knows how the first vampire came into existence.");
        lore.Add("To turn someone into a Vampire they need to be drained of the majority of their human blood, then they need to have some vampire blood transfused into them.");
        lore.Add("Vampires have stronger bones, are stronger, faster and they do not need to sleep. Vampires are not weak to sunlight, crosses, holy water or stakes through the heart.");
        lore.Add("You can dash through enemies and take no damage.");
        lore.Add("For a vampire to get stronger they need to feed off of another Vampire. The stronger the vampire they feed off of the bigger the increase in their power.");
        lore.Add("Hint: try dashing through enemies.");
        lore.Add("The bounty hunters are employed to hunt down and kill the rogue vampires, it is seen as a desirable job to have because they are allowed to feed on the rogue vampires increasing their own power.");
        lore.Add("Humans with large amount of 'Gene V' are known as a 'potential'. These humans have more chance to survive the transition process to become a vampire");
        lore.Add("Hold the jump button to slow down your decent slightly.");
        lore.Add("'Potentials' excel in one of the following attributes; knowledge, speed, endurance or strength. This natural ability is amplified when they become a vampire");
        lore.Add("Contrary to popular belief, vampires actually love garlic. Ginger on the other hand leads to bad things.");
        lore.Add("The unwanted is a term for any vampire that the Council wants removing. This can be for a number of reasons some of them just but not all of the reasons are just. This means that the vampire bounty hunters can hunt you if they wish and be rewarded for killing you.");
        lore.Add("Blood pools are scattered throughout the Caves, they are created from the blood of fallen vampires. The high concentration of vampire blood has some strange environmental effects, one of which being the creation of a strange rift that seems to release a restoring energy.");
        lore.Add("The vampire village is the only above surface location known to vampires.");
        lore.Add("Thralls are made when humans that are drained of all their blood and then have vampire blood transferred to them, but not enough blood to turn them into a vampire. The blood is enough to keep them just alive but not enough to turn them into a vampire, the vampire blood damages the body and can cause mutations of all kinds.");
        lore.Add("Struggling to get through the game? Read these hints to get useful knowledge about the lore and gameplay hints.");
        lore.Add("Have you tried aiming you attack up and down? Try Jumping on an enemy while attacking downwards to 'Pogo' off of them.");
        lore.Add("Thralls could be made on purpose by vampires who want a slave or servant or they have been made by accident by in-experienced vampires who don’t know what they are doing.");
        lore.Add("The third and final human Vs vampire war was a decisive victory for the humans.");
        lore.Add("Following the third Human Vs Vampire war the humans imposed upon the vampire council some rules. One of these rules was needing permission from your region's council to create more vampires by transition.");
        lore.Add("Some of the vampires love fighting but if they were seen to be fighting it could be seen as training for another war.To prevent this the vampires created an underground arena in which vampires fight each other.The vampires watching bet on the outcome and the competitors have to put some of their own blood on the line every fight.");
        lore.Add("If you find the game too dificult, try setting your respawn position more often and then just get good.");
        lore.Add("The council tries to regulate the vampire numbers by following the human rules as well as employing bounty hunters to remove the unwanted.");
        lore.Add("As the bounty hunters increased in power the council began to fear what could happen if the bounty hunters ever decided to try and overthrow them. To stop this happening they began to capture dangerous vampires and also capture the bounty hunters they used to employ.");
        lore.Add("The council sometimes decides to lock up the unwanted in the dungeon beneath the cities. The dungeon beneath the cities was built by human slaves before the first human vs vampire war.");
        lore.Add("Humans treat the vampires with disdain and they force them to pay a vampire tax of either money or vampire lives to remind them of the terrible conditions vampires once forced the humans to live in.");
        lore.Add("The dungeon was designed to be a prison with no escape, to make sure no one left alive they filled the dungeon with traps.");
        lore.Add("To enter the arena you must bet a pint of blood. If you win, you get the opponents blood along with a gold reward. This can be very risky for unprepared vampires but has the potential for increasing a vampire's power level if they get the blood of a more powerful vampire.");
        //lore.Add("");
    }
    private void DisplayLore()
    {
        currentLoreNum = Random.Range(0, lore.Count);
        loreText.text = lore[currentLoreNum];
    }
    public void NextLore()
    {
        am.Play("buttonPress");
        ++currentLoreNum;
        if(currentLoreNum >= lore.Count)
        {
            currentLoreNum = 0;
        }
        loreText.text = lore[currentLoreNum];
    }
    public void PreviousLore()
    {
        am.Play("buttonPress");
        --currentLoreNum;
        if (currentLoreNum <= 0)
        {
            currentLoreNum = lore.Count;
        }
        loreText.text = lore[currentLoreNum];
    }
}
