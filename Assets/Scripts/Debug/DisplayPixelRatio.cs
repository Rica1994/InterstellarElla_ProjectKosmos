using UnityEngine;
using UnityEngine.UI;

public class DisplayPixelRatio : MonoBehaviour
{
    public Text pixelRatioText; // Assign this in the inspector

    // Method to be called from JavaScript
    public void UpdatePixelRatio(string pixelRatioString)
    {
        float pixelRatio = float.Parse(pixelRatioString);
        pixelRatioText.text = "Pixel Ratio: " + pixelRatio;
    }
}
