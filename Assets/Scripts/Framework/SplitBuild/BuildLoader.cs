using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildLoader : MonoBehaviour
{
    [SerializeField]
    private Text _dataText;

    [SerializeField]
    private Counter _counter;

    void Start()
    {
        // Read the URL parameters
        string data = GetURLParameter("data");

        if (_dataText != null && data != "")
        {
            _dataText.text = data;
            _counter.counter = int.Parse(data);
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

    public void LoadAnotherBuild(string buildURL)
    {

        // Add the data you want to transfer as URL parameters
        buildURL += "?data=" + UnityEngine.Networking.UnityWebRequest.EscapeURL(_counter.counter.ToString());

        Application.ExternalEval($"window.location.href = '{buildURL}';");
    }
}
