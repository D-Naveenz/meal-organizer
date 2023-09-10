using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

/// <summary>
/// This component listens for images detected by the <c>XRImageTrackingSubsystem</c>
/// and overlays some prefabs on top of the detected image.
/// </summary>
[RequireComponent(typeof(ARTrackedImageManager))]
public class PrefabImagePairManager : MonoBehaviour, ISerializationCallbackReceiver
{
    /// <summary>
    /// Used to associate an `XRReferenceImage` with a Prefab by using the `XRReferenceImage`'s guid as a unique identifier for a particular reference image.
    /// </summary>
    [Serializable]
    struct NamedPrefab
    {
        // System.Guid isn't serializable, so we store the Guid as a string. At runtime, this is converted back to a System.Guid
        public string imageGuid;
        public GameObject imagePrefab;

        public NamedPrefab(Guid guid, GameObject prefab)
        {
            imageGuid = guid.ToString();
            imagePrefab = prefab;
        }
    }

    [SerializeField]
    [HideInInspector]
    List<NamedPrefab> m_PrefabsList = new List<NamedPrefab>();

    Dictionary<Guid, GameObject> m_PrefabsDictionary = new Dictionary<Guid, GameObject>();
    Dictionary<Guid, GameObject> m_Instantiated = new Dictionary<Guid, GameObject>();
    ARTrackedImageManager m_TrackedImageManager;

    [SerializeField]
    [Tooltip("Reference Image Library")]
    XRReferenceImageLibrary m_ImageLibrary;

    /// <summary>
    /// Get the <c>XRReferenceImageLibrary</c>
    /// </summary>
    public XRReferenceImageLibrary imageLibrary
    {
        get => m_ImageLibrary;
        set => m_ImageLibrary = value;
    }

    public void OnBeforeSerialize()
    {
        m_PrefabsList.Clear();
        foreach (var kvp in m_PrefabsDictionary)
        {
            m_PrefabsList.Add(new NamedPrefab(kvp.Key, kvp.Value));
        }
    }

    public void OnAfterDeserialize()
    {
        m_PrefabsDictionary = new Dictionary<Guid, GameObject>();
        foreach (var entry in m_PrefabsList)
        {
            m_PrefabsDictionary.Add(Guid.Parse(entry.imageGuid), entry.imagePrefab);
        }
    }

    void Awake()
    {
        m_TrackedImageManager = GetComponent<ARTrackedImageManager>();
    }

    void OnEnable()
    {
        m_TrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        m_TrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
        {
            // Give the initial image a reasonable default scale
            //var minLocalScalar = Mathf.Min(trackedImage.size.x, trackedImage.size.y) / 2;
            //trackedImage.transform.localScale = new Vector3(minLocalScalar, minLocalScalar, minLocalScalar);

            if (m_PrefabsDictionary.TryGetValue(trackedImage.referenceImage.guid, out var prefab) && !prefab.IsUnityNull())
                m_Instantiated[trackedImage.referenceImage.guid] = Instantiate(prefab, trackedImage.transform);
        }

        foreach (var trackedImage in eventArgs.updated)
        {
            // check if the prefab exists in instantiated dictionary
            if (m_Instantiated.TryGetValue(trackedImage.referenceImage.guid, out var prefab))
            {
                Console.WriteLine("Tracked Image Name: " + trackedImage.referenceImage.name);

                if (trackedImage.trackingState == TrackingState.Tracking)
                {
                    // activate the prefab
                    prefab.SetActive(true);
                    // update the position and rotation of the prefab to the tracked image
                    prefab.transform.SetPositionAndRotation(trackedImage.transform.position, trackedImage.transform.rotation);
                }
                else
                {
                    // deactivate the prefab if the image is no longer tracked
                    prefab.SetActive(false);

                    Console.WriteLine("Deactivated: " + trackedImage.referenceImage.name);
                }
            }
        }

        foreach (var trackedImage in eventArgs.removed)
        {
            Console.WriteLine("Removed: " + trackedImage.referenceImage.name);

            if (m_Instantiated.TryGetValue(trackedImage.referenceImage.guid, out var prefab))
            {
                // remove the prefab from the dictionary
                m_Instantiated.Remove(trackedImage.referenceImage.guid);
                // destroy the prefab
                Destroy(prefab);
            }
        }
    }

    public GameObject GetPrefabForReferenceImage(XRReferenceImage referenceImage)
        => m_PrefabsDictionary.TryGetValue(referenceImage.guid, out var prefab) ? prefab : null;

    public void SetPrefabForReferenceImage(XRReferenceImage referenceImage, GameObject alternativePrefab)
    {
        m_PrefabsDictionary[referenceImage.guid] = alternativePrefab;
        if (m_Instantiated.TryGetValue(referenceImage.guid, out var instantiatedPrefab))
        {
            m_Instantiated[referenceImage.guid] = Instantiate(alternativePrefab, instantiatedPrefab.transform.parent);
            Destroy(instantiatedPrefab);
        }
    }

#if UNITY_EDITOR
    /// <summary>
    /// This customizes the inspector component and updates the prefab list when
    /// the reference image library is changed.
    /// </summary>
    [CustomEditor(typeof(PrefabImagePairManager))]
    class PrefabImagePairManagerInspector : Editor
    {
        List<XRReferenceImage> m_ReferenceImages = new List<XRReferenceImage>();
        bool m_IsExpanded = true;

        bool HasLibraryChanged(XRReferenceImageLibrary library)
        {
            if (library == null)
                return m_ReferenceImages.Count == 0;

            if (m_ReferenceImages.Count != library.count)
                return true;

            for (int i = 0; i < library.count; i++)
            {
                if (m_ReferenceImages[i] != library[i])
                    return true;
            }

            return false;
        }

        public override void OnInspectorGUI()
        {
            //customized inspector
            var behaviour = serializedObject.targetObject as PrefabImagePairManager;

            serializedObject.Update();
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
            }

            var libraryProperty = serializedObject.FindProperty(nameof(m_ImageLibrary));
            EditorGUILayout.PropertyField(libraryProperty);
            var library = libraryProperty.objectReferenceValue as XRReferenceImageLibrary;

            //check library changes
            if (HasLibraryChanged(library))
            {
                if (library)
                {
                    var tempDictionary = new Dictionary<Guid, GameObject>();
                    foreach (var referenceImage in library)
                    {
                        tempDictionary.Add(referenceImage.guid, behaviour.GetPrefabForReferenceImage(referenceImage));
                    }
                    behaviour.m_PrefabsDictionary = tempDictionary;
                }
            }

            // update current
            m_ReferenceImages.Clear();
            if (library)
            {
                foreach (var referenceImage in library)
                {
                    m_ReferenceImages.Add(referenceImage);
                }
            }

            //show prefab list
            m_IsExpanded = EditorGUILayout.Foldout(m_IsExpanded, "Prefab List");
            if (m_IsExpanded)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUI.BeginChangeCheck();

                    var tempDictionary = new Dictionary<Guid, GameObject>();
                    foreach (var image in library)
                    {
                        var prefab = (GameObject)EditorGUILayout.ObjectField(image.name, behaviour.m_PrefabsDictionary[image.guid], typeof(GameObject), false);
                        tempDictionary.Add(image.guid, prefab);
                    }

                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(target, "Update Prefab");
                        behaviour.m_PrefabsDictionary = tempDictionary;
                        EditorUtility.SetDirty(target);
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
