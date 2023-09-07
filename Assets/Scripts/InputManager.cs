using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private MealComponent selectedMealComponent;

    // Singleton
    public InputManager Instance { get; private set; }

    public event EventHandler<SelectedComponentChangedEventArgs> OnSelectedComponentChanged;
    public class SelectedComponentChangedEventArgs : EventArgs
    {
        public MealComponent SelectedComponent { get; set; }
    }


    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one Input Manager instance");
        }

        Instance = this;
    }

    private void SetSelectedMealComponent(MealComponent component)
    {
        // Check if a component is already selected
        if (selectedMealComponent != null)
        {
            // Deselect the selected component
            selectedMealComponent.Deselect();

            // Fire the event
            // OnSelectedComponentChanged?.Invoke(this, new SelectedComponentChangedEventArgs { SelectedComponent = null });
        }

        // Select the new component
        selectedMealComponent = component;

        // Fire selection interaction if the selected component is not null
        if (selectedMealComponent != null)
        {
            // Select the new component
            selectedMealComponent.Select();

            // Fire the event
            // OnSelectedComponentChanged?.Invoke(this, new SelectedComponentChangedEventArgs { SelectedComponent = selectedMealComponent });
        }
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // Casting a ray from the mouse position
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider)
            {
                if (hit.transform.TryGetComponent(out MealComponent component))
                {
                    // Debug.Log($"Selected a Meal Component: {hit.collider.name}");
                    SetSelectedMealComponent(component);

                    // Fire the event
                    OnSelectedComponentChanged?.Invoke(this, new SelectedComponentChangedEventArgs { SelectedComponent = component });
                }
            }
            else
            {
                // Debug.Log("Selected nothing");
                SetSelectedMealComponent(null);

                // Fire the event
                OnSelectedComponentChanged?.Invoke(this, new SelectedComponentChangedEventArgs { SelectedComponent = null });
            }
        }
    }
}
