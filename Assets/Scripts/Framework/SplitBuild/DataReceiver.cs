using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataReceiver : MonoBehaviour
{
    [SerializeField]
    private Text _dataText;

    void Start()
    {
        // Read the URL parameters
        string data = GetURLParameter("data");

        if (_dataText != null && data != "")
        {
            _dataText.text = data;
        }
    }

    string GetURLParameter(string paramName)
    {
        string paramValue = string.Empty;
        string url = Application.absoluteURL;
        Debug.Log(url);

        if (url.Contains("?") && url.Contains(paramName))
        {
            int paramStartIndex = url.IndexOf(paramName + "=") + paramName.Length + 1;
            int paramEndIndex = url.IndexOf("&", paramStartIndex);

            if (paramEndIndex == -1)
            {
                paramEndIndex = url.Length;
            }

            paramValue = UnityEngine.Networking.UnityWebRequest.UnEscapeURL(url.Substring(paramStartIndex, paramEndIndex - paramStartIndex));
        }

        return paramValue;
    }
}
