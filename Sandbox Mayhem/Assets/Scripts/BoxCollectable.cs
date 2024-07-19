using UnityEngine;

public class BoxCollectable : CollectableBase
{
    public override void Collect(GameObject collector)
    {
        var playerController = collector.GetComponent<IPlayer>();
        playerController.SetDart();
        playerController.BoxCollision(gameObject);
        Respawn();
    }
}
