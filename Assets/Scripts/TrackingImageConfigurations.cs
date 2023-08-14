using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;

public enum MealType
{
    MainDish,
    SideDish,
    Beverage,
    Dessert
}

[CreateAssetMenu(fileName = "TrackingImageConfigurations", menuName = "ScriptableObjects/TrackingImageConfigurations")]
public class TrackingImageConfigurations : ScriptableObject
{
    // Struct to store the reference image and the meal type
    [Serializable]
    public struct ImageData
    {
        public string imageName;
        public MealType mealType;

        public ImageData(string image, MealType mealType = MealType.MainDish)
        {
            this.imageName = image;
            this.mealType = mealType;
        }
    }

    [SerializeField]
    private XRReferenceImageLibrary referenceImageLibrary; // reference to the image library

    // list of image-meal type pairs from the reference image library
    public List<ImageData> imageMealTypePairs = new();

    public void PopulateList()
    {
        // clear the list before populating
        imageMealTypePairs.Clear();

        // populate the list
        foreach (var image in referenceImageLibrary)
        {
            imageMealTypePairs.Add(new ImageData(image.name));
        }
    }

    internal MealType GetMealType(string imageName)
    {
        foreach (var imageData in imageMealTypePairs)
        {
            // if the image name matches, return the meal type
            if (string.Compare(imageData.imageName, imageName, StringComparison.OrdinalIgnoreCase) == 0)
            {
                return imageData.mealType;
            }
        }

        // if no match found, return MainDish
        return MealType.MainDish;
    }
}

[CustomEditor(typeof(TrackingImageConfigurations))]
public class TrackingImageConfigurationsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        TrackingImageConfigurations trackingImageConfigurations = (TrackingImageConfigurations)target;

        if (GUILayout.Button("Update List"))
        {
            trackingImageConfigurations.PopulateList();
        }
    }
}
