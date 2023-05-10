using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Service
{
    public void EndGame()
    {
        Debug.Log("Game Over");
        var pickUpManager = ServiceLocator.Instance.GetService<PickUpManager>();
        var pickUpsCollected = pickUpManager.PickUpsPickedUp;
        var ellaPickUpsCollected = pickUpManager.FoundEllaPickUps;
        Debug.Log($"You collected {pickUpsCollected} / {pickUpManager.PickUps.Count}");
        Debug.Log($"You collected Ella's letters: {ellaPickUpsCollected}");
    }
}
