using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Runtime.InteropServices;

public class SDKManager : Service
{
    public event LoadCallback LoadedEvent;

    // Importing JavaScript functions.
    [DllImport("__Internal")]
    private static extern void InitializeSDK(EventHandler eventHandler);

    [DllImport("__Internal")]
    private static extern void SaveState(string stateId, string value);

    [DllImport("__Internal")]
    private static extern void LoadState(string stateId, LoadCallback callback);

    // Delegate matching the expected callback format.
    public delegate void LoadCallback(string stateId, string loadedData);
    // Define the delegate type.
    public delegate void EventHandler(string eventType, bool eventDetail);

    protected override void Awake()
    {
        base.Awake();
        if (_isDestroyed) return;
        // Initialize the SDK with the event handler.
        InitializeSDK(HandleSDKEvent);
    }


    public void LoadScore()
    {
        LoadState("Ella_score", LoadedEvent);
    }

    public void SaveScore(int score)
    {
        SaveState("Ella_score", $"{score}");
    }

    private void HandleSDKEvent(string eventType, bool eventDetail)
    {
        Debug.Log($"SDK Event: {eventType}, Detail: {eventDetail}");
    }

    // Example callback method to handle loaded data.
    private void OnDataLoaded(string stateId, string loadedData)
    {
        Debug.Log("Loaded Data for " + stateId + ": " + loadedData);
    }
}
