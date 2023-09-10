using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Food", menuName = "Food")]
public class FoodScriptableObject : ScriptableObject
{
    public bool requirePlatform = false;

    public Transform prefab;

    public Sprite foodIcon;

    public string foodName;
}
