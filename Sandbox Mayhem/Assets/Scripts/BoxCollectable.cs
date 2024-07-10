using UnityEngine;

public class BoxCollectable : CollectableBase
{
    public override void Collect(GameObject collector)
    {
        var playerController = collector.GetComponent<IPlayer>();
        playerController.SetDart();
        EventManager.TriggerEvent<BoxBreakEvent, GameObject>(gameObject);
        Respawn();
    }
}
