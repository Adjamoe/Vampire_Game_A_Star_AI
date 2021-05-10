using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public int exitNum;
    private RoomManager roomManger;
    private void Start()
    {
        roomManger = GameObject.Find("RoomManager").GetComponent<RoomManager>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            roomManger.ExitRoom(exitNum);
            collision.gameObject.GetComponent<Player>().EnterDoor(0.8f);
        }
    }
}
