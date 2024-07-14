using UnityEngine;

public class SpeedUpCollectable : CollectableBase
{
    public override void Collect(GameObject collector)
    {
        var player = collector.GetComponent<PlayerController>();
        if (player == null) return;
        var isHuman = player.isHuman;
        var boostSpeed = isHuman ? 1.5f : 2.5f;
        var boostDuration = isHuman ? 3f : 6f;
        player.Car.ActivateConsumableSpeedBoost(boostSpeed, boostDuration);
        // Debug.Log("Speeding up...");

        if (player.isHuman)
        {
            EventManager.TriggerEvent<SpeedUpCollectionEvent, GameObject>(collector);
        }

        Respawn();
    }
}