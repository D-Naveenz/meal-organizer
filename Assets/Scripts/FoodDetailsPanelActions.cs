using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FoodDetailsPanelActions : MonoBehaviour
{
    private InputManager inputManager;

    [SerializeField] private TextMeshProUGUI categoryText;
    [SerializeField] private TextMeshProUGUI foodNameText;
    
    // Start is called before the first frame update
    void Start()
    {
        // hide this panel on start
        Hide();

        // get the input manager
        inputManager = InputManager.Instance;

        // subscribe to the event
        inputManager.OnSelectedComponentChanged += InputManager_OnSelectedComponentChanged;
        MealComponent.OnSelectedFoodChanged += MealComponent_OnSelectedFoodChanged;
    }

    private void MealComponent_OnSelectedFoodChanged(object sender, MealComponent.SelectedFoodChangedEventArgs e)
    {
        // change the food name text
        if (e.SelectedFood != null)
        {
            foodNameText.text = e.SelectedFood.name;
        }
        else
        {
            foodNameText.text = "None";
        }
    }

    private void InputManager_OnSelectedComponentChanged(object sender, InputManager.SelectedComponentChangedEventArgs e)
    {
        // if the selected component is null, hide the panel
        if (e.SelectedComponent == null)
        {
            Hide();
        }
        else
        {
            // otherwise, show the panel
            Show();

            // check the type of the selected component
            if (e.SelectedComponent.category == MealComponentCategory.MainCourse)
            {
                // set the category text
                categoryText.text = "Main Course";
            }
            else if (e.SelectedComponent.category == MealComponentCategory.SideDish)
            {
                // set the category text
                categoryText.text = "Side Dish";
            }
            else if (e.SelectedComponent.category == MealComponentCategory.Dessert)
            {
                // set the category text
                categoryText.text = "Dessert";
            }
            else if (e.SelectedComponent.category == MealComponentCategory.Drink)
            {
                // set the category text
                categoryText.text = "Drink";
            }
            else
            {
                // set the category text
                categoryText.text = "Unknown";
            }
        }
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
}
