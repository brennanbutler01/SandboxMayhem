using UnityEngine;

namespace Collectables.Coin
{
    public class CoinCollectable: MonoBehaviour, ICollectable, IRespawnable
    {
        public float RespawnTime { get; set; }
        private void Start()
        {
            // 1minute - 2 minute random range to provide variety
            RespawnTime = Random.Range(60f, 120f);
        }
        
        public void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            Collect(other.gameObject);
        }
    
        public void Collect(GameObject collector)
        {
            var player = collector.GetComponent<PlayerController>();
            if (player == null) return;
        
            Debug.Log("Coins: " + player.coins);
            player.coins++;
        
            // only play sound for real players, not when ai collects 
            if (player.isHuman)
            {
                EventManager.TriggerEvent<CoinCollectionEvent, GameObject>(collector);
            }
            
            // respawn the item
            Respawn();
        }


        // let respawn manager handle this - cannot handle respawn inside CoinCollectable because the game object cannot re-activate itself!
        public void Respawn() => 
            RespawnManager.Instance.ScheduleRespawn(gameObject, RespawnTime);
    }
}