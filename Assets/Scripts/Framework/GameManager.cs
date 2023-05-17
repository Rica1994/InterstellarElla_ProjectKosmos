using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Service
{
    private PlayerController _playerController;
    //private void Awake()
    //{
    //    _playerController = FindObjectOfType<PlayerController>();
    //    if (_playerController == null) throw new Exception("No PlayerController found in scene");
    //}

    protected override void OnEnable()
    {
        base.OnEnable();

        _playerController = FindObjectOfType<PlayerController>();
        if (_playerController == null)
        {
            Debug.LogWarning("No player controller found in this scene");
        }
    }

    public void EndGame()
    {
        Debug.Log("Game Over");
        var pickUpManager = ServiceLocator.Instance.GetService<PickUpManager>();
        var pickUpsCollected = pickUpManager.PickUpsPickedUp;
        var ellaPickUpsCollected = pickUpManager.FoundEllaPickUps;
        Debug.Log($"You collected {pickUpsCollected} / {pickUpManager.PickUps.Count}");
        Debug.Log($"You collected Ella's letters: {ellaPickUpsCollected}");
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
    
    public void SetPlayerController(PlayerController playerController = null)
    {
        _playerController = playerController;
    }
}
