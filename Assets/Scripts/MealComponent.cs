using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MealComponent : MonoBehaviour
{
    [SerializeField] private GameObject highlighter;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Select()
    {
        highlighter.SetActive(true);
    }

    private void Deselect()
    {
        highlighter.SetActive(false);
    }
}
