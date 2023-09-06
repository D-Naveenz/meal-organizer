using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInput : MonoBehaviour
{
    private ObjectControls objectControls;

    public event EventHandler OnInteract;

    private void Awake()
    {
        objectControls = new ObjectControls();
        objectControls.MealComponentsMap.Enable();

        objectControls.MealComponentsMap.Interact.performed += Interact_performed;
    }

    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnInteract?.Invoke(this, EventArgs.Empty);
    }
}
