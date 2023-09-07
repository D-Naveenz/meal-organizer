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

    public FoodObject FoodObject { get; set; }


    private void Start()
    {
        // Make sure the highlighter is not visible at the start
        highlighter.SetActive(false);
        // Make sure the placeholder is visible at the start
        platform.SetActive(true);
    }

    public void Select()
    {
        highlighter.SetActive(true);

        SpawnFood(food);
    }

    public void Deselect()
    {
        highlighter.SetActive(false);
    }

    public bool HasFoodObject()
    {
        return FoodObject != null;
    }

    public void ClearFoodObject()
    {
        if (FoodObject != null)
        {
            Destroy(FoodObject.gameObject);
            FoodObject = null;
        }
    }

    public Transform getFoodAnchorPoint()
    {
        return foodAnchorPoint;
    }

    private void SpawnFood(FoodScriptableObject foodSO)
    {
        if (FoodObject == null)
        {
            Transform target;

            if (foodSO.requirePlatform)
            {
                // show the platform
                platform.SetActive(true);
                // The object should be spawned on the platform
                target = foodAnchorPoint;
            }
            else
            {
                // hide the platform
                platform.SetActive(false);
                // The object should be spawned center of the meal component
                target = transform;
            }

            // spawn the food on the target location
            Transform foodTransform = Instantiate(foodSO.prefab, target);
            foodTransform.GetComponent<FoodObject>().MealComponent = this;
        }
        else
        {
            Debug.Log($"MealComponent already has a food. (Meal component: {this.name} | Food: {food.name})");
        }
    }
}
