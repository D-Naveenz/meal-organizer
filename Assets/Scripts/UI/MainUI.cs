using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    [SerializeField] private Button menuButton;
    [SerializeField] private Button resetButton;
    [SerializeField] private Button detailsButton;
    [SerializeField] private Button orderButton;

    // Start is called before the first frame update
    void Start()
    {
        resetButton.onClick.AddListener(ResetButton_OnClick);
    }

    private void ResetButton_OnClick()
    {
        Utils.ShowAndroidToastMessage("Reset button clicked");
    }
}
