using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcedualGeneration : MonoBehaviour
{
    private int maxRooms; 
    public GameObject roomA;    // 0    Exit all 
    public GameObject roomA1;    // 19    Exit all variant 
    public GameObject roomD;    // 1    Exit down
    public GameObject roomL;    // 2    Exit left
    public GameObject roomLD;   // 3    Exit left down
    public GameObject roomLR;   // 4    Exit left right
    public GameObject roomLRD;  // 5    Exit left right down
    public GameObject roomLRU;  // 6    Exit left right up
    public GameObject roomLU;   // 7    Exit left up
    public GameObject roomLUD;  // 8    Exit left down
    public GameObject roomR;    // 9    Exit Right
    public GameObject roomR1;    // 20    Exit Right variant
    public GameObject roomRD;   // 10   Exit Right down
    public GameObject roomRU;   // 11   Exit Right up
    public GameObject roomRUD;  // 12   Exit Right up down
    public GameObject roomU;    // 13   Exit up
    public GameObject roomUD;   // 14   Exit up down
    // map objects
    public GameObject mRoomA;    // 0    Exit all 
    public GameObject mRoomD;    // 1    Exit down
    public GameObject mRoomL;    // 2    Exit left
    public GameObject mRoomLD;   // 3    Exit left down
    public GameObject mRoomLR;   // 4    Exit left right
    public GameObject mRoomLRD;  // 5    Exit left right down
    public GameObject mRoomLRU;  // 6    Exit left right up
    public GameObject mRoomLU;   // 7    Exit left up
    public GameObject mRoomLUD;  // 8    Exit left down
    public GameObject mRoomR;    // 9    Exit Right
    public GameObject mRoomRD;   // 10   Exit Right down
    public GameObject mRoomRU;   // 11   Exit Right up
    public GameObject mRoomRUD;  // 12   Exit Right up down
    public GameObject mRoomU;    // 13   Exit up
    public GameObject mRoomUD;   // 14   Exit up down
    public GameObject mRoomBoss;   // 14   Boss room, left
    public GameObject mRoomVillage;   // 14   Boss room, down

    public GameObject GODSROOM;
    private bool GODMODE = false;
    private bool AI_TEST = false;


    public GameObject VillageAboveDungeon; // 15
    public GameObject tutorialRoom; // 16
    public GameObject arenaRoom; // 17
    public GameObject bossRoom; // 18
    public bool tutorialCompleted { get; set; } = false;

    private Vector2 villageMapPos;
    private const int roomXSize = 7, roomYSize = 7; // never ever ever have these numbers different!


    public int GetRoomXSize()
    {
        return roomXSize;
    }
    public int GetRoomYSize()
    {
        return roomYSize;
    }
    public Room[,] CreateMap()
    { // TODO: send biome number s we know what art to use.
        Room[,] map;
        map = new Room[roomXSize,roomYSize] // x is y for some reason... 
        { // GetRoom(x, y, RoomType)
            { GetRoom(0, 0, 15),      null,                     null,                   GetRoom(0, 3, 10),      GetRoom(0, 4, 2) , null, null},
            { GetRoom(1, 0, 11),      GetRoom(1, 1, 4),         GetRoom(1, 2, 5),       GetRoom(1, 3, 7),       null             , null, null},
            { null,                   null,                     GetRoom(2, 2, 12),      GetRoom(2, 3, 3),       null             , null, null},
            { null,                   GetRoom(3, 1, 16),        GetRoom(3, 2, 0),       GetRoom(3, 3, 6),       GetRoom(3, 4, 17), null, null},
            { null,                   null,                     GetRoom(4, 2, 14),      null,                   null             , null, null},
            { null,                   GetRoom(5, 1, 20),         GetRoom(5, 2, 19),       GetRoom(5, 3, 18),      null             , null, null},
            { null,                   null,                     GetRoom(6, 2, 13),      null,                   null             , null, null}
        };
        return map;
    }

    private Room GetRoom(int x, int y, int _roomType) // room type in order as above
    {
        switch(_roomType)
        {
            case 0:
                return new Room(x, y, roomA, mRoomA);
            case 1:
                return new Room(x, y, roomD, mRoomD);
            case 2:
                return new Room(x, y, roomL, mRoomL);
            case 3:
                return new Room(x, y, roomLD, mRoomLD);
            case 4:
                return new Room(x, y, roomLR, mRoomLR);
            case 5:
                return new Room(x, y, roomLRD, mRoomLRD);
            case 6:
                return new Room(x, y, roomLRU, mRoomLRU);
            case 7:
                return new Room(x, y, roomLU, mRoomLU);
            case 8:
                return new Room(x, y, roomLUD, mRoomLUD);
            case 9:
                return new Room(x, y, roomR, mRoomR);
            case 10:
                return new Room(x, y, roomRD, mRoomRD);
            case 11:
                return new Room(x, y, roomRU, mRoomRU);
            case 12:
                return new Room(x, y, roomRUD, mRoomRUD);
            case 13:
                return new Room(x, y, roomU, mRoomU);
            case 14:
                return new Room(x, y, roomUD, mRoomUD);
            case 15:
                villageMapPos = new Vector2(x, y);
                return new Room(x, y, VillageAboveDungeon, mRoomVillage);            
            case 16:
                if (GODMODE)
                {
                    return new Room(x, y, GODSROOM, mRoomR);
                }
                else if (AI_TEST)
                {
                    return new Room(x, y, roomRUD, mRoomRUD);
                }
                else
                {
                    if (tutorialCompleted)
                        return new Room(x, y, roomR, mRoomR);
                    else
                        return new Room(x, y, tutorialRoom, mRoomR);
                }
            case 17:
                return new Room(x, y, arenaRoom, mRoomL);
            case 18:
                return new Room(x, y, bossRoom, mRoomBoss);
            case 19:
                return new Room(x, y, roomA1, mRoomA);
            case 20:
                return new Room(x, y, roomR1, mRoomR);
            default:
                return new Room(x, y, roomA, mRoomA);

        }
    }
    public GameObject GetTutRoomReplacement()
    {
        return roomR;
    }
    public Vector2 GetVillageMapPos()
    {
        return villageMapPos;
    }
    public bool GetAITest()
    {
        return AI_TEST;
    }
}
