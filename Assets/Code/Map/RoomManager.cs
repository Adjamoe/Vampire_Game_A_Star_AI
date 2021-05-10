using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomManager : MonoBehaviour 
{
    private Room[,] dungeons;
    private GameObject[,] map;
    private Room currentRoom, nextRoom;
    private GameObject mapCanvas;
    private NPCManager npcm;

    private ProcedualGeneration procGen;
    private LootDropsManager ldm;
    private UIManager ui;
    private NodeManager nm;
    private Player pScript;

    private GameObject currentRoomObj;
    public GameObject fadeToBlackSquare, ftbPlayer;
    public GameObject player;
    public GameObject playerIcon;
    private Vector2 playerMapPos;
    private Vector2 respawnPointCoords;
    private int mapXSize;
    private int mapYSize;

    private List<bool> UnlockedNPCs;

    public GameObject walkEnemy, chargeEnemy, jumpEnemy, flyEnemy, firstBoss, flyAI;
    public void start()
    {
        pScript = player.GetComponent<Player>();
        UnlockedNPCs = new List<bool>();
        UnlockedNPCs.Add(true);
        UnlockedNPCs.Add(false);
        UnlockedNPCs.Add(false);
        ldm = GetComponent<LootDropsManager>();
        procGen = GetComponent<ProcedualGeneration>();
        ui = GetComponent<UIManager>();
        nm = GetComponent<NodeManager>();
        CreateRooms();
    }
    private void CreateRooms()
    {
        mapXSize = procGen.GetRoomXSize();
        mapYSize = procGen.GetRoomYSize();
        map = new GameObject[mapXSize, mapYSize];
        dungeons = procGen.CreateMap();

        nextRoom = dungeons[3, 1];
        respawnPointCoords = new Vector2(3, 1);
        CreateGameFirstTime();
        StartCoroutine(FadeFromBlack());
        CreateMapCanvas(3, 1);
    }
    private void CreateMapCanvas(int playerStartX, int playerStartY)
    {
        if(mapCanvas == null)
        {
            mapCanvas = ui.GetMap();
        }
        Vector3 canvasPos = mapCanvas.transform.position;
        

        for(int y = 0; y < mapXSize; y++)
        {
            for(int x = 0; x < mapYSize; x++)
            {
                if (dungeons[y, x] != null) // if there is a room at coords
                {
                    map[y, x] = Instantiate(dungeons[y, x].GetMapRoomLayout(), canvasPos + new Vector3(-3.0f + (x * 2f), 3.5f + (-y * 2f)), Quaternion.identity, mapCanvas.transform);
                    if(x == playerStartY && y == playerStartX) // if player is in the room
                    {
                        dungeons[playerStartX, playerStartY].SetUnlocked(); // player has unlocked the room
                        playerIcon = Instantiate(playerIcon, map[x, y].transform); // create a player icon
                        playerMapPos = new Vector2(playerStartX, playerStartY); // save the position so we have a refernce
                        playerIcon.transform.SetParent(map[(int)playerMapPos.x, (int)playerMapPos.y].transform);
                        playerIcon.transform.localPosition = Vector3.zero;
                    }
                    if(dungeons[y, x].GetUnlocked()) // if unlocked, show it
                    {
                        map[y, x].SetActive(true);
                    }
                    else // if locked still, don't show it
                    {
                        map[y, x].SetActive(false);
                    }
                }
            }
        }
    }

    public void RefreshMap()
    {
        for (int x = 0; x < mapXSize; x++)
        {
            for (int y = 0; y < mapYSize; y++)
            {
                if(dungeons[x, y] != null) // going through each room, check if there are any more unlocked rooms
                {
                    if(dungeons[x, y].GetUnlocked())
                    {
                        map[x, y].SetActive(true);
                    }
                    else
                    {
                        map[x, y].SetActive(false);
                    }
                }
            }
        }
        playerMapPos = new Vector2(currentRoom.GetXPos(), currentRoom.GetYPos()); // save the position so we have a refernce
        playerIcon.transform.SetParent(map[(int)playerMapPos.x, (int)playerMapPos.y].transform);
        playerIcon.transform.localPosition = Vector3.zero;
    }
    private void CreateGameFirstTime()
    {
        currentRoom = nextRoom;
        currentRoomObj = Instantiate(currentRoom.GetRoomLayout(), Vector3.zero, Quaternion.identity);
        currentRoom.CreateRoom();
        player.gameObject.transform.position = currentRoom.GetRespawnPlayerLocation();
        SpawnEnemiesInRoom();

        if (procGen.GetAITest())
        {
            nm.SetUpNodes();
        }
    }
    public void ExitRoom(int _doorExit)
    {
        StartCoroutine(ChangeRoom(_doorExit));
    }

    private IEnumerator ChangeRoom(int _doorExit)
    {
        fadeToBlackSquare.SetActive(true);
        // return 0 = left
        // return 1 = right
        // return 2 = top
        // return 3 = bottom
        while (fadeToBlackSquare.GetComponent<Image>().color.a < 1) // fade to black
        {
            fadeToBlackSquare.GetComponent<Image>().color = new Color(fadeToBlackSquare.GetComponent<Image>().color.r, fadeToBlackSquare.GetComponent<Image>().color.g, fadeToBlackSquare.GetComponent<Image>().color.b, fadeToBlackSquare.GetComponent<Image>().color.a + (Time.deltaTime * 5));
            ftbPlayer.GetComponent<SpriteRenderer>().color = new Color(ftbPlayer.GetComponent<SpriteRenderer>().color.r, ftbPlayer.GetComponent<SpriteRenderer>().color.g, ftbPlayer.GetComponent<SpriteRenderer>().color.b, fadeToBlackSquare.GetComponent<Image>().color.a);
            yield return null; // waits a frame
        }
        yield return new WaitForSeconds(0.8f); // wait as black
        if(!procGen.tutorialCompleted) // don't want to show tutorial if we've already done it
        {
            procGen.tutorialCompleted = true;
            dungeons[currentRoom.GetXPos(), currentRoom.GetYPos()].SetRoomLayout(procGen.GetTutRoomReplacement()); // tutroial room is now a normal room
        }
        ClearRoom();
        yield return new WaitForEndOfFrame(); // need to wait a frame for Instantiate

        switch (_doorExit) // move room depending on door taken
        {
            case 0:
                nextRoom = dungeons[currentRoom.GetXPos(), currentRoom.GetYPos() - 1];
                dungeons[currentRoom.GetXPos(), currentRoom.GetYPos() - 1].SetUnlocked();
                playerMapPos.x -= 1;
                break;
            case 1:
                nextRoom = dungeons[currentRoom.GetXPos(), currentRoom.GetYPos() + 1];
                dungeons[currentRoom.GetXPos(), currentRoom.GetYPos() + 1].SetUnlocked();
                playerMapPos.x += 1;
                break;
            case 2:
                nextRoom = dungeons[currentRoom.GetXPos() - 1, currentRoom.GetYPos()];
                dungeons[currentRoom.GetXPos() - 1, currentRoom.GetYPos()].SetUnlocked();
                playerMapPos.y -= 1;
                break;
            case 3:
                nextRoom = dungeons[currentRoom.GetXPos() + 1, currentRoom.GetYPos()];
                dungeons[currentRoom.GetXPos() + 1, currentRoom.GetYPos()].SetUnlocked();
                playerMapPos.y += 1;
                break;
        }
        player.gameObject.transform.position = new Vector3(1000, 1000, 0);
        currentRoom = nextRoom;
        currentRoomObj = Instantiate(nextRoom.GetRoomLayout(), Vector3.zero, Quaternion.identity); // create next room
        yield return new WaitForEndOfFrame(); // need to wait a frame for Instantiate
        nextRoom.CreateRoom();
        SpawnEnemiesInRoom();
        yield return new WaitForEndOfFrame(); // need to wait a frame for Instantiate
        if (procGen.GetVillageMapPos() == new Vector2(nextRoom.GetXPos(), nextRoom.GetYPos())) // if entering the village, update the NCPs
        {
            npcm = GameObject.Find("NPCManager").GetComponent<NPCManager>(); // only exits in this room so can't do at the top
            npcm.UpdateNPCs();
        }

        if (!procGen.GetAITest() && currentRoom.GetXPos() == 2 && currentRoom.GetYPos() == 2)
        {
            nm.SetUpNodes();
        }
        else
        {
            nm.SetActive(false);
        }

        //player.gameObject.transform.position = nextRoom.GetSpawnPlayerLocation(_doorExit); // use door exit to determine door enterence
        StartCoroutine(FadeFromBlack());
        yield return new WaitForEndOfFrame(); // need to wait a frame for Instantiate
        player.gameObject.transform.position = nextRoom.GetSpawnPlayerLocation(_doorExit); // use door exit to determine door enterence
    }
    private IEnumerator FadeFromBlack()
    {
        fadeToBlackSquare.SetActive(true);
        fadeToBlackSquare.GetComponent<Image>().color = new Color(fadeToBlackSquare.GetComponent<Image>().color.r, fadeToBlackSquare.GetComponent<Image>().color.g, fadeToBlackSquare.GetComponent<Image>().color.b, 1);
        yield return new WaitForSeconds(0.7f); // wait as black
        while (fadeToBlackSquare.GetComponent<Image>().color.a > 0) // fade back in
        {
            fadeToBlackSquare.GetComponent<Image>().color = new Color(fadeToBlackSquare.GetComponent<Image>().color.r, fadeToBlackSquare.GetComponent<Image>().color.g, fadeToBlackSquare.GetComponent<Image>().color.b, fadeToBlackSquare.GetComponent<Image>().color.a - (Time.deltaTime));
            ftbPlayer.GetComponent<SpriteRenderer>().color = new Color(ftbPlayer.GetComponent<SpriteRenderer>().color.r, ftbPlayer.GetComponent<SpriteRenderer>().color.g, ftbPlayer.GetComponent<SpriteRenderer>().color.b, fadeToBlackSquare.GetComponent<Image>().color.a);
            yield return null;
        }
        fadeToBlackSquare.SetActive(false);
    }
    public void PlayerDied()
    {
        StartCoroutine(RespawnPlayer());
    }
    public void ResPlayerBtnClick()
    {
        StartCoroutine(FadeFromBlack());
        ui.PlayerHasRespawned();
    }
    public IEnumerator RespawnPlayer()
    {
        fadeToBlackSquare.SetActive(true);
        while (fadeToBlackSquare.GetComponent<Image>().color.a < 1) // fade to black
        {
            fadeToBlackSquare.GetComponent<Image>().color = new Color(fadeToBlackSquare.GetComponent<Image>().color.r, fadeToBlackSquare.GetComponent<Image>().color.g, fadeToBlackSquare.GetComponent<Image>().color.b, fadeToBlackSquare.GetComponent<Image>().color.a + (Time.deltaTime * 5));
            ftbPlayer.GetComponent<SpriteRenderer>().color = new Color(ftbPlayer.GetComponent<SpriteRenderer>().color.r, ftbPlayer.GetComponent<SpriteRenderer>().color.g, ftbPlayer.GetComponent<SpriteRenderer>().color.b, fadeToBlackSquare.GetComponent<Image>().color.a);
            yield return null;
        }
        yield return new WaitForSeconds(0.8f); // wait as black
        ClearRoom();
        ui.DisplayPlayerDiedMenu();
        yield return new WaitForEndOfFrame(); // need to wait a frame for Instantiate
        yield return new WaitForSeconds(0.5f);

        nextRoom = dungeons[(int)respawnPointCoords.x, (int)respawnPointCoords.y]; // respawn on last res point interacted with
        playerMapPos.x = respawnPointCoords.x;
        playerMapPos.y = respawnPointCoords.y;
        player.gameObject.transform.position = new Vector3(1000, 1000, 0);
        currentRoom = nextRoom;
        currentRoomObj = Instantiate(nextRoom.GetRoomLayout(), Vector3.zero, Quaternion.identity);
        yield return new WaitForEndOfFrame();        
        nextRoom.CreateRoom();
        SpawnEnemiesInRoom();
        yield return new WaitForEndOfFrame();        
        pScript.ResetVelocity();
        player.gameObject.transform.position = nextRoom.GetRespawnPlayerLocation();
        yield return new WaitForEndOfFrame();
    }
    public void ClearRoom()
    {
        ldm.ClearRoomOfLoot(); // refresh room so we don't have duplicates or loading issues
        currentRoom.ClearEnemies();
        Destroy(currentRoomObj);
    }
    public void SetResPoint()
    {
        respawnPointCoords.x = currentRoom.GetXPos();
        respawnPointCoords.y = currentRoom.GetYPos();
    }
    public GameObject GetEnemy(int type)
    {
        int maxEnemyTypes = 3; // not including boss. No one wants randomly spawning bosses!
        if(type == -1)
        {
            type = Random.Range(0, maxEnemyTypes);
        }
        switch(type)
        {
            case 0:
                return walkEnemy;
            case 1:
                return chargeEnemy;
            case 2:
                return jumpEnemy;
            case 3:
                return flyEnemy;
            case 4:
                return firstBoss;
            case 5:
                return flyAI;


            default:
                return walkEnemy;
        }
    }
    private void SpawnEnemiesInRoom()
    {
        currentRoom.SpawnEnemies();
    }
    public List<bool> GetUnlockedNPCs()
    {
        return UnlockedNPCs;
    }
    public void UnlockNPC(int num, bool toggle)
    {
        // 0 = Ginger
        // 1 = ShopKeep
        UnlockedNPCs[num] = toggle;
    }
}
