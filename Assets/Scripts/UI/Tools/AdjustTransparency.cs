using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdjustTransparency : MonoBehaviour
{
    public Image binoculars; // Assign the binoculars UI image in the Inspector

    public void ChangeTransparency(float value)
    {
        if (binoculars != null)
        {
            Color color = binoculars.color;
            color.a = value; // Set the alpha based on the slider value (0 to 1)
            binoculars.color = color;
        }
    }
}