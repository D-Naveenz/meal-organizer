using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARTrackedImageManager))]
public class PrefabImagePairManager : MonoBehaviour
{
    [Serializable]
    struct NamedPrefab
    {
        public MealType mealType;
        public GameObject prefab;

        public NamedPrefab(MealType type, GameObject prefab)
        {
            this.mealType = type;
            this.prefab = prefab;
        }
    }


    // reference to the ARTrackedImageManager Component
    private ARTrackedImageManager _trackedImageManager;
    // dictionary to store the reference image name and the prefab
    private Dictionary<MealType, GameObject> _instantiatedPrefabs = new();

    // Get the reference to the ScriptableObject
    [SerializeField]
    private TrackingImageConfigurations trackingImageConfigurations;
    // list of prefabs to be instantiated
    // these prefabs should be in the same order as the reference images in the ARTrackedImageManager
    [SerializeField]
    private NamedPrefab[] arPrefabs;

    private void Awake()
    {
        // get the reference
        _trackedImageManager = GetComponent<ARTrackedImageManager>();
    }

    private void OnEnable()
    {
        // subscribe to the event
        _trackedImageManager.trackedImagesChanged += OnImageChanged;
    }

    private void OnDisable()
    {
        // unsubscribe to the event
        _trackedImageManager.trackedImagesChanged -= OnImageChanged;
    }

    // Event Handler
    private void OnImageChanged(ARTrackedImagesChangedEventArgs args)
    {
        // loop through all new tracked images that have been detected
        foreach (var trackedImage in args.added)
        {
            // get the name of the tracked image
            string imageName = trackedImage.referenceImage.name;
            // get the corresponding meal type from the ScriptableObject
            MealType mealType = trackingImageConfigurations.GetMealType(imageName);

            // loop through the list of prefabs
            foreach (var curPrefab in arPrefabs)
            {
                // check whether prefab is matches the tracked image name, and that prefab hasnb't already been created
                if (curPrefab.mealType == mealType && !_instantiatedPrefabs.ContainsKey(mealType))
                {
                    // Initiate the prefab, Parenting it to the ArTrackedImage
                    var newPrefab = Instantiate(curPrefab.prefab, trackedImage.transform);
                    // add the prefab to the dictionary
                    _instantiatedPrefabs.Add(mealType, newPrefab);
                }
            }
        }

        // for all prefabs that have been created so far,
        foreach (var trackedImage in args.updated)
        {
            // set the corresponding prefab to active/inactive depending on the tracking state
            if(_instantiatedPrefabs.TryGetValue(trackingImageConfigurations.GetMealType(trackedImage.referenceImage.name), out GameObject prefab))
                prefab.SetActive(trackedImage.trackingState == TrackingState.Tracking);
        }

        // if the AR system has given up looking for the image, remove the corresponding prefab
        foreach (var trackedImage in args.removed)
        {
            MealType type = trackingImageConfigurations.GetMealType(trackedImage.referenceImage.name);

            if (_instantiatedPrefabs.TryGetValue(type, out GameObject prefab))
            {
                // destroy the corresponding prefab
                Destroy(prefab);
                // remove the prefab from the dictionary
                _instantiatedPrefabs.Remove(type);
            }
        }
    }
}
