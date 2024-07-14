using UnityEngine;

public class CoinCollectable: CollectableBase
{
    public override void Collect(GameObject collector)
    {
        var player = collector.GetComponent<PlayerController>();
        if (player == null) return;
    
        // Debug.Log("Coins: " + player.coins);
        player.coins++;
    
        // only play sound for real players, not when AI collects 
        if (player.isHuman)
        {
            EventManager.TriggerEvent<CoinCollectionEvent, GameObject>(collector);
        }

        if (player.coins % 5 == 0)
        {
            var isHuman = player.isHuman;
            var boostSpeed = isHuman ? 1.5f : 2.5f;
            var boostDuration = isHuman ? 3f : 6f;
            player.Car.ActivateConsumableSpeedBoost(boostSpeed, boostDuration);
        }
        
        // respawn the item
        Respawn();
    }
}