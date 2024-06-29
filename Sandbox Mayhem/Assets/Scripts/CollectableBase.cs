using UnityEngine;

public abstract class CollectableBase : MonoBehaviour, ICollectable, IRespawnable
{
    public float RespawnTime { get; set; }
    public void Respawn() => RespawnManager.Instance.ScheduleRespawn(gameObject, RespawnTime);

    protected virtual void Start()
    {
        RespawnTime = Random.Range(60f, 120f);
    }

    // inheriting collectables will override 
    public abstract void Collect(GameObject collector);
    
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Collect(other.gameObject);
        }
    }
}