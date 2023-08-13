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
    // reference to the ARTrackedImageManager Component
    private ARTrackedImageManager _trackedImageManager;
    // dictionary to store the reference image name and the prefab
    private Dictionary<string, GameObject> _instantiatedPrefabs = new Dictionary<string, GameObject>();

    // list of prefabs to be instantiated
    // these prefabs should be in the same order as the reference images in the ARTrackedImageManager
    [SerializeField]
    private GameObject[] arPrefabs;

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
            string imageName = trackedImage.referenceImage.name;

            // loop through the list of prefabs
            foreach (var curPrefab in arPrefabs)
            {
                // check whether prefab is matches the tracked image name, and that prefab hasnb't already been created
                if (string.Compare(curPrefab.name, imageName, StringComparison.OrdinalIgnoreCase) == 0 &&
                    !_instantiatedPrefabs.ContainsKey(imageName))
                {
                    // Initiate the prefab, Parenting it to the ArTrackedImage
                    var newPrefab = Instantiate(curPrefab, trackedImage.transform);
                    // add the prefab to the dictionary
                    _instantiatedPrefabs.Add(imageName, newPrefab);
                }
            }
        }

        // loop through all the tracked images that have been updated
        foreach (var trackedImage in args.updated)
        {
            // set the corresponding prefab to active/inactive depending on the tracking state
            if(_instantiatedPrefabs.TryGetValue(trackedImage.referenceImage.name, out GameObject prefab))
                prefab.SetActive(trackedImage.trackingState == TrackingState.Tracking);
        }
    }
}
