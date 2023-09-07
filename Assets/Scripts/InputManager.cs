using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public event EventHandler<SelectedComponentChangedEventArgs> OnSelectedComponentChanged;
    public class SelectedComponentChangedEventArgs : EventArgs
    {
        public MealComponent SelectedComponent { get; set; }
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

                    // Toggle the selection
                    component.ToggleSelection();

                    // Fire the event
                    OnSelectedComponentChanged?.Invoke(this, new SelectedComponentChangedEventArgs { SelectedComponent = component });
                }
            }
        }
    }
}
