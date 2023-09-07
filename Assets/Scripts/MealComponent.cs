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
    [SerializeField] private Transform foodAnchorPoint;
    [SerializeField] private FoodScriptableObject food;
    private Transform spawnedFood;

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

        SpawnFood(food);
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

    private void SpawnFood(FoodScriptableObject foodSO)
    {
        // destroy the old food
        if (spawnedFood != null)
        {
            Destroy(spawnedFood.gameObject);
        }

        if (foodSO.requirePlatform)
        {
            // show the platform
            platform.SetActive(true);

            // spawn the food on the anchor point
            spawnedFood = Instantiate(foodSO.prefab, foodAnchorPoint);
            spawnedFood.localPosition = Vector3.zero;  // Reset the position of the anchor point
        }
        else
        {
            // hide the platform
            platform.SetActive(false);

            // spawn the food at the center of the meal component
            spawnedFood = Instantiate(foodSO.prefab, transform);
            spawnedFood.localPosition = Vector3.zero;  // Reset the position of the anchor point
        }
    }
}
