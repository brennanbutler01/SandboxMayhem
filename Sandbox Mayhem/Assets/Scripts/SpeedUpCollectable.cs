using UnityEngine;

public class SpeedUpCollectable : CollectableBase
{
    public override void Collect(GameObject collector)
    {
        var player = collector.GetComponent<PlayerController>();
        if (player == null) return;
        player.Car.ActivateSpeedBoost(player.isHuman ? 1.5f : 2.5f, player.isHuman ? 3f : 6f);
        Debug.Log("Speeding up...");

        if (player.isHuman)
        {
            EventManager.TriggerEvent<SpeedUpCollectionEvent, GameObject>(collector);
        }

        Respawn();
    }
}