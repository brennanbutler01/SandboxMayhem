using UnityEngine;

public class CoinCollector: MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        
        var player = other.GetComponent<PlayerController>();
        
        if (player == null) return;
        
        player.CollectCoin();
        gameObject.SetActive(false);
    }
}