using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private MealComponent selectedMealComponent;
    private ObjectControls objectControls;

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

        // Initialize the object controls
        objectControls = new ObjectControls();
        objectControls.Touch.Enable();
        // Enable the touch position event
        objectControls.Touch.TouchPosition.performed += TouchPosition_performed;
    }

    private void TouchPosition_performed(InputAction.CallbackContext context)
    {
        // Casting a ray from the touch position
        Ray ray = Camera.main.ScreenPointToRay(context.ReadValue<Vector2>());

        if (Physics.Raycast(ray, out RaycastHit hit, 1000.0f) && hit.collider)
        {
            if (hit.transform.TryGetComponent(out MealComponent component))
            {
                Debug.Log($"Selected a Meal Component: {hit.collider.name}");
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
}
