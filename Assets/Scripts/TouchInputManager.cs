using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchInputManager : MonoBehaviour
{
    private MealComponent selectedMealComponent;

    [SerializeField] private LayerMask ARObjectsLayer;
    [SerializeField] private LayerMask ExcludedLayer;

    // Singleton
    public static TouchInputManager Instance { get; private set; }

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

    private void Update()
    {
        // Check if the user is interacting with the UI
        if (Input.touchCount > 0)
        {
            UnityEngine.Touch touch = Input.GetTouch(0);

            if (touch.phase == UnityEngine.TouchPhase.Began)
            {
                // Make sure the raycast hit an AR object and not a UI element
                if (!EventSystem.current.IsPointerOverGameObject(PointerInputModule.kMouseLeftId))
                {
                    // Casting a ray from the touch position
                    Ray ray = Camera.main.ScreenPointToRay(touch.position);

                    if (Physics.Raycast(ray, out RaycastHit hit, 400.0f) && hit.collider && hit.transform.TryGetComponent(out MealComponent component))
                    {
                        SetSelectedMealComponent(component);

                        // Fire the event
                        OnSelectedComponentChanged?.Invoke(this, new SelectedComponentChangedEventArgs { SelectedComponent = component });
                    }
                    else
                    {
                        if (hit.collider)
                        {
                            Debug.Log("Hit layer: " + hit.transform.gameObject.layer);
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
