using UnityEngine;

public class CoinCollectable: CollectableBase
{
    public override void Collect(GameObject collector)
    {
        var player = collector.GetComponent<PlayerController>();
        if (player == null) return;
    
        Debug.Log("Coins: " + player.coins);
        player.coins++;
    
        // only play sound for real players, not when AI collects 
        if (player.isHuman)
        {
            EventManager.TriggerEvent<CoinCollectionEvent, GameObject>(collector);
        }
        
        // respawn the item
        Respawn();
    }
}