using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpeedBoostZone : MonoBehaviour
{
    private float _boostMultiplier = 2f;
    private const float MaxBoostMultiplier = 5f;
    private const float BoostIncreaseRate = .5f;
    public float scrollSpeed = .5f;

    private Material _material;

    private void Start()
    {
        var component = GetComponent<Renderer>();
        _material = component.material;
    }

    private void Update()
    {
        // Scroll the texture
        var offset = Time.time * scrollSpeed;
        _material.mainTextureOffset = new Vector2(-offset, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        var carController = other.GetComponent<CarController>();
        if (carController == null) return;

        if (!other.CompareTag("Player")) return;
        var player = other.GetComponent<PlayerController>();
        Debug.Log("Boost zoning...");
        if (player.isHuman)
        {
            EventManager.TriggerEvent<SpeedBoostZoneEvent, GameObject>(other.gameObject);
            carController.ApplyBoostZone(_boostMultiplier);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        var carController = other.GetComponent<CarController>();
        if (carController == null) return;

        if (!other.CompareTag("Player")) return;
        // increase over time to the maximum
        _boostMultiplier = Mathf.Min(_boostMultiplier + BoostIncreaseRate * Time.deltaTime, MaxBoostMultiplier);
        if (carController.GetComponent<PlayerController>().isHuman)
        {
            carController.ApplyBoostZone(_boostMultiplier);
        }
        // Debug.Log("Increasing the boost");
    }
    
    private IEnumerator _reset(CarController carController)
    {
        yield return new WaitForSeconds(Random.Range(1f, 4f));
        carController.RemoveBoostZone();
        _boostMultiplier = 1.5f;
    }

    private void OnTriggerExit(Collider other)
    {
        var carController = other.GetComponent<CarController>();
        if (carController == null) return;

        if (!other.CompareTag("Player")) return;
        
        // Debug.Log("Leaving zone...");
        StartCoroutine(_reset(carController));
    }
}