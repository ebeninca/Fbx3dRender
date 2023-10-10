using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowFOV : MonoBehaviour
{
    public TMP_Text fovText;

    // Update is called once per frame
    void Update()
    {
        if (Camera.main != null)
        {
            // Update the TextMeshPro Text element's text with the camera's FOV value.
            fovText.text = "FOV " + Camera.main.fieldOfView.ToString("F1"); // Display with one decimal place.
        }

    }
}
