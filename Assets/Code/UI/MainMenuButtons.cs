using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;


public class MainMenuButtons : MonoBehaviour
{
    private float scaleFactor = 1.25f;
    private Vector3 originalScale;
    private void Start()
    {
        originalScale = transform.localScale;
    }
    private void OnMouseOver()
    {
        transform.localScale = new Vector3(originalScale.x * scaleFactor, originalScale.y * scaleFactor, 1);
    }
    private void OnMouseExit()
    {
        transform.localScale = originalScale;
    }

}
