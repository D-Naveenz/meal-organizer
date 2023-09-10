using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodDetailsPanelActions : MonoBehaviour
{
    private InputManager inputManager;
    
    // Start is called before the first frame update
    void Start()
    {
        // hide this panel on start
        Hide();

        // get the input manager
        inputManager = InputManager.Instance;

        // subscribe to the event
        inputManager.OnSelectedComponentChanged += InputManager_OnSelectedComponentChanged;
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
