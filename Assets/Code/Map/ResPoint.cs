using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResPoint : MonoBehaviour
{
    public Text text;
    public void SetAsResPoint()
    {
        text.text = "This is set as your Resurrection  location";
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        text.gameObject.SetActive(false);
        text.text = "Press E to set as Resurrection  location";
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            text.gameObject.SetActive(true);
        }
    }
}
