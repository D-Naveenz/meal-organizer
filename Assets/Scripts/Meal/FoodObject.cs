using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodObject : MonoBehaviour
{
    [SerializeField] private FoodScriptableObject foodScriptableObject;

    private MealComponent mealComponent;

    public MealComponent MealComponent
    {
        get => mealComponent;
        set
        {
            // Clean the old meal component
            if (this.mealComponent != null)
            {
                this.mealComponent.ClearFoodObject();
            }

            mealComponent = value;

            // Chec if the new meal component already has a food object
            if (mealComponent.HasFoodObject())
            {
                Debug.LogError("Meal Component already has a Food Object");
            }

            mealComponent.FoodObject = this;

            // Set position and parent
            transform.parent = mealComponent.getFoodAnchorPoint();
            transform.localPosition = Vector3.zero;  // Reset the position of the anchor point
        }
    }

    public FoodScriptableObject GetScriptableObject()
    {
        return foodScriptableObject;
    }
}
