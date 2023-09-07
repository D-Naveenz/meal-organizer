using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MealComponent : MonoBehaviour
{
    [SerializeField] private GameObject highlighter;
    [SerializeField] private GameObject platform;

    private bool isSelected = false;


    private void Start()
    {
        // Make sure the highlighter is not visible at the start
        highlighter.SetActive(false);
        // Make sure the placeholder is visible at the start
        platform.SetActive(true);
    }

    public void Select()
    {
        isSelected = true;

        highlighter.SetActive(true);
    }

    public void Deselect()
    {
        isSelected = false;

        highlighter.SetActive(false);
    }

    public void ToggleSelection()
    {
        if (isSelected)
        {
            Deselect();
        }
        else
        {
            Select();
        }
    }
}
