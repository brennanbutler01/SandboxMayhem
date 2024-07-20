using UnityEngine;

public class CoinCollectable: CollectableBase
{
    public override void Collect(GameObject collector)
    {
        var player = collector.GetComponent<PlayerController>();
        if (player == null) return;
    
        player.coins++;
        if (player.boostCoins < 5)
        {
            player.boostCoins++;
        }
    
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

            if (!isHuman)
            {
                player.Car.ActivateConsumableSpeedBoost(boostSpeed, boostDuration);
            }
        }

        if (player.boostCoins == 5 && !player.hasEnoughCoinsForBoost)
        {
            player.hasEnoughCoinsForBoost = true;
        }

        // respawn the item
        Respawn();
    }
}