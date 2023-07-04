using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Service
{
    private PlayerController _playerController;

    private enum GameMode
    {
        SpeederSpace,
        SpeederGround,
        BoostBoots,
        RemoteCar,
    }

    private bool _isMobile = false;
    public bool IsMobileWebGl => _isMobile;

#if !UNITY_EDITOR && UNITY_WEBGL
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern bool IsMobile();
#endif

    protected override void OnEnable()
    {
        base.OnEnable();

        _playerController = FindObjectOfType<PlayerController>();
        if (_playerController == null)
        {
            Debug.LogWarning("No player controller found in this scene");
        }

        _isMobile = true;
#if !UNITY_EDITOR && UNITY_WEBGL
        _isMobile = IsMobile();
#endif
    }

    public void EndGame()
    {
        Debug.Log("Game Over");
        var pickUpManager = ServiceLocator.Instance.GetService<PickUpManager>();
        var pickUpsCollected = pickUpManager.PickUpsPickedUp;
        Debug.Log($"You collected {pickUpsCollected} / {pickUpManager.PickUps.Count}");

        var ellaPickUpsCollected = pickUpManager.FoundEllaPickUps;

        foreach (var pickupElla in ellaPickUpsCollected)
        {
            Debug.Log($"You collected Ella's letters: {pickupElla.ToString()}");

            // get corresponding slot in endscreen -> pop-in the found letter(s)

            // yield return a delay -> repeat
        }
        
    }

    public void RespawnPlayer(GameObject player, GameObject checkpoint)
    {
        player.transform.position = checkpoint.transform.position;
    }

    private void Update()
    {
        if (_playerController != null)
        {
            _playerController.UpdateController();
        }       
    }
    private void FixedUpdate()
    {
        if (_playerController != null)
        {
            _playerController.FixedUpdateController();
        }
    }

    public void SetPlayerController(PlayerController playerController = null)
    {
        _playerController = playerController;
    }
}
