using UnityEngine;
using UnityEngine.Audio;

public class WindFrequencyChanger : MonoBehaviour
{
    public AudioMixer windMixer;
    public string parameterName = "WindCenterFreq"; // Use the name you gave to the exposed parameter
    public float parameterChangeRate = 1f;
    private float parameterValue;

    void Start()
    {
        // Initialize parameterValue
        bool parameterFound = windMixer.GetFloat(parameterName, out parameterValue);

        // Check if the parameter is retrieved successfully
        if (!parameterFound)
        {
            Debug.LogError("Parameter not found. Check the name: " + parameterName);
        }
    }

    void Update()
    {
        // Update and apply the change if the parameter is found
        if (windMixer.GetFloat(parameterName, out parameterValue))
        {
            parameterValue += parameterChangeRate * Time.deltaTime;

            // Debug: Log the current parameter value to console
            Debug.Log("Parameter Value: " + parameterValue);

            bool setValueResult = windMixer.SetFloat(parameterName, parameterValue);

            // Debug: Check if SetFloat was successful
            if (!setValueResult)
            {
                Debug.LogError("Failed to set parameter: " + parameterName);
            }
        }
    }
}
