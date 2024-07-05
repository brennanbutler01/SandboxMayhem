using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpeedBoostZone : MonoBehaviour
{
    private float _boostMultiplier = 2f;
    private const float MaxBoostMultiplier = 5f;
    private const float BoostIncreaseRate = .5f;
    public float height = 5f; // Height of the vertical plane
    public float scrollSpeed = 1f;

    private Material _material;

    private void Start()
    {
        var component = GetComponent<Renderer>();
        _material = component.material;

        var boxCollider = GetComponent<BoxCollider>();
        if (boxCollider == null) return;
        
        boxCollider.size = new Vector3(transform.localScale.x, height, 0.1f);
        boxCollider.center = new Vector3(0, height / 2, 0);
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
        }
        carController.ApplyBoostZone(_boostMultiplier);
    }

    private void OnTriggerStay(Collider other)
    {
        var carController = other.GetComponent<CarController>();
        if (carController == null) return;

        if (!other.CompareTag("Player")) return;
        // increase over time to the maximum
        _boostMultiplier = Mathf.Min(_boostMultiplier + BoostIncreaseRate * Time.deltaTime, MaxBoostMultiplier);
        carController.ApplyBoostZone(_boostMultiplier);
        Debug.Log("Increasing the boost");
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

        if (other.CompareTag("Player"))
        {
            Debug.Log("Leaving zone...");
            StartCoroutine(_reset(carController));

        }
    }

 

}